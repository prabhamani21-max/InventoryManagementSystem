using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class PurchaseOrderItemDtoValidator : AbstractValidator<PurchaseOrderItemDto>
    {
        public PurchaseOrderItemDtoValidator()
        {
            RuleFor(x => x.PurchaseOrderId)
                .GreaterThan(0).WithMessage("PurchaseOrderId must be greater than 0");

            RuleFor(x => x.JewelleryItemId)
                .GreaterThan(0).WithMessage("JewelleryItemId must be greater than 0");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("UnitPrice must be greater than 0");

            RuleFor(x => x.TotalPrice)
                .GreaterThan(0).WithMessage("TotalPrice must be greater than 0");

            RuleFor(x => x.StatusId)
                .GreaterThan(0).WithMessage("StatusId must be greater than 0");
        }
    }
}