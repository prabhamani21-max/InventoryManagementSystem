using System;
using System.Linq;
using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// Invoice PDF Service Implementation
    /// Uses QuestPDF to generate professional invoice documents
    /// </summary>
    public class InvoicePdfService : IInvoicePdfService
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicePdfService> _logger;

        public InvoicePdfService(
            IInvoiceService invoiceService,
            ILogger<InvoicePdfService> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;

            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Generate a PDF document for an invoice by ID
        /// </summary>
        public async Task<byte[]> GenerateInvoicePdfAsync(long invoiceId)
        {
            _logger.LogInformation("Generating PDF for invoice ID: {InvoiceId}", invoiceId);

            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice not found for ID: {InvoiceId}", invoiceId);
                return Array.Empty<byte>();
            }

            var pdfBytes = await GeneratePdfAsync(invoice);
            _logger.LogInformation("Generated PDF for invoice ID: {InvoiceId}. Size: {Size} bytes", invoiceId, pdfBytes.Length);
            return pdfBytes;
        }

        /// <summary>
        /// Generate a PDF document for an invoice by invoice number
        /// </summary>
        public async Task<byte[]> GenerateInvoicePdfByNumberAsync(string invoiceNumber)
        {
            _logger.LogInformation("Generating PDF for invoice: {InvoiceNumber}", invoiceNumber);

            var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice not found: {InvoiceNumber}", invoiceNumber);
                return Array.Empty<byte>();
            }

            var pdfBytes = await GeneratePdfAsync(invoice);
            _logger.LogInformation("Generated PDF for invoice {InvoiceNumber}. Size: {Size} bytes", invoiceNumber, pdfBytes.Length);
            return pdfBytes;
        }

        private async Task<byte[]> GeneratePdfAsync(InvoiceResponseDto invoice)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(20, Unit.Point);

                        page.Header().Element(container => ComposeHeader(container, invoice));
                        page.Content().Element(container => ComposeContent(container, invoice));
                        page.Footer().Element(ComposeFooter);
                    });
                });

                return document.GeneratePdf();
            });
        }

        private void ComposeHeader(IContainer container, InvoiceResponseDto invoice)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(invoice.CompanyName)
                        .FontSize(18).Bold().FontColor(Colors.Blue.Medium);

                    if (!string.IsNullOrWhiteSpace(invoice.CompanyAddress))
                        column.Item().Text(invoice.CompanyAddress).FontSize(9);

                    if (!string.IsNullOrWhiteSpace(invoice.CompanyPhone))
                        column.Item().Text($"Phone: {invoice.CompanyPhone}").FontSize(9);

                    if (!string.IsNullOrWhiteSpace(invoice.CompanyEmail))
                        column.Item().Text($"Email: {invoice.CompanyEmail}").FontSize(9);

                    if (!string.IsNullOrWhiteSpace(invoice.CompanyGSTIN))
                        column.Item().Text($"GSTIN: {invoice.CompanyGSTIN}").FontSize(9);

                    if (!string.IsNullOrWhiteSpace(invoice.CompanyHallmarkLicense))
                        column.Item().Text($"Hallmark License: {invoice.CompanyHallmarkLicense}").FontSize(9);
                });

                row.ConstantItem(220).AlignRight().Column(column =>
                {
                    column.Item().Text("TAX INVOICE")
                        .FontSize(20).Bold().FontColor(Colors.Blue.Medium);

                    column.Item().Text($"Invoice No: {invoice.InvoiceNumber}")
                        .FontSize(10).Bold();

                    column.Item().Text($"Date: {invoice.InvoiceDate:dd MMM yyyy}")
                        .FontSize(10);

                    if (invoice.SaleOrderId.HasValue)
                        column.Item().Text($"Order Ref: #{invoice.SaleOrderId}")
                            .FontSize(9).FontColor(Colors.Grey.Medium);

                    if (!string.IsNullOrWhiteSpace(invoice.OrderNumber))
                        column.Item().Text($"Order No: {invoice.OrderNumber}")
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeContent(IContainer container, InvoiceResponseDto invoice)
        {
            container.PaddingVertical(20, Unit.Point).Column(column =>
            {
                column.Item().Element(c => ComposeCustomerDetails(c, invoice));
                column.Item().PaddingVertical(10).Element(c => ComposeMetaStrip(c, invoice));
                column.Item().Element(c => ComposeItemsTable(c, invoice));
                column.Item().PaddingTop(10).Element(c => ComposeSummary(c, invoice));

                if (!string.IsNullOrWhiteSpace(invoice.TermsAndConditions))
                    column.Item().PaddingTop(15).Element(c => ComposeTermsAndConditions(c, invoice));
            });
        }

        private void ComposeCustomerDetails(IContainer container, InvoiceResponseDto invoice)
        {
            container.Row(row =>
            {
                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(column =>
                {
                    column.Item().Text("Bill To:").Bold().FontSize(10).FontColor(Colors.Grey.Darken1);
                    column.Item().Text(invoice.PartyName).Bold().FontSize(11);

                    if (!string.IsNullOrWhiteSpace(invoice.PartyAddress))
                        column.Item().Text(invoice.PartyAddress).FontSize(9);

                    if (!string.IsNullOrWhiteSpace(invoice.PartyPhone))
                        column.Item().Text($"Phone: {invoice.PartyPhone}").FontSize(9);

                    if (!string.IsNullOrWhiteSpace(invoice.PartyEmail))
                        column.Item().Text($"Email: {invoice.PartyEmail}").FontSize(9);

                    if (!string.IsNullOrWhiteSpace(invoice.PartyGSTIN))
                        column.Item().Text($"GSTIN: {invoice.PartyGSTIN}").FontSize(9);
                });

                row.ConstantItem(190).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(column =>
                {
                    column.Item().Text("Payment Status:").Bold().FontSize(10).FontColor(Colors.Grey.Darken1);

                    if (invoice.ExchangeCreditApplied > 0)
                    {
                        column.Item().Text($"Exchange Credit: Rs. {invoice.ExchangeCreditApplied:N2}")
                            .FontSize(9).FontColor(Colors.Blue.Medium);
                        column.Item().Text($"Net Payable: Rs. {invoice.NetAmountPayable:N2}")
                            .FontSize(9);
                    }

                    column.Item().Text($"Total Paid: Rs. {invoice.TotalPaid:N2}")
                        .FontSize(9).FontColor(Colors.Green.Medium);

                    var balanceColor = invoice.BalanceDue > 0 ? Colors.Red.Medium : Colors.Green.Medium;
                    column.Item().Text($"Balance Due: Rs. {invoice.BalanceDue:N2}")
                        .FontSize(9).FontColor(balanceColor);
                });
            });
        }

        private void ComposeMetaStrip(IContainer container, InvoiceResponseDto invoice)
        {
            container.Row(row =>
            {
                row.RelativeItem().Background(Colors.Grey.Lighten3).Padding(8).Row(inner =>
                {
                    inner.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Transaction").FontSize(8).FontColor(Colors.Grey.Darken1);
                        c.Item().Text("Jewellery Sale").Bold().FontSize(10);
                    });

                    inner.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Pieces").FontSize(8).FontColor(Colors.Grey.Darken1);
                        c.Item().Text($"{invoice.TotalPieces ?? 0}").Bold().FontSize(10);
                    });

                    inner.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Net Weight").FontSize(8).FontColor(Colors.Grey.Darken1);
                        c.Item().Text($"{invoice.TotalGoldWeight ?? 0:N3} g").Bold().FontSize(10);
                    });

                    inner.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Invoice Total").FontSize(8).FontColor(Colors.Grey.Darken1);
                        c.Item().Text($"Rs. {invoice.GrandTotal:N2}").Bold().FontSize(10).FontColor(Colors.Blue.Medium);
                    });
                });
            });
        }

        private void ComposeItemsTable(IContainer container, InvoiceResponseDto invoice)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten2).Column(column =>
            {
                column.Item().Background(Colors.Grey.Lighten3).Padding(5).Row(row =>
                {
                    row.RelativeItem(2).Text("Item").Bold().FontSize(9);
                    row.ConstantItem(55).AlignCenter().Text("Qty").Bold().FontSize(9);
                    row.ConstantItem(70).AlignRight().Text("Net Wt").Bold().FontSize(9);
                    row.ConstantItem(85).AlignRight().Text("Metal Amt").Bold().FontSize(9);
                    row.ConstantItem(80).AlignRight().Text("Making").Bold().FontSize(9);
                    row.ConstantItem(70).AlignRight().Text("GST").Bold().FontSize(9);
                    row.ConstantItem(90).AlignRight().Text("Total").Bold().FontSize(9);
                });

                foreach (var item in invoice.Items)
                {
                    column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Row(row =>
                    {
                        row.RelativeItem(2).Column(c =>
                        {
                            c.Item().Text(item.ItemName).FontSize(9);

                            if (!string.IsNullOrWhiteSpace(item.MetalType) || !string.IsNullOrWhiteSpace(item.Purity))
                                c.Item().Text($"{item.MetalType ?? "Metal"} | {item.Purity ?? "Purity"}")
                                    .FontSize(7).FontColor(Colors.Grey.Darken1);

                            if (IsHallmarkedGoldItem(item))
                                c.Item().Text($"HUID: {item.HUID}").FontSize(7).FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(55).AlignCenter().Text($"{item.Quantity}").FontSize(9);
                        row.ConstantItem(70).AlignRight().Text($"{item.NetMetalWeight ?? 0:N3} g").FontSize(9);
                        row.ConstantItem(85).AlignRight().Text($"Rs. {item.MetalAmount ?? 0:N2}").FontSize(9);
                        row.ConstantItem(80).AlignRight().Text($"Rs. {item.MakingCharges ?? 0:N2}").FontSize(9);
                        row.ConstantItem(70).AlignRight().Text($"Rs. {item.TotalGSTAmount:N2}").FontSize(9);
                        row.ConstantItem(90).AlignRight().Text($"Rs. {item.TotalAmount:N2}").Bold().FontSize(9);
                    });
                }
            });
        }

        private void ComposeSummary(IContainer container, InvoiceResponseDto invoice)
        {
            container.AlignRight().Width(270).Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Sub Total:").FontSize(9);
                    row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.SubTotal:N2}").FontSize(9);
                });

                if (invoice.Items.Count > 0)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Making Charges:").FontSize(9);
                        row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.Items.Sum(item => item.MakingCharges ?? 0):N2}").FontSize(9);
                    });
                }

                if (invoice.DiscountAmount > 0)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Discount:").FontSize(9).FontColor(Colors.Green.Medium);
                        row.ConstantItem(115).AlignRight().Text($"-Rs. {invoice.DiscountAmount:N2}").FontSize(9).FontColor(Colors.Green.Medium);
                    });
                }

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("CGST:").FontSize(9);
                    row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.CGSTAmount:N2}").FontSize(9);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("SGST:").FontSize(9);
                    row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.SGSTAmount:N2}").FontSize(9);
                });

                if (invoice.IGSTAmount > 0)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("IGST:").FontSize(9);
                        row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.IGSTAmount:N2}").FontSize(9);
                    });
                }

                if (invoice.MakingChargesGSTAmount > 0)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Making GST:").FontSize(9);
                        row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.MakingChargesGSTAmount:N2}").FontSize(9);
                    });
                }

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Total GST:").FontSize(9);
                    row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.TotalGSTAmount:N2}").FontSize(9);
                });

                if (invoice.RoundOff != 0)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Round Off:").FontSize(9);
                        row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.RoundOff:N2}").FontSize(9);
                    });
                }

                column.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text("Grand Total:").Bold().FontSize(11);
                    row.ConstantItem(115).AlignRight().Text($"Rs. {invoice.GrandTotal:N2}").Bold().FontSize(11).FontColor(Colors.Blue.Medium);
                });

                if (!string.IsNullOrWhiteSpace(invoice.GrandTotalInWords))
                {
                    column.Item().PaddingTop(5).Text(invoice.GrandTotalInWords)
                        .FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                }
            });
        }

        private void ComposeTermsAndConditions(IContainer container, InvoiceResponseDto invoice)
        {
            container.Column(column =>
            {
                column.Item().Text("Terms & Conditions:").Bold().FontSize(9);
                column.Item().Text(invoice.TermsAndConditions ?? string.Empty).FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                text.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
            });
        }

        private static bool IsHallmarkedGoldItem(InvoiceItemDto item)
        {
            return item.IsHallmarked
                && !string.IsNullOrWhiteSpace(item.HUID)
                && (item.MetalType?.Contains("gold", StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}
