using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// TCS (Tax Collected at Source) Service Implementation
    /// Handles TCS calculation and reporting for B2C jewellery sales
    /// 
    /// TCS Rules for Jewellery (Section 206C(1H)):
    /// - Threshold: ₹10 lakh per financial year per customer
    /// - Rate with PAN: 0.1%
    /// - Rate without PAN: 1%
    /// - Applicable only for B2C customers
    /// </summary>
    public class TcsService : ITcsService
    {
        private readonly ITcsRepository _tcsRepository;
        private readonly IUserKycRepository _userKycRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TcsService> _logger;

        // TCS Constants
        private const decimal TCS_THRESHOLD = 1000000m; // ₹10 lakh
        private const decimal TCS_RATE_WITH_PAN = 0.001m; // 0.1%
        private const decimal TCS_RATE_WITHOUT_PAN = 0.01m; // 1%

        // PAN validation pattern (ABCDE1234F)
        private static readonly Regex PanPattern = new(@"^[A-Z]{5}\d{4}[A-Z]$", RegexOptions.Compiled);

        public TcsService(
            ITcsRepository tcsRepository,
            IUserKycRepository userKycRepository,
            IInvoiceRepository invoiceRepository,
            IUserRepository userRepository,
            ILogger<TcsService> logger)
        {
            _tcsRepository = tcsRepository;
            _userKycRepository = userKycRepository;
            _invoiceRepository = invoiceRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Calculate TCS for a sale transaction
        /// </summary>
        public async Task<TcsCalculationResponseDto> CalculateTcsAsync(TcsCalculationRequestDto request)
        {
            try
            {
                var financialYear = GetFinancialYear(request.TransactionDate);
                var (hasValidPAN, panNumber) = await CheckCustomerPANAsync(request.CustomerId);
                var cumulativeSales = await GetCumulativeSalesAsync(request.CustomerId, financialYear);

                var response = new TcsCalculationResponseDto
                {
                    HasValidPAN = hasValidPAN,
                    PanNumber = panNumber,
                    CumulativeSaleAmount = cumulativeSales,
                    ThresholdLimit = TCS_THRESHOLD,
                    FinancialYear = financialYear
                };

                // Calculate total sales including this transaction
                var totalSales = cumulativeSales + request.SaleAmount;

                // Check if total sales exceed threshold
                if (totalSales <= TCS_THRESHOLD)
                {
                    response.IsTcsApplicable = false;
                    response.TcsRate = 0;
                    response.TcsAmount = 0;
                    response.TcsType = "BelowThreshold";
                    response.IsExempted = false;

                    _logger.LogInformation(
                        "TCS not applicable for Customer {CustomerId}. Total sales: {TotalSales}, Threshold: {Threshold}",
                        request.CustomerId, totalSales, TCS_THRESHOLD);

                    return response;
                }

                // Calculate TCS on amount exceeding threshold
                decimal taxableAmount;
                if (cumulativeSales >= TCS_THRESHOLD)
                {
                    // Already exceeded threshold, TCS on full amount
                    taxableAmount = request.SaleAmount;
                }
                else
                {
                    // First time crossing threshold, TCS only on excess amount
                    taxableAmount = totalSales - TCS_THRESHOLD;
                }

                // Determine rate based on PAN availability
                if (hasValidPAN)
                {
                    response.TcsRate = TCS_RATE_WITH_PAN;
                    response.TcsType = "WithPAN";
                }
                else
                {
                    response.TcsRate = TCS_RATE_WITHOUT_PAN;
                    response.TcsType = "WithoutPAN";
                }

                response.TcsAmount = Math.Round(taxableAmount * response.TcsRate, 2);
                response.IsTcsApplicable = true;
                response.IsExempted = false;

                _logger.LogInformation(
                    "TCS calculated for Customer {CustomerId}. Rate: {Rate}, Amount: {Amount}, Taxable: {Taxable}",
                    request.CustomerId, response.TcsRate, response.TcsAmount, taxableAmount);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating TCS for Customer {CustomerId}", request.CustomerId);
                throw;
            }
        }

        /// <summary>
        /// Get cumulative sales for a customer in a financial year
        /// </summary>
        public async Task<decimal> GetCumulativeSalesAsync(long customerId, string financialYear)
        {
            return await _tcsRepository.GetCumulativeSalesAsync(customerId, financialYear);
        }

        /// <summary>
        /// Check if customer has valid PAN
        /// </summary>
        public async Task<(bool hasValidPAN, string? panNumber)> CheckCustomerPANAsync(long customerId)
        {
            try
            {
                var kyc = await _userKycRepository.GetUserKycByUserIdAsync(customerId);

                if (kyc == null || string.IsNullOrWhiteSpace(kyc.PanCardNumber))
                {
                    _logger.LogDebug("No KYC or PAN found for Customer {CustomerId}", customerId);
                    return (false, null);
                }

                // Validate PAN format
                var panUpper = kyc.PanCardNumber.ToUpper().Trim();
                var isValidFormat = PanPattern.IsMatch(panUpper);

                // PAN is considered valid if format is correct and KYC is verified
                var isValid = isValidFormat && kyc.IsVerified;

                _logger.LogDebug(
                    "PAN check for Customer {CustomerId}: PAN={PAN}, ValidFormat={ValidFormat}, Verified={Verified}, Result={Result}",
                    customerId, panUpper, isValidFormat, kyc.IsVerified, isValid);

                return (isValid, isValidFormat ? panUpper : null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking PAN for Customer {CustomerId}", customerId);
                return (false, null);
            }
        }

        /// <summary>
        /// Create TCS transaction record for an invoice
        /// </summary>
        public async Task<TcsTransactionDto> CreateTcsTransactionAsync(long invoiceId)
        {
            try
            {
                // Check if TCS transaction already exists
                if (await _tcsRepository.ExistsForInvoiceAsync(invoiceId))
                {
                    _logger.LogWarning("TCS transaction already exists for Invoice {InvoiceId}", invoiceId);
                    var existing = await _tcsRepository.GetByInvoiceIdAsync(invoiceId);
                    return MapToDto(existing!);
                }

                // Get invoice details
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                {
                    throw new InvalidOperationException($"Invoice {invoiceId} not found");
                }

                // Calculate TCS
                var request = new TcsCalculationRequestDto
                {
                    CustomerId = invoice.PartyId,
                    SaleAmount = invoice.GrandTotal,
                    TransactionDate = invoice.InvoiceDate
                };

                var tcsCalculation = await CalculateTcsAsync(request);

                // Create TCS transaction record
                var transaction = new TcsTransactionDb
                {
                    InvoiceId = invoiceId,
                    CustomerId = invoice.PartyId,
                    FinancialYear = tcsCalculation.FinancialYear,
                    PanNumber = tcsCalculation.PanNumber,
                    SaleAmount = invoice.GrandTotal,
                    CumulativeSaleAmount = tcsCalculation.CumulativeSaleAmount + invoice.GrandTotal,
                    TcsRate = tcsCalculation.TcsRate,
                    TcsAmount = tcsCalculation.TcsAmount,
                    TcsType = tcsCalculation.TcsType,
                    IsExempted = tcsCalculation.IsExempted,
                    ExemptionReason = tcsCalculation.ExemptionReason,
                    TransactionDate = invoice.InvoiceDate,
                    Quarter = GetQuarter(invoice.InvoiceDate),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = invoice.CreatedBy
                };

                var result = await _tcsRepository.CreateAsync(transaction);

                _logger.LogInformation(
                    "TCS transaction created for Invoice {InvoiceId}. TCS Amount: {TcsAmount}",
                    invoiceId, result.TcsAmount);

                return MapToDto(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TCS transaction for Invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        /// <summary>
        /// Generate Form 26Q report data
        /// </summary>
        public async Task<Form26QReportDto> GenerateForm26QReportAsync(string financialYear, int quarter)
        {
            try
            {
                var transactions = await _tcsRepository.GetByFinancialYearAsync(financialYear, quarter);

                var report = new Form26QReportDto
                {
                    FinancialYear = financialYear,
                    Quarter = quarter,
                    QuarterDescription = GetQuarterDescription(quarter),
                    GeneratedDate = DateTime.UtcNow
                };

                int serialNo = 1;
                foreach (var txn in transactions.OrderBy(t => t.TransactionDate))
                {
                    // Only include transactions where TCS was actually collected
                    if (txn.TcsAmount > 0)
                    {
                        report.Entries.Add(new Form26QEntryDto
                        {
                            SerialNumber = serialNo++,
                            CollecteePAN = txn.PanNumber ?? "PANNA", // PANNA for no PAN
                            CollecteeName = txn.Customer?.Name ?? "",
                            CollecteeAddress = txn.Customer?.Address,
                            CollecteePhone = txn.Customer?.ContactNumber,
                            TransactionDate = txn.TransactionDate,
                            InvoiceNumber = txn.Invoice?.InvoiceNumber ?? "",
                            AmountReceived = txn.SaleAmount,
                            TcsRate = txn.TcsRate * 100, // Convert to percentage
                            TcsAmount = txn.TcsAmount,
                            NatureOfGoods = "Jewellery"
                        });

                        report.TotalTcsCollected += txn.TcsAmount;
                        report.TotalSaleAmount += txn.SaleAmount;
                    }
                }

                report.TotalTransactions = report.Entries.Count;

                _logger.LogInformation(
                    "Form 26Q report generated for FY {FinancialYear} Q{Quarter}. Transactions: {Count}, Total TCS: {TotalTcs}",
                    financialYear, quarter, report.TotalTransactions, report.TotalTcsCollected);

                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Form 26Q report for FY {FinancialYear} Q{Quarter}", financialYear, quarter);
                throw;
            }
        }

        /// <summary>
        /// Get TCS transactions for a customer
        /// </summary>
        public async Task<List<TcsTransactionDto>> GetCustomerTcsTransactionsAsync(long customerId, string? financialYear = null)
        {
            var transactions = await _tcsRepository.GetByCustomerIdAsync(customerId, financialYear);
            return transactions.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Get customer TCS summary
        /// </summary>
        public async Task<CustomerTcsSummaryDto> GetCustomerTcsSummaryAsync(long customerId, string financialYear)
        {
            var (hasValidPAN, panNumber) = await CheckCustomerPANAsync(customerId);
            var transactions = await _tcsRepository.GetByCustomerIdAsync(customerId, financialYear);
            var transactionList = transactions.ToList();

            var customer = await _userRepository.GetUserByIdAsync(customerId);

            return new CustomerTcsSummaryDto
            {
                CustomerId = customerId,
                CustomerName = customer?.Name ?? "",
                CustomerPhone = customer?.ContactNumber,
                CustomerAddress = customer?.Address,
                PanNumber = panNumber,
                HasValidPAN = hasValidPAN,
                FinancialYear = financialYear,
                TotalSales = transactionList.Sum(t => t.SaleAmount),
                TotalTcsCollected = transactionList.Sum(t => t.TcsAmount),
                TransactionCount = transactionList.Count,
                RemainingThreshold = Math.Max(0, TCS_THRESHOLD - transactionList.Sum(t => t.SaleAmount)),
                ThresholdLimit = TCS_THRESHOLD,
                RecentTransactions = transactionList
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(5)
                    .Select(MapToDto)
                    .ToList()
            };
        }

        /// <summary>
        /// Get all TCS transactions
        /// </summary>
        public async Task<List<TcsTransactionDto>> GetTcsTransactionsAsync(string financialYear, int? quarter = null)
        {
            var transactions = await _tcsRepository.GetByFinancialYearAsync(financialYear, quarter);
            return transactions.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Get dashboard summary
        /// </summary>
        public async Task<TcsDashboardSummaryDto> GetDashboardSummaryAsync(string financialYear)
        {
            var (totalTcs, transactionCount, customerCount, totalSales) = await _tcsRepository.GetSummaryAsync(financialYear);
            var quarterlyData = await _tcsRepository.GetQuarterlySummaryAsync(financialYear);

            return new TcsDashboardSummaryDto
            {
                FinancialYear = financialYear,
                TotalTcsCollected = totalTcs,
                TotalTransactions = transactionCount,
                TotalCustomers = customerCount,
                TotalSaleAmount = totalSales,
                QuarterlySummaries = quarterlyData
                    .Select(q => new TcsQuarterlySummaryDto
                    {
                        Quarter = q.Quarter,
                        QuarterDescription = GetQuarterDescription(q.Quarter),
                        TcsCollected = q.TcsAmount,
                        TransactionCount = q.TransactionCount,
                        SaleAmount = q.SaleAmount
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Get TCS transaction by ID
        /// </summary>
        public async Task<TcsTransactionDto?> GetTcsTransactionByIdAsync(long id)
        {
            var transaction = await _tcsRepository.GetByIdAsync(id);
            return transaction != null ? MapToDto(transaction) : null;
        }

        /// <summary>
        /// Get TCS transaction by invoice ID
        /// </summary>
        public async Task<TcsTransactionDto?> GetTcsTransactionByInvoiceIdAsync(long invoiceId)
        {
            var transaction = await _tcsRepository.GetByInvoiceIdAsync(invoiceId);
            return transaction != null ? MapToDto(transaction) : null;
        }

        /// <summary>
        /// Get current financial year
        /// </summary>
        public string GetCurrentFinancialYear()
        {
            return GetFinancialYear(DateTime.Now);
        }

        /// <summary>
        /// Get financial year for a date
        /// </summary>
        public string GetFinancialYear(DateTime date)
        {
            // Indian financial year: April 1 to March 31
            if (date.Month >= 4)
            {
                return $"{date.Year}-{(date.Year + 1).ToString().Substring(2)}";
            }
            else
            {
                return $"{date.Year - 1}-{date.Year.ToString().Substring(2)}";
            }
        }

        /// <summary>
        /// Get quarter for a date
        /// </summary>
        private int GetQuarter(DateTime date)
        {
            // Q1: Apr-Jun, Q2: Jul-Sep, Q3: Oct-Dec, Q4: Jan-Mar
            return date.Month switch
            {
                >= 4 and <= 6 => 1,
                >= 7 and <= 9 => 2,
                >= 10 and <= 12 => 3,
                _ => 4
            };
        }

        /// <summary>
        /// Get quarter description
        /// </summary>
        private string GetQuarterDescription(int quarter)
        {
            return quarter switch
            {
                1 => "Q1 (Apr-Jun)",
                2 => "Q2 (Jul-Sep)",
                3 => "Q3 (Oct-Dec)",
                4 => "Q4 (Jan-Mar)",
                _ => $"Q{quarter}"
            };
        }

        /// <summary>
        /// Map database entity to DTO
        /// </summary>
        private TcsTransactionDto MapToDto(TcsTransactionDb db)
        {
            return new TcsTransactionDto
            {
                Id = db.Id,
                InvoiceId = db.InvoiceId,
                InvoiceNumber = db.Invoice?.InvoiceNumber ?? "",
                CustomerId = db.CustomerId,
                CustomerName = db.Customer?.Name ?? "",
                CustomerPhone = db.Customer?.ContactNumber,
                CustomerAddress = db.Customer?.Address,
                FinancialYear = db.FinancialYear,
                PanNumber = db.PanNumber,
                SaleAmount = db.SaleAmount,
                CumulativeSaleAmount = db.CumulativeSaleAmount,
                TcsRate = db.TcsRate,
                TcsAmount = db.TcsAmount,
                TcsType = db.TcsType,
                IsExempted = db.IsExempted,
                ExemptionReason = db.ExemptionReason,
                TransactionDate = db.TransactionDate,
                Quarter = db.Quarter,
                CreatedDate = db.CreatedDate
            };
        }
    }
}
