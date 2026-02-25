using InventoryManagementSytem.Common.Dtos;

namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// E-Invoice Service Interface for GST Compliance
    /// Handles IRN generation, QR code generation, and e-invoice cancellation with NIC portal
    /// </summary>
    public interface IEInvoiceService
    {
        /// <summary>
        /// Generate IRN (Invoice Reference Number) from NIC portal
        /// </summary>
        /// <param name="invoiceId">Invoice ID to generate IRN for</param>
        /// <returns>E-Invoice response with IRN and QR code</returns>
        Task<EInvoiceResponseDto> GenerateIRNAsync(long invoiceId);

        /// <summary>
        /// Cancel e-invoice on NIC portal
        /// </summary>
        /// <param name="invoiceId">Invoice ID to cancel</param>
        /// <param name="cancelReason">Reason for cancellation</param>
        /// <returns>True if cancelled successfully</returns>
        Task<bool> CancelEInvoiceAsync(long invoiceId, string cancelReason);

        /// <summary>
        /// Get IRN details from NIC portal
        /// </summary>
        /// <param name="irn">Invoice Reference Number</param>
        /// <returns>E-Invoice details</returns>
        Task<EInvoiceResponseDto?> GetIRNDetailsAsync(string irn);

        /// <summary>
        /// Generate QR code for invoice
        /// </summary>
        /// <param name="invoiceId">Invoice ID to generate QR code for</param>
        /// <returns>Base64 encoded QR code</returns>
        Task<string> GenerateQRCodeAsync(long invoiceId);

        /// <summary>
        /// Check if invoice is eligible for e-invoicing
        /// </summary>
        /// <param name="invoiceId">Invoice ID to check</param>
        /// <returns>True if eligible for e-invoicing</returns>
        Task<bool> IsEligibleForEInvoicingAsync(long invoiceId);

        /// <summary>
        /// Sync invoice with NIC portal (regenerate if needed)
        /// </summary>
        /// <param name="invoiceId">Invoice ID to sync</param>
        /// <returns>E-Invoice response</returns>
        Task<EInvoiceResponseDto> SyncWithNICPortalAsync(long invoiceId);
    }

    /// <summary>
    /// E-Invoice Response DTO
    /// </summary>
    public class EInvoiceResponseDto
    {
        /// <summary>
        /// Invoice Reference Number (64 character unique ID)
        /// </summary>
        public string? IRN { get; set; }

        /// <summary>
        /// Acknowledgement Number
        /// </summary>
        public string? AcknowledgementNumber { get; set; }

        /// <summary>
        /// Acknowledgement Date
        /// </summary>
        public DateTime? AcknowledgementDate { get; set; }

        /// <summary>
        /// Base64 encoded QR code
        /// </summary>
        public string? QRCode { get; set; }

        /// <summary>
        /// Signed invoice data
        /// </summary>
        public string? SignedInvoice { get; set; }

        /// <summary>
        /// Status of e-invoice (Generated, Cancelled, Error)
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Error message if any
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Error code if any
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Date when IRN was generated
        /// </summary>
        public DateTime? IRNGeneratedDate { get; set; }

        /// <summary>
        /// Date when e-invoice was cancelled (if applicable)
        /// </summary>
        public DateTime? CancelledDate { get; set; }

        /// <summary>
        /// Reason for cancellation (if applicable)
        /// </summary>
        public string? CancelReason { get; set; }
    }

    /// <summary>
    /// E-Invoice Request DTO for NIC portal
    /// </summary>
    public class EInvoiceRequestDto
    {
        public string Version { get; set; } = "1.1";
        public EInvoiceTransactionDetails TransactionDetails { get; set; } = new();
        public EInvoiceDocumentDetails DocumentDetails { get; set; } = new();
        public EInvoiceSellerDetails SellerDetails { get; set; } = new();
        public EInvoiceBuyerDetails? BuyerDetails { get; set; }
        public List<EInvoiceItemDetails> ItemList { get; set; } = new();
        public EInvoiceValueDetails ValueDetails { get; set; } = new();
        public EInvoicePaymentDetails? PaymentDetails { get; set; }
    }

    /// <summary>
    /// Transaction details for e-invoice
    /// </summary>
    public class EInvoiceTransactionDetails
    {
        public string TaxScheme { get; set; } = "GST";
        public string SupplyType { get; set; } = "B2C"; // B2B, B2C, SEZ, EXP
        public string? IGSTOnIntra { get; set; }
    }

    /// <summary>
    /// Document details for e-invoice
    /// </summary>
    public class EInvoiceDocumentDetails
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string InvoiceType { get; set; } = "INV"; // INV, CRN, DBN
    }

    /// <summary>
    /// Seller details for e-invoice
    /// </summary>
    public class EInvoiceSellerDetails
    {
        public string GSTIN { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string? TradeName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    /// <summary>
    /// Buyer details for e-invoice
    /// </summary>
    public class EInvoiceBuyerDetails
    {
        public string? GSTIN { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public string? TradeName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    /// <summary>
    /// Item details for e-invoice
    /// </summary>
    public class EInvoiceItemDetails
    {
        public int SerialNumber { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? HSNCode { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; } = "NOS";
        public decimal UnitPrice { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal CGSTRate { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTRate { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTRate { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// Value details for e-invoice
    /// </summary>
    public class EInvoiceValueDetails
    {
        public decimal TotalInvoiceValue { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal CESSAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal RoundOff { get; set; }
    }

    /// <summary>
    /// Payment details for e-invoice
    /// </summary>
    public class EInvoicePaymentDetails
    {
        public string? PaymentMode { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? BalanceDue { get; set; }
    }

    /// <summary>
    /// E-Invoice cancellation request DTO
    /// </summary>
    public class EInvoiceCancelRequestDto
    {
        public string IRN { get; set; } = string.Empty;
        public string CancelReason { get; set; } = string.Empty;
        public string? CancelRemark { get; set; }
    }
}
