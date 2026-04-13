using FluentValidation;
using System;

namespace SharedKernel.DTOs.Inventory
{
    public class UpdateMaterialRequest
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Shape { get; set; } = string.Empty;
        public decimal? Thickness { get; set; }
        public decimal? Width { get; set; }
        public decimal? StandardLength { get; set; }
        public string SoldBy { get; set; } = "Kg";
        public decimal GlobalRate { get; set; }
    }

    public class UpdateMaterialRequestValidator : AbstractValidator<UpdateMaterialRequest>
    {
        public UpdateMaterialRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Material ID is required for updates.");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Sub-Category must be selected.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Material Name is required.");
            RuleFor(x => x.Shape).NotEmpty().WithMessage("Shape must be defined.");
            
            // Allow 0 for thickness/width/length in case some materials are purely count based
            RuleFor(x => x.Thickness).GreaterThanOrEqualTo(0).WithMessage("Thickness cannot be negative.");
            RuleFor(x => x.Width).GreaterThanOrEqualTo(0).WithMessage("Width cannot be negative.");
            RuleFor(x => x.StandardLength).GreaterThanOrEqualTo(0).WithMessage("Length cannot be negative.");
            
            RuleFor(x => x.GlobalRate).GreaterThanOrEqualTo(0).WithMessage("Global Rate must be a positive value.");
        }
    }
}
