using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class PurchaseOrderDtoValidator : AbstractValidator<PurchaseOrderDto>
    {
        public PurchaseOrderDtoValidator()
        {
            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("SupplierId must be greater than 0");

            RuleFor(x => x.OrderNumber)
                .NotEmpty().WithMessage("OrderNumber is required")
                .MaximumLength(50);

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("OrderDate is required");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("TotalAmount must be non-negative");

            RuleFor(x => x.StatusId)
                .GreaterThan(0).WithMessage("StatusId must be greater than 0");
        }
    }
}