using FluentValidation;
using InventoryManagementSystem.Dto;

namespace InventoryManagementSystem.Validation
{
    public class StatusDtoValidator : AbstractValidator<StatusDto>
    {
        public StatusDtoValidator()
        {
            RuleFor(us => us.Name)
                .NotEmpty().WithMessage("User status name is required.")
                .MaximumLength(50).WithMessage("User status name cannot exceed 50 characters.");
        }
    }
}