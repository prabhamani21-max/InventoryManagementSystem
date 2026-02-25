using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using InventoryManagementSytem.Common.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// E-Invoice Service Implementation for GST Compliance
    /// Handles IRN generation, QR code generation, and e-invoice cancellation with NIC portal
    /// </summary>
    public class EInvoiceService : IEInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<EInvoiceService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        // NIC Portal Configuration
        private readonly string _nicBaseUrl;
        private readonly string _nicAuthUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _gstin;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _isProductionMode;

        public EInvoiceService(
            IInvoiceRepository invoiceRepository,
            ILogger<EInvoiceService> logger,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;

            // Load NIC Portal configuration from appsettings
            var eInvoiceConfig = configuration.GetSection("EInvoice");
            _isProductionMode = eInvoiceConfig.GetValue<bool>("IsProductionMode");
            _nicBaseUrl = _isProductionMode
                ? eInvoiceConfig["ProductionBaseUrl"] ?? "https://einvoice1.gst.gov.in"
                : eInvoiceConfig["SandboxBaseUrl"] ?? "https://einv-apisandbox.nic.in";
            _nicAuthUrl = $"{_nicBaseUrl}/connect/token";
            _clientId = eInvoiceConfig["ClientId"] ?? "";
            _clientSecret = eInvoiceConfig["ClientSecret"] ?? "";
            _gstin = eInvoiceConfig["Gstin"] ?? "";
            _username = eInvoiceConfig["Username"] ?? "";
            _password = eInvoiceConfig["Password"] ?? "";
        }

        /// <summary>
        /// Generate IRN (Invoice Reference Number) from NIC portal
        /// </summary>
        public async Task<EInvoiceResponseDto> GenerateIRNAsync(long invoiceId)
        {
            _logger.LogInformation("Generating IRN for Invoice {InvoiceId}", invoiceId);

            try
            {
                // Get invoice details
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                {
                    return new EInvoiceResponseDto
                    {
                        Status = "Error",
                        ErrorMessage = "Invoice not found"
                    };
                }

                // Check if IRN already exists
                if (!string.IsNullOrEmpty(invoice.IRN))
                {
                    _logger.LogWarning("IRN already exists for Invoice {InvoiceId}: {IRN}", invoiceId, invoice.IRN);
                    return new EInvoiceResponseDto
                    {
                        IRN = invoice.IRN,
                        AcknowledgementNumber = invoice.AcknowledgementNumber,
                        AcknowledgementDate = invoice.AcknowledgementDate,
                        QRCode = invoice.QRCode,
                        Status = invoice.EInvoiceStatus ?? "Generated",
                        IRNGeneratedDate = invoice.IRNGeneratedDate
                    };
                }

                // Check eligibility
                if (!await IsEligibleForEInvoicingAsync(invoiceId))
                {
                    return new EInvoiceResponseDto
                    {
                        Status = "Error",
                        ErrorMessage = "Invoice is not eligible for e-invoicing. B2B invoices with GSTIN are required."
                    };
                }

                // Build e-invoice request
                var eInvoiceRequest = await BuildEInvoiceRequestAsync(invoice);

                // Get authentication token
                var authToken = await GetAuthTokenAsync();
                if (string.IsNullOrEmpty(authToken))
                {
                    return new EInvoiceResponseDto
                    {
                        Status = "Error",
                        ErrorMessage = "Failed to authenticate with NIC portal"
                    };
                }

                // Call NIC API to generate IRN
                var response = await CallNICGenerateIRNAsync(authToken, eInvoiceRequest);

                if (response.Status == "Generated" && !string.IsNullOrEmpty(response.IRN))
                {
                    // Update invoice with IRN details
                    await UpdateInvoiceWithIRNAsync(invoiceId, response);

                    _logger.LogInformation("IRN generated successfully for Invoice {InvoiceId}: {IRN}", invoiceId, response.IRN);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating IRN for Invoice {InvoiceId}", invoiceId);
                return new EInvoiceResponseDto
                {
                    Status = "Error",
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Cancel e-invoice on NIC portal
        /// </summary>
        public async Task<bool> CancelEInvoiceAsync(long invoiceId, string cancelReason)
        {
            _logger.LogInformation("Cancelling e-invoice for Invoice {InvoiceId}", invoiceId);

            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null || string.IsNullOrEmpty(invoice.IRN))
                {
                    _logger.LogWarning("Invoice not found or IRN not generated for Invoice {InvoiceId}", invoiceId);
                    return false;
                }

                // Get authentication token
                var authToken = await GetAuthTokenAsync();
                if (string.IsNullOrEmpty(authToken))
                {
                    _logger.LogError("Failed to authenticate with NIC portal for cancellation");
                    return false;
                }

                // Call NIC API to cancel IRN
                var cancelRequest = new EInvoiceCancelRequestDto
                {
                    IRN = invoice.IRN,
                    CancelReason = cancelReason
                };

                var response = await CallNICCancelIRNAsync(authToken, cancelRequest);

                if (response)
                {
                    // Update invoice with cancellation details
                    await UpdateInvoiceWithCancellationAsync(invoiceId, cancelReason);
                    _logger.LogInformation("E-invoice cancelled successfully for Invoice {InvoiceId}", invoiceId);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling e-invoice for Invoice {InvoiceId}", invoiceId);
                return false;
            }
        }

        /// <summary>
        /// Get IRN details from NIC portal
        /// </summary>
        public async Task<EInvoiceResponseDto?> GetIRNDetailsAsync(string irn)
        {
            _logger.LogInformation("Getting IRN details for {IRN}", irn);

            try
            {
                var authToken = await GetAuthTokenAsync();
                if (string.IsNullOrEmpty(authToken))
                {
                    return new EInvoiceResponseDto
                    {
                        Status = "Error",
                        ErrorMessage = "Failed to authenticate with NIC portal"
                    };
                }

                return await CallNICGetIRNDetailsAsync(authToken, irn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting IRN details for {IRN}", irn);
                return new EInvoiceResponseDto
                {
                    Status = "Error",
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Generate QR code for invoice
        /// </summary>
        public async Task<string> GenerateQRCodeAsync(long invoiceId)
        {
            _logger.LogInformation("Generating QR code for Invoice {InvoiceId}", invoiceId);

            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                {
                    throw new InvalidOperationException("Invoice not found");
                }

                // If QR code already exists, return it
                if (!string.IsNullOrEmpty(invoice.QRCode))
                {
                    return invoice.QRCode;
                }

                // Build QR code data
                var qrData = BuildQRCodeData(invoice);

                // Generate QR code image
                var qrCodeBase64 = GenerateQRCodeImage(qrData);

                // Update invoice with QR code
                invoice.QRCode = qrCodeBase64;
                await _invoiceRepository.UpdateInvoiceAsync(invoice);

                return qrCodeBase64;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code for Invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        /// <summary>
        /// Check if invoice is eligible for e-invoicing
        /// </summary>
        public async Task<bool> IsEligibleForEInvoicingAsync(long invoiceId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null) return false;

            // E-invoicing is mandatory for B2B invoices
            // Check if party has GSTIN (B2B transaction)
            if (!string.IsNullOrEmpty(invoice.PartyGSTIN))
            {
                // Check if invoice value exceeds threshold (currently ₹5 lakhs for most businesses)
                if (invoice.GrandTotal >= 500000)
                {
                    return true;
                }
            }

            // For export invoices
            if (string.IsNullOrEmpty(invoice.PartyGSTIN) && invoice.PartyType == PartyType.CUSTOMER)
            {
                // Could be export - check additional conditions
                return false;
            }

            return false;
        }

        /// <summary>
        /// Sync invoice with NIC portal
        /// </summary>
        public async Task<EInvoiceResponseDto> SyncWithNICPortalAsync(long invoiceId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return new EInvoiceResponseDto
                {
                    Status = "Error",
                    ErrorMessage = "Invoice not found"
                };
            }

            // If IRN exists, get details from NIC
            if (!string.IsNullOrEmpty(invoice.IRN))
            {
                return await GetIRNDetailsAsync(invoice.IRN) ?? new EInvoiceResponseDto
                {
                    Status = "Error",
                    ErrorMessage = "Failed to get IRN details"
                };
            }

            // Otherwise, generate new IRN
            return await GenerateIRNAsync(invoiceId);
        }

        #region Private Helper Methods

        /// <summary>
        /// Get authentication token from NIC portal
        /// </summary>
        private async Task<string?> GetAuthTokenAsync()
        {
            try
            {
                var tokenRequest = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", _username },
                    { "password", EncryptPassword(_password) },
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "gstin", _gstin }
                };

                var formContent = new FormUrlEncodedContent(tokenRequest);
                var response = await _httpClient.PostAsync(_nicAuthUrl, formContent);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<NICTokenResponse>();
                    return tokenResponse?.AccessToken;
                }

                _logger.LogError("Failed to get auth token from NIC portal. Status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting auth token from NIC portal");
                return null;
            }
        }

        /// <summary>
        /// Encrypt password using public key
        /// </summary>
        private string EncryptPassword(string password)
        {
            // In production, use the public key provided by NIC
            // For sandbox, this might be different
            using (var rsa = RSA.Create())
            {
                // Load NIC public key (this should be configured)
                var publicKeyPem = _configuration["EInvoice:PublicKey"] ?? "";
                if (!string.IsNullOrEmpty(publicKeyPem))
                {
                    rsa.ImportFromPem(publicKeyPem);
                    var encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(password), RSAEncryptionPadding.Pkcs1);
                    return Convert.ToBase64String(encryptedBytes);
                }

                // For sandbox/testing, return base64 encoded password
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Build e-invoice request from invoice data
        /// </summary>
        private async Task<EInvoiceRequestDto> BuildEInvoiceRequestAsync(Invoice invoice)
        {
            var request = new EInvoiceRequestDto
            {
                TransactionDetails = new EInvoiceTransactionDetails
                {
                    TaxScheme = "GST",
                    SupplyType = string.IsNullOrEmpty(invoice.PartyGSTIN) ? "B2C" : "B2B"
                },
                DocumentDetails = new EInvoiceDocumentDetails
                {
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    InvoiceType = "INV"
                },
                SellerDetails = new EInvoiceSellerDetails
                {
                    GSTIN = invoice.CompanyGSTIN ?? _gstin,
                    LegalName = invoice.CompanyName,
                    Address1 = invoice.CompanyAddress,
                    Phone = invoice.CompanyPhone,
                    Email = invoice.CompanyEmail
                },
                BuyerDetails = new EInvoiceBuyerDetails
                {
                    GSTIN = invoice.PartyGSTIN,
                    LegalName = invoice.PartyName,
                    Address1 = invoice.PartyAddress,
                    Phone = invoice.PartyPhone,
                    Email = invoice.PartyEmail
                },
                ValueDetails = new EInvoiceValueDetails
                {
                    TotalInvoiceValue = invoice.GrandTotal,
                    TaxableValue = invoice.TaxableAmount,
                    CGSTAmount = invoice.CGSTAmount,
                    SGSTAmount = invoice.SGSTAmount,
                    IGSTAmount = invoice.IGSTAmount,
                    Discount = invoice.DiscountAmount,
                    RoundOff = invoice.RoundOff
                }
            };

            // Add items
            int serialNo = 1;
            foreach (var item in invoice.InvoiceItems)
            {
                request.ItemList.Add(new EInvoiceItemDetails
                {
                    SerialNumber = serialNo++,
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    Unit = "NOS",
                    UnitPrice = item.TaxableAmount / item.Quantity,
                    GrossAmount = item.ItemSubtotal,
                    Discount = item.Discount,
                    TaxableValue = item.TaxableAmount,
                    CGSTRate = item.CGSTAmount > 0 ? (item.CGSTAmount / item.TaxableAmount) * 100 : 0,
                    CGSTAmount = item.CGSTAmount,
                    SGSTRate = item.SGSTAmount > 0 ? (item.SGSTAmount / item.TaxableAmount) * 100 : 0,
                    SGSTAmount = item.SGSTAmount,
                    IGSTRate = item.IGSTAmount > 0 ? (item.IGSTAmount / item.TaxableAmount) * 100 : 0,
                    IGSTAmount = item.IGSTAmount,
                    TotalAmount = item.TotalAmount
                });
            }

            return request;
        }

        /// <summary>
        /// Call NIC API to generate IRN
        /// </summary>
        private async Task<EInvoiceResponseDto> CallNICGenerateIRNAsync(string authToken, EInvoiceRequestDto request)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
                _httpClient.DefaultRequestHeaders.Add("gstin", _gstin);

                var generateUrl = $"{_nicBaseUrl}/api/einvoice/v1.03/Invoice";
                var jsonContent = JsonContent.Create(request);

                var response = await _httpClient.PostAsync(generateUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var nicResponse = await response.Content.ReadFromJsonAsync<NICGenerateIRNResponse>();
                    if (nicResponse != null)
                    {
                        return new EInvoiceResponseDto
                        {
                            IRN = nicResponse.Irn,
                            AcknowledgementNumber = nicResponse.AckNo,
                            AcknowledgementDate = ParseNICDate(nicResponse.AckDt),
                            QRCode = nicResponse.SignedQRCode,
                            SignedInvoice = nicResponse.SignedInvoice,
                            Status = "Generated",
                            IRNGeneratedDate = DateTime.UtcNow
                        };
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("NIC IRN generation failed. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, errorContent);

                return new EInvoiceResponseDto
                {
                    Status = "Error",
                    ErrorMessage = $"NIC API Error: {response.StatusCode}",
                    ErrorCode = ((int)response.StatusCode).ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NIC generate IRN API");
                return new EInvoiceResponseDto
                {
                    Status = "Error",
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Call NIC API to cancel IRN
        /// </summary>
        private async Task<bool> CallNICCancelIRNAsync(string authToken, EInvoiceCancelRequestDto request)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
                _httpClient.DefaultRequestHeaders.Add("gstin", _gstin);

                var cancelUrl = $"{_nicBaseUrl}/api/einvoice/v1.03/Invoice/Cancel";
                var jsonContent = JsonContent.Create(request);

                var response = await _httpClient.PostAsync(cancelUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var cancelResponse = await response.Content.ReadFromJsonAsync<NICCancelResponse>();
                    return cancelResponse?.Status == "Success";
                }

                _logger.LogError("NIC IRN cancellation failed. Status: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NIC cancel IRN API");
                return false;
            }
        }

        /// <summary>
        /// Call NIC API to get IRN details
        /// </summary>
        private async Task<EInvoiceResponseDto?> CallNICGetIRNDetailsAsync(string authToken, string irn)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
                _httpClient.DefaultRequestHeaders.Add("gstin", _gstin);

                var detailsUrl = $"{_nicBaseUrl}/api/einvoice/v1.03/Invoice/IRN/{irn}";
                var response = await _httpClient.GetAsync(detailsUrl);

                if (response.IsSuccessStatusCode)
                {
                    var nicResponse = await response.Content.ReadFromJsonAsync<NICGenerateIRNResponse>();
                    if (nicResponse != null)
                    {
                        return new EInvoiceResponseDto
                        {
                            IRN = nicResponse.Irn,
                            AcknowledgementNumber = nicResponse.AckNo,
                            AcknowledgementDate = ParseNICDate(nicResponse.AckDt),
                            QRCode = nicResponse.SignedQRCode,
                            Status = nicResponse.Status ?? "Generated"
                        };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NIC get IRN details API");
                return null;
            }
        }

        /// <summary>
        /// Build QR code data string
        /// </summary>
        private string BuildQRCodeData(Invoice invoice)
        {
            // QR code format as per NIC specification
            var qrData = new
            {
                irn = invoice.IRN ?? "",
                ack_no = invoice.AcknowledgementNumber ?? "",
                ack_dt = invoice.AcknowledgementDate?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                gstin = invoice.CompanyGSTIN ?? "",
                total = invoice.GrandTotal,
                ityp = "INV",
                dt = invoice.InvoiceDate.ToString("dd/MM/yyyy"),
                inv_no = invoice.InvoiceNumber
            };

            return JsonSerializer.Serialize(qrData);
        }

        /// <summary>
        /// Generate QR code image as base64
        /// </summary>
        private string GenerateQRCodeImage(string data)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                var qrCodeBytes = qrCode.GetGraphic(20);
                return Convert.ToBase64String(qrCodeBytes);
            }
        }

        /// <summary>
        /// Update invoice with IRN details
        /// </summary>
        private async Task UpdateInvoiceWithIRNAsync(long invoiceId, EInvoiceResponseDto response)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            if (invoice != null)
            {
                invoice.IRN = response.IRN;
                invoice.IRNGeneratedDate = response.IRNGeneratedDate ?? DateTime.UtcNow;
                invoice.QRCode = response.QRCode;
                invoice.AcknowledgementNumber = response.AcknowledgementNumber;
                invoice.AcknowledgementDate = response.AcknowledgementDate;
                invoice.EInvoiceStatus = "Generated";

                await _invoiceRepository.UpdateInvoiceAsync(invoice);
            }
        }

        /// <summary>
        /// Update invoice with cancellation details
        /// </summary>
        private async Task UpdateInvoiceWithCancellationAsync(long invoiceId, string cancelReason)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            if (invoice != null)
            {
                invoice.EInvoiceStatus = "Cancelled";
                invoice.EInvoiceCancelledDate = DateTime.UtcNow;
                invoice.EInvoiceCancelReason = cancelReason;

                await _invoiceRepository.UpdateInvoiceAsync(invoice);
            }
        }

        /// <summary>
        /// Parse NIC date format
        /// </summary>
        private DateTime? ParseNICDate(string? nicDate)
        {
            if (string.IsNullOrEmpty(nicDate)) return null;

            // NIC date format: "dd/MM/yyyy HH:mm:ss"
            if (DateTime.TryParseExact(nicDate, "dd/MM/yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var result))
            {
                return result;
            }

            return DateTime.TryParse(nicDate, out result) ? result : null;
        }

        #endregion

        #region NIC Response Models

        private class NICTokenResponse
        {
            public string? AccessToken { get; set; }
            public string? TokenType { get; set; }
            public int ExpiresIn { get; set; }
        }

        private class NICGenerateIRNResponse
        {
            public string? Irn { get; set; }
            public string? AckNo { get; set; }
            public string? AckDt { get; set; }
            public string? SignedQRCode { get; set; }
            public string? SignedInvoice { get; set; }
            public string? Status { get; set; }
            public string? ErrorDetails { get; set; }
        }

        private class NICCancelResponse
        {
            public string? Status { get; set; }
            public string? ErrorDetails { get; set; }
        }

        #endregion
    }
}
