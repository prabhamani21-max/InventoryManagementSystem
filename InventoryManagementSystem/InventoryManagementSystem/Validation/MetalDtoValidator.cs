using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class MetalDtoValidator : AbstractValidator<MetalDto>
    {
        public MetalDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100);

            RuleFor(x => x.StatusId).GreaterThan(0);
        }
    }
}