using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class SupplierDtoValidator : AbstractValidator<SupplierDto>
    {
        public SupplierDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.ContactPerson)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.ContactPerson));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .Matches(@"^\d{10,15}$").WithMessage("Invalid phone number")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Address)
                .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.GSTNumber)
                .MaximumLength(15).When(x => !string.IsNullOrEmpty(x.GSTNumber));
            RuleFor(x=>x.TANNumber).MaximumLength(15).When(x => !string.IsNullOrEmpty(x.TANNumber));

            RuleFor(x => x.StatusId).GreaterThan(0);
        }
    }
}