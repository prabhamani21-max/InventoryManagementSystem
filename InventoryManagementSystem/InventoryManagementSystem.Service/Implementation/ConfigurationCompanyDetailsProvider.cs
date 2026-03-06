using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Configuration;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// Configuration-based Company Details Provider
    /// Reads company details from appsettings.json (OCP compliance)
    /// </summary>
    public class ConfigurationCompanyDetailsProvider : ICompanyDetailsProvider
    {
        private readonly IConfiguration _configuration;

        public ConfigurationCompanyDetailsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CompanyDetails GetCompanyDetails()
        {
            var companySection = _configuration.GetSection("Company");
            return companySection.Get<CompanyDetails>() ?? new CompanyDetails();
        }

        public string GetTermsAndConditions()
        {
            return _configuration["Invoice:TermsAndConditions"] ?? string.Empty;
        }

        public string GetReturnPolicy()
        {
            return _configuration["Invoice:ReturnPolicy"] ?? string.Empty;
        }

        public string GetDeclaration()
        {
            return _configuration["Invoice:Declaration"] ?? string.Empty;
        }
    }
}