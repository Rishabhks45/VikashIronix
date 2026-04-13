using FluentValidation;
using System;

namespace SharedKernel.DTOs.Inventory
{
    public class MaterialDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Shape { get; set; } = string.Empty;
        public decimal? Thickness { get; set; }
        public decimal? Width { get; set; }
        public decimal? StandardLength { get; set; }
        public string SoldBy { get; set; } = string.Empty;
        public decimal GlobalRate { get; set; }
        public decimal LastPurchaseRate { get; set; }
        public decimal StockMT { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CreateMaterialRequest
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Shape { get; set; } = string.Empty;
        public decimal? Thickness { get; set; }
        public decimal? Width { get; set; }
        public decimal? StandardLength { get; set; }
        public string SoldBy { get; set; } = string.Empty;
        public decimal GlobalRate { get; set; }
    }

    public class CreateMaterialRequestValidator : FluentValidation.AbstractValidator<CreateMaterialRequest>
    {
        public CreateMaterialRequestValidator()
        {
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Sub-Category must be selected.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Material Name is required.");
            RuleFor(x => x.Shape).NotEmpty().WithMessage("Shape must be defined.");
            
            RuleFor(x => x.Thickness).GreaterThanOrEqualTo(0).When(x => x.Thickness.HasValue).WithMessage("Thickness cannot be negative.");
            RuleFor(x => x.Width).GreaterThanOrEqualTo(0).When(x => x.Width.HasValue).WithMessage("Width cannot be negative.");
            RuleFor(x => x.StandardLength).GreaterThanOrEqualTo(0).When(x => x.StandardLength.HasValue).WithMessage("Length cannot be negative.");
            
            RuleFor(x => x.GlobalRate).GreaterThanOrEqualTo(0).WithMessage("Global Rate must be a positive value.");
        }
    }
}
