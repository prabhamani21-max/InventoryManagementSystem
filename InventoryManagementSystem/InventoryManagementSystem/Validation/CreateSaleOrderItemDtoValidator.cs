using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class CreateSaleOrderItemDtoValidator : AbstractValidator<DTO.CreateSaleOrderItemDto>
    {
        public CreateSaleOrderItemDtoValidator()
        {
            // Required fields validation
            RuleFor(x => x.SaleOrderId)
                .GreaterThan(0).WithMessage("SaleOrderId is required");

            RuleFor(x => x.JewelleryItemId)
                .GreaterThan(0).WithMessage("JewelleryItemId is required");

            // Quantity validation
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1")
                .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000");

            // Optional overrides validation
            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0).When(x => x.DiscountAmount.HasValue)
                .WithMessage("DiscountAmount cannot be negative");

            RuleFor(x => x.GstPercentage)
                .GreaterThanOrEqualTo(0).WithMessage("GST percentage cannot be negative")
                .LessThanOrEqualTo(100).WithMessage("GST percentage cannot exceed 100");

            RuleFor(x => x.StoneAmount)
                .GreaterThanOrEqualTo(0).When(x => x.StoneAmount.HasValue)
                .WithMessage("StoneAmount cannot be negative");
        }
    }
}
