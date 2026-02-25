using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class ItemStoneDtoValidator : AbstractValidator<ItemStoneDto>
    {
        public ItemStoneDtoValidator()
        {
            RuleFor(x => x.JewelleryItemId).GreaterThan(0).WithMessage("ItemId must be greater than 0");
            RuleFor(x => x.StoneId).GreaterThan(0).WithMessage("StoneId must be greater than 0");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
            RuleFor(x => x.Weight).GreaterThan(0).WithMessage("Weight must be greater than 0");
            RuleFor(x => x.StatusId).GreaterThan(0).WithMessage("StatusId must be greater than 0");
        }
    }
} 