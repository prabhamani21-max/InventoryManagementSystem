using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class JewelleryItemDtoValidator : AbstractValidator<JewelleryItemDto>
    {
        public JewelleryItemDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).When(x => x.Id > 0).WithMessage("Id must be greater than 0 when provided");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0");

            RuleFor(x => x.StatusId)
                .GreaterThan(0).WithMessage("StatusId must be greater than 0");

            // Metal validation rules
            RuleFor(x => x.MetalId)
                .GreaterThan(0).WithMessage("MetalId must be greater than 0");

            RuleFor(x => x.PurityId)
                .GreaterThan(0).WithMessage("PurityId must be greater than 0");

            RuleFor(x => x.GrossWeight)
                .GreaterThan(0).WithMessage("GrossWeight must be greater than 0");

            RuleFor(x => x.NetMetalWeight)
                .GreaterThan(0).WithMessage("NetWeight must be greater than 0");

            RuleFor(x => x.NetMetalWeight)
                .LessThanOrEqualTo(x => x.GrossWeight).WithMessage("NetWeight cannot be greater than GrossWeight");

            // Hallmark validation rules
            RuleFor(x => x.HUID)
                .MaximumLength(6).WithMessage("HUID cannot exceed 6 characters")
                .Matches("^[A-Za-z0-9]{6}$").When(x => !string.IsNullOrEmpty(x.HUID)).WithMessage("HUID must be exactly 6 alphanumeric characters");

            RuleFor(x => x.BISCertificationNumber)
                .MaximumLength(50).WithMessage("BIS Certification Number cannot exceed 50 characters");

            RuleFor(x => x.HallmarkCenterName)
                .MaximumLength(100).WithMessage("Hallmark Center Name cannot exceed 100 characters");

            RuleFor(x => x.HallmarkDate)
                .LessThanOrEqualTo(DateTime.Today).When(x => x.HallmarkDate.HasValue).WithMessage("Hallmark Date cannot be in the future");
        }
    }

    // public class JewelleryItemCreateDtoValidator : AbstractValidator<JewelleryItemCreateDto>
    // {
    //     public JewelleryItemCreateDtoValidator()
    //     {
    //         RuleFor(x => x.Name)
    //             .NotEmpty().WithMessage("Name is required")
    //             .MaximumLength(200);

    //         RuleFor(x => x.CategoryId)
    //             .GreaterThan(0).WithMessage("CategoryId must be greater than 0");

    //         RuleFor(x => x.StoneId)
    //             .GreaterThan(0).When(x => x.HasStone).WithMessage("StoneId must be greater than 0 when HasStone is true");

    //         // Metal validation rules - required for create
    //         RuleFor(x => x.MetalId)
    //             .GreaterThan(0).WithMessage("MetalId is required and must be greater than 0");

    //         RuleFor(x => x.PurityId)
    //             .GreaterThan(0).WithMessage("PurityId is required and must be greater than 0");

    //         RuleFor(x => x.GrossWeight)
    //             .GreaterThan(0).WithMessage("GrossWeight is required and must be greater than 0");

    //         RuleFor(x => x.NetWeight)
    //             .GreaterThan(0).WithMessage("NetWeight is required and must be greater than 0");

    //         RuleFor(x => x.NetWeight)
    //             .LessThanOrEqualTo(x => x.GrossWeight).WithMessage("NetWeight cannot be greater than GrossWeight");
    //     }
    // }

    // public class JewelleryItemUpdateDtoValidator : AbstractValidator<JewelleryItemUpdateDto>
    // {
    //     public JewelleryItemUpdateDtoValidator()
    //     {
    //         RuleFor(x => x.Id)
    //             .GreaterThan(0).WithMessage("Id must be greater than 0");

    //         RuleFor(x => x.Name)
    //             .NotEmpty().WithMessage("Name is required")
    //             .MaximumLength(200);

    //         RuleFor(x => x.CategoryId)
    //             .GreaterThan(0).WithMessage("CategoryId must be greater than 0");

    //         RuleFor(x => x.StoneId)
    //             .GreaterThan(0).When(x => x.HasStone).WithMessage("StoneId must be greater than 0 when HasStone is true");

    //         // Metal validation rules
    //         RuleFor(x => x.MetalId)
    //             .GreaterThan(0).WithMessage("MetalId must be greater than 0");

    //         RuleFor(x => x.PurityId)
    //             .GreaterThan(0).WithMessage("PurityId must be greater than 0");

    //         RuleFor(x => x.GrossWeight)
    //             .GreaterThan(0).WithMessage("GrossWeight must be greater than 0");

    //         RuleFor(x => x.NetWeight)
    //             .GreaterThan(0).WithMessage("NetWeight must be greater than 0");

    //         RuleFor(x => x.NetWeight)
    //             .LessThanOrEqualTo(x => x.GrossWeight).WithMessage("NetWeight cannot be greater than GrossWeight");
    //     }
    // }
}
