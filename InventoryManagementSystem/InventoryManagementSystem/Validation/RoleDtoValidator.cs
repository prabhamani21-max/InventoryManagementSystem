using FluentValidation;
using InventoryManagementSystem.Dto;

namespace InventoryManagementSystem.Validation
{
    public class RoleDtoValidator : AbstractValidator<RoleDto>
    {
        public RoleDtoValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(2).WithMessage("Role name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.");

            RuleFor(r => r.StatusId)
                .NotNull().WithMessage("Status is required.")
                .GreaterThan(0).WithMessage("Status cannot be a negative number or zero.")
                .InclusiveBetween(1, 2).WithMessage("Status must be either 1 (Active) or 2 (Inactive).");
        }
    }
}
