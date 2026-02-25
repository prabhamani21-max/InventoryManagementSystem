using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSytem.Common.Enums;

namespace InventoryManagementSystem.Common.Models
{
    /// <summary>
    /// Invoice domain model for jewellery store invoices
    /// </summary>
    public class Invoice
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string? InvoiceType { get; set; }

        // Company Details
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyAddress { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyGSTIN { get; set; }
        public string? CompanyPAN { get; set; }
        public string? CompanyHallmarkLicense { get; set; }

        // Customer Details
        public long PartyId { get; set; }
        public PartyType PartyType { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string? PartyAddress { get; set; }
        public string? PartyPhone { get; set; }
        public string? PartyEmail { get; set; }
        public string? PartyGSTIN { get; set; }
        public string? PartyPANNUmber { get; set; }

        // Sale Order Reference
        public long? SaleOrderId { get; set; }
        public long? PurchaseOrderId { get; set; }

        // Pricing Summary
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal TotalGSTAmount { get; set; }
        public decimal RoundOff { get; set; }
        public decimal GrandTotal { get; set; }
        public string GrandTotalInWords { get; set; } = string.Empty;

        // Payment Details
        public decimal TotalPaid { get; set; }
        public decimal BalanceDue { get; set; }

        // Jewellery-Specific Details
        public decimal? TotalGoldWeight { get; set; }
        public decimal? TotalStoneWeight { get; set; }
        public int? TotalPieces { get; set; }

        // Hallmark Details
        public bool IsHallmarked { get; set; }
        public string? HallmarkLogo { get; set; }
        public string? BISHallmarkNumber { get; set; }

        // Footer
        public bool IncludeTermsAndConditions { get; set; } = true;
        public string? TermsAndConditions { get; set; }
        public string? ReturnPolicy { get; set; }
        public string? Notes { get; set; }
        public string? Declaration { get; set; }

        // Audit fields
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }

        // E-Invoice Fields (GST Compliance)
        public string? IRN { get; set; } // Invoice Reference Number
        public DateTime? IRNGeneratedDate { get; set; }
        public string? QRCode { get; set; } // Base64 QR code
        public string? EInvoiceStatus { get; set; } // Generated, Cancelled, Error
        public DateTime? EInvoiceCancelledDate { get; set; }
        public string? EInvoiceCancelReason { get; set; }
        public string? AcknowledgementNumber { get; set; }
        public DateTime? AcknowledgementDate { get; set; }

        // Navigation properties for creation
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public ICollection<InvoicePayment> InvoicePayments { get; set; } = new List<InvoicePayment>();
    }

    /// <summary>
    /// Individual jewellery item in the invoice
    /// </summary>
    public class InvoiceItem
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public long? ReferenceItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // Metal Details
        public int MetalId { get; set; }
        public int PurityId { get; set; }
        public decimal? NetMetalWeight { get; set; }
        public decimal? MetalAmount { get; set; }

        // Stone Details (SNAPSHOT at billing time - never depend on JewelleryItem later)
        public int? StoneId { get; set; }
        public decimal? StoneWeight { get; set; }
        public decimal? StoneRate { get; set; }
        public decimal? StoneAmount { get; set; }

        // Making Charges
        public decimal? MakingCharges { get; set; }
        public decimal? WastageAmount { get; set; }

        // Pricing
        public decimal ItemSubtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxableAmount { get; set; }

        // GST Breakdown (required for proper GST calculation - derived from SaleOrderItem)
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Hallmark snapshot
        public bool IsHallmarked { get; set; }
    }

    /// <summary>
    /// Payment details for the invoice
    /// </summary>
    public class InvoicePayment
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public long PaymentId { get; set; }
        public decimal AllocatedAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
    }
}
