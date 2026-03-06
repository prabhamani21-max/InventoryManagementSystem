namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Company Details Provider Interface
    /// Provides company configuration details (OCP compliance - configuration externalized)
    /// </summary>
    public interface ICompanyDetailsProvider
    {
        /// <summary>
        /// Get company details for invoice
        /// </summary>
        CompanyDetails GetCompanyDetails();

        /// <summary>
        /// Get terms and conditions for invoice
        /// </summary>
        string GetTermsAndConditions();

        /// <summary>
        /// Get return policy for invoice
        /// </summary>
        string GetReturnPolicy();

        /// <summary>
        /// Get declaration text for invoice
        /// </summary>
        string GetDeclaration();
    }

    /// <summary>
    /// Company details model for configuration
    /// </summary>
    public class CompanyDetails
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string GSTIN { get; set; } = string.Empty;
        public string PAN { get; set; } = string.Empty;
        public string HallmarkLicense { get; set; } = string.Empty;
    }
}