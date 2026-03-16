using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class UserKycDtoValidator : AbstractValidator<UserKycDto>
    {
        public UserKycDtoValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId is required");

            RuleFor(x => x.PanCardNumber)
                .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$").WithMessage("PAN card number must be in format: AAAAA9999A (5 uppercase letters, 4 digits, 1 uppercase letter)")
                .When(x => !string.IsNullOrEmpty(x.PanCardNumber));

            RuleFor(x => x.AadhaarCardNumber)
                .Must(BeValidAadhaar).WithMessage("Aadhaar card number must be 12 digits (spaces and dashes are allowed)")
                .When(x => !string.IsNullOrEmpty(x.AadhaarCardNumber));

            RuleFor(x => x.StatusId).GreaterThan(0).WithMessage("StatusId is required");
        }

        private bool BeValidAadhaar(string aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar)) return false;

            // Remove spaces and dashes
            var cleaned = aadhaar.Replace(" ", "").Replace("-", "");

            // Must be exactly 12 digits
            return cleaned.Length == 12 && cleaned.All(char.IsDigit);
        }
    }
}