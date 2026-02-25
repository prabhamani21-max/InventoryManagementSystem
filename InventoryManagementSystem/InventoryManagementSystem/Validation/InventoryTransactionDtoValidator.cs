using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class InventoryTransactionDtoValidator : AbstractValidator<InventoryTransactionDto>
    {
        public InventoryTransactionDtoValidator()
        {
            RuleFor(x => x.JewelleryItemId).GreaterThan(0).WithMessage("JewelleryItemId must be greater than 0");
            RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("WarehouseId must be greater than 0");
            RuleFor(x => x.TransactionType).NotEmpty().Must(x => x == "IN" || x == "OUT" || x == "ADJUST").WithMessage("TransactionType must be 'IN', 'OUT', or 'ADJUST'");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
            RuleFor(x => x.ReferenceType).Must(x => string.IsNullOrEmpty(x) || x == "PURCHASE" || x == "SALE").WithMessage("ReferenceType must be 'PURCHASE' or 'SALE' if provided");
            RuleFor(x => x.TransactionDate).NotEmpty().WithMessage("TransactionDate is required");
            RuleFor(x => x.StatusId).GreaterThan(0).WithMessage("StatusId must be greater than 0");
        }
    }
}