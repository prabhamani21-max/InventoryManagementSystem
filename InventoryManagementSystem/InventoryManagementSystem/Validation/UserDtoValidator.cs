using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
                .Matches(@"[!@#$%^&*(),.?|<>]").WithMessage("Password must contain at least one special character !@#$%^&*(),.?|<>");

            RuleFor(x => x.RoleId).GreaterThan(0);
            RuleFor(x => x.StatusId).GreaterThan(0);
            RuleFor(x => x.Gender).InclusiveBetween(0, 3); // assuming 1=Male, 0=Female, 2=Other
            RuleFor(x => x.DOB);
            RuleFor(x => x.ContactNumber)
                .NotEmpty().WithMessage("Contact number is required")
                .Matches(@"^\d{10,15}$").WithMessage("Invalid contact number");
        }
    }
}
