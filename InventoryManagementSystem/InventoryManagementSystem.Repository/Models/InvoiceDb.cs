using InventoryManagementSytem.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("invoice")]
    public class InvoiceDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [Column("invoice_date")]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [Column("invoice_type")]
        public TransactionType InvoiceType { get; set; }

        /* ---------- COMPANY SNAPSHOT ---------- */

        [Required]
        [Column("company_name")]
        public string CompanyName { get; set; } = string.Empty;

        [Column("company_address")]
        public string? CompanyAddress { get; set; }

        [Column("company_phone")]
        public string? CompanyPhone { get; set; }

        [Column("company_email")]
        public string? CompanyEmail { get; set; }

        [Column("company_gstin")]
        public string? CompanyGSTIN { get; set; }

        [Column("company_pan")]
        public string? CompanyPAN { get; set; }

        [Column("company_hallmark_license")]
        public string? CompanyHallmarkLicense { get; set; }

        /* ---------- PARTY SNAPSHOT ---------- */

        [Required]
        [Column("party_id")]
        public long PartyId { get; set; }

        [Required]
        [Column("party_type")]
        public PartyType PartyType { get; set; }

        [Required]
        [Column("party_name")]
        public string PartyName { get; set; } = string.Empty;

        [Column("party_address")]
        public string? PartyAddress { get; set; }

        [Column("party_phone")]
        public string? PartyPhone { get; set; }

        [Column("party_email")]
        public string? PartyEmail { get; set; }

        [Column("party_gstin")]
        public string? PartyGSTIN { get; set; }
        [Column("party_pan")]
        public string? PartyPANNUmber { get; set; }

        /* ---------- ORDER REFERENCES ---------- */

        [Column("sale_order_id")]
        public long? SaleOrderId { get; set; }

        [Column("purchase_order_id")]
        public long? PurchaseOrderId { get; set; }

        /* ---------- PRICING SUMMARY ---------- */

        [Column("sub_total")]
        public decimal SubTotal { get; set; }

        [Column("discount_amount")]
        public decimal DiscountAmount { get; set; }

        [Column("taxable_amount")]
        public decimal TaxableAmount { get; set; }

        [Column("cgst_amount")]
        public decimal CGSTAmount { get; set; }

        [Column("sgst_amount")]
        public decimal SGSTAmount { get; set; }

        [Column("igst_amount")]
        public decimal IGSTAmount { get; set; }

        [Column("total_gst_amount")]
        public decimal TotalGSTAmount { get; set; }

        [Column("round_off")]
        public decimal RoundOff { get; set; }

        [Column("grand_total")]
        public decimal GrandTotal { get; set; }

        [Column("grand_total_in_words")]
        public string? GrandTotalInWords { get; set; }

        /* ---------- PAYMENT SUMMARY ---------- */

        [Column("total_paid")]
        public decimal TotalPaid { get; set; }

        [Column("balance_due")]
        public decimal BalanceDue { get; set; }

        /* ---------- JEWELLERY SUMMARY ---------- */

        [Column("total_gold_weight")]
        public decimal? TotalGoldWeight { get; set; }

        [Column("total_stone_weight")]
        public decimal? TotalStoneWeight { get; set; }

        [Column("total_pieces")]
        public int? TotalPieces { get; set; }

        /* ---------- FOOTER ---------- */

        [Column("terms_and_conditions")]
        public string? TermsAndConditions { get; set; }

        [Column("return_policy")]
        public string? ReturnPolicy { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("declaration")]
        public string? Declaration { get; set; }

        /* ---------- AUDIT ---------- */

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("created_by")]
        public long CreatedBy { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Required]
        [Column("status_id")]
        public int StatusId { get; set; }

        /* ---------- E-INVOICE FIELDS (GST COMPLIANCE) ---------- */

        [Column("irn")]
        public string? IRN { get; set; } // Invoice Reference Number

        [Column("irn_generated_date")]
        public DateTime? IRNGeneratedDate { get; set; }

        [Column("qr_code")]
        public string? QRCode { get; set; } // Base64 QR code

        [Column("einvoice_status")]
        public string? EInvoiceStatus { get; set; } // Generated, Cancelled, Error

        [Column("einvoice_cancelled_date")]
        public DateTime? EInvoiceCancelledDate { get; set; }

        [Column("einvoice_cancel_reason")]
        public string? EInvoiceCancelReason { get; set; }

        [Column("acknowledgement_number")]
        public string? AcknowledgementNumber { get; set; }

        [Column("acknowledgement_date")]
        public DateTime? AcknowledgementDate { get; set; }

        /* ---------- NAVIGATION ---------- */

        public virtual ICollection<InvoiceItemDb> InvoiceItems { get; set; }
        public virtual ICollection<InvoicePaymentDb> InvoicePayments { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
        [ForeignKey(nameof(PurchaseOrderId))]

        public virtual PurchaseOrderDb Purchase { get; set; }
        [ForeignKey(nameof(SaleOrderId))]
        public virtual SaleOrderDb SaleOrder { get; set; }

    
    }
}
