using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class SaleOrderDtoValidator : AbstractValidator<SaleOrderDto>
    {
        public SaleOrderDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("CustomerId must be greater than 0");

            // OrderNumber is optional - will be auto-generated if not provided


            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("OrderDate is required");

            // StatusId is optional for create - will be set to Active by default
        }
    }
}