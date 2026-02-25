using FluentValidation;
using InventoryManagementSystem.DTO;

namespace InventoryManagementSystem.Validation
{
    public class SaleOrderItemDtoValidator : AbstractValidator<SaleOrderItemDto>
    {
        public SaleOrderItemDtoValidator()
        {
            /* ---------------- PARENT ---------------- */

            RuleFor(x => x.SaleOrderId)
                .GreaterThan(0).WithMessage("SaleOrderId must be greater than 0");

            RuleFor(x => x.JewelleryItemId)
                .GreaterThan(0).WithMessage("JewelleryItemId must be greater than 0");

            /* ---------------- ITEM SNAPSHOT ---------------- */

            RuleFor(x => x.ItemName)
                .NotEmpty().WithMessage("ItemName is required");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            /* ---------------- METAL SNAPSHOT ---------------- */

            RuleFor(x => x.MetalId)
                .GreaterThan(0).WithMessage("MetalId must be greater than 0");

            RuleFor(x => x.PurityId)
                .GreaterThan(0).WithMessage("PurityId must be greater than 0");

            RuleFor(x => x.GrossWeight)
                .GreaterThan(0).WithMessage("GrossWeight must be greater than 0");

            RuleFor(x => x.NetMetalWeight)
                .GreaterThan(0).WithMessage("NetMetalWeight must be greater than 0");

            RuleFor(x => x.MetalRatePerGram)
                .GreaterThanOrEqualTo(0).WithMessage("MetalRatePerGram must be greater than or equal to 0");

            RuleFor(x => x.MetalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("MetalAmount must be greater than or equal to 0");

            /* ---------------- MAKING CHARGES ---------------- */

            RuleFor(x => x.MakingChargeValue)
                .GreaterThanOrEqualTo(0).WithMessage("MakingChargeValue must be greater than or equal to 0");

            RuleFor(x => x.TotalMakingCharges)
                .GreaterThanOrEqualTo(0).WithMessage("TotalMakingCharges must be greater than or equal to 0");

            RuleFor(x => x.WastagePercentage)
                .GreaterThanOrEqualTo(0).WithMessage("WastagePercentage must be greater than or equal to 0");

            RuleFor(x => x.WastageWeight)
                .GreaterThanOrEqualTo(0).WithMessage("WastageWeight must be greater than or equal to 0");

            RuleFor(x => x.WastageAmount)
                .GreaterThanOrEqualTo(0).WithMessage("WastageAmount must be greater than or equal to 0");

            /* ---------------- STONE SUMMARY ---------------- */

            RuleFor(x => x.StoneAmount)
                .GreaterThanOrEqualTo(0).When(x => x.StoneAmount.HasValue).WithMessage("StoneAmount must be greater than or equal to 0");

            /* ---------------- PRICE & TAX ---------------- */

            RuleFor(x => x.ItemSubtotal)
                .GreaterThanOrEqualTo(0).WithMessage("ItemSubtotal must be greater than or equal to 0");

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("DiscountAmount must be greater than or equal to 0");

            RuleFor(x => x.TaxableAmount)
                .GreaterThanOrEqualTo(0).WithMessage("TaxableAmount must be greater than or equal to 0");

            RuleFor(x => x.GstPercentage)
                .GreaterThanOrEqualTo(0).WithMessage("GstPercentage must be greater than or equal to 0");

            RuleFor(x => x.GstAmount)
                .GreaterThanOrEqualTo(0).WithMessage("GstAmount must be greater than or equal to 0");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("TotalAmount must be greater than or equal to 0");

            /* ---------------- HALLMARK ---------------- */

            RuleFor(x => x.HUID)
                .MaximumLength(6).WithMessage("HUID cannot exceed 6 characters")
                .Matches("^[A-Za-z0-9]{6}$").When(x => !string.IsNullOrEmpty(x.HUID)).WithMessage("HUID must be exactly 6 alphanumeric characters");

            RuleFor(x => x.BISCertificationNumber)
                .MaximumLength(50).WithMessage("BIS Certification Number cannot exceed 50 characters");

            RuleFor(x => x.HallmarkCenterName)
                .MaximumLength(100).WithMessage("Hallmark Center Name cannot exceed 100 characters");

            RuleFor(x => x.HallmarkDate)
                .LessThanOrEqualTo(DateTime.Today).When(x => x.HallmarkDate.HasValue).WithMessage("Hallmark Date cannot be in the future");

            /* ---------------- AUDIT ---------------- */

            RuleFor(x => x.StatusId)
                .GreaterThan(0).WithMessage("StatusId must be greater than 0");
        }
    }
}
