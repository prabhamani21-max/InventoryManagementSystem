using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class ItemStockDtoValidator : AbstractValidator<ItemStockDto>
    {
        public ItemStockDtoValidator()
        {
            RuleFor(x => x.JewelleryItemId).GreaterThan(0).WithMessage("JewelleryItemId must be greater than 0");
            RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("WarehouseId must be greater than 0");
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater than or equal to 0");
            RuleFor(x => x.ReservedQuantity).GreaterThanOrEqualTo(0).WithMessage("ReservedQuantity must be greater than or equal to 0");
            RuleFor(x => x.StatusId).GreaterThan(0).WithMessage("StatusId must be greater than 0");

            RuleFor(x => x.ReservedQuantity).LessThanOrEqualTo(x => x.Quantity).WithMessage("ReservedQuantity cannot be greater than Quantity");
        }
    }
}