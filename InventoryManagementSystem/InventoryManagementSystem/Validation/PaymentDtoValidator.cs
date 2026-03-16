using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class PaymentDtoValidator : AbstractValidator<PaymentDto>
    {
        public PaymentDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("OrderId must be greater than 0");

            RuleFor(x => x.OrderType)
                .NotEmpty().WithMessage("OrderType is required")
                .Must(x => x == "PURCHASE" || x == "SALE").WithMessage("OrderType must be 'PURCHASE' or 'SALE'");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("PaymentMethod is required");

            RuleFor(x => x.PaymentDate)
                .NotEmpty().WithMessage("PaymentDate is required");

            RuleFor(x => x.StatusId)
                .GreaterThan(0).WithMessage("StatusId must be greater than 0");
        }
    }
}