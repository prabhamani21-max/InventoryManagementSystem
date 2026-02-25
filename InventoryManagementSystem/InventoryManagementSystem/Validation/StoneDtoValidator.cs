using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class StoneDtoValidator : AbstractValidator<StoneDto>
    {
        public StoneDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2)
                .MaximumLength(100);

            RuleFor(x => x.Unit)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Unit));

            RuleFor(x => x.StatusId).GreaterThan(0);
        }
    }
}