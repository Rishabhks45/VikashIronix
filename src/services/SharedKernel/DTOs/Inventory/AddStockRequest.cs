using FluentValidation;
using System;

namespace SharedKernel.DTOs.Inventory
{
    public class AddStockRequest
    {
        public Guid MaterialId { get; set; }
        public decimal AddedQuantity { get; set; }
        public decimal PurchaseRate { get; set; }
    }

    public class AddStockRequestValidator : AbstractValidator<AddStockRequest>
    {
        public AddStockRequestValidator()
        {
            RuleFor(x => x.MaterialId).NotEmpty().WithMessage("Material ID is required.");
            RuleFor(x => x.AddedQuantity).GreaterThan(0).WithMessage("Added quantity must be greater than zero.");
            RuleFor(x => x.PurchaseRate).GreaterThan(0).WithMessage("Purchase rate must be a positive value.");
        }
    }
}
