using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class WarehouseDtoValidator : AbstractValidator<WarehouseDto>
    {
        public WarehouseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.Address)
                .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.ManagerId)
                .GreaterThan(0).When(x => x.ManagerId.HasValue);

            RuleFor(x => x.StatusId).GreaterThan(0);
        }
    }
}