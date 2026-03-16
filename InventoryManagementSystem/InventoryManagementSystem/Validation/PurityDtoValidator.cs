using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class PurityDtoValidator : AbstractValidator<PurityDto>
    {
        public PurityDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100);

            RuleFor(x => x.Percentage)
                .GreaterThan(0).WithMessage("Percentage must be greater than 0");

            RuleFor(x => x.MetalId).GreaterThan(0);
            RuleFor(x => x.StatusId).GreaterThan(0);
        }
    }
}