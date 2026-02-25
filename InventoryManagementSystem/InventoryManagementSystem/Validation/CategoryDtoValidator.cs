using FluentValidation;
using InventoryManagementSytem.Common.Dtos;

namespace InventoryManagementSystem.Validation
{
    /// <summary>
    /// Validator for CategoryCreateDto
    /// </summary>
    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-&']+$").WithMessage("Name can only contain letters, numbers, spaces, hyphens, ampersands, and apostrophes");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }

    /// <summary>
    /// Validator for CategoryUpdateDto
    /// </summary>
    public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
    {
        public CategoryUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Category ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-&']+$").WithMessage("Name can only contain letters, numbers, spaces, hyphens, ampersands, and apostrophes");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.ParentId)
                .GreaterThan(0).When(x => x.ParentId.HasValue).WithMessage("Parent ID must be greater than 0");

            // Prevent circular reference (category cannot be its own parent)
            RuleFor(x => x)
                .Must(x => x.Id != x.ParentId).WithMessage("Category cannot be its own parent")
                .When(x => x.Id > 0 && x.ParentId.HasValue);
        }
    }

    /// <summary>
    /// Validator for CategoryDto (used for general validation)
    /// </summary>
    public class CategoryDtoValidator : AbstractValidator<CategoryDto>
    {
        public CategoryDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).When(x => x.Id > 0).WithMessage("Invalid category ID");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
