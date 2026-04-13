using System;
using FluentValidation;

namespace SharedKernel.DTOs.Inventory
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string? HsnCode { get; set; }
        public bool IsRoot { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string? HsnCode { get; set; }
        public bool IsRoot { get; set; }
    }

    public class UpdateCategoryRequest : CreateCategoryRequest
    {
        public Guid Id { get; set; }
    }

    public class CreateCategoryRequestValidator : FluentValidation.AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Category Name is required.")
                .MaximumLength(150).WithMessage("Category Name must be 150 characters or fewer.");
                
            RuleFor(x => x.HsnCode).MaximumLength(20).WithMessage("HSN Code must be 20 characters or fewer.");
            
            RuleFor(x => x.ParentId).NotEmpty().When(x => !x.IsRoot)
                .WithMessage("A Parent Category must be selected for Sub-Categories.");
        }
    }

    public class UpdateCategoryRequestValidator : FluentValidation.AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Category ID is required for updates.");
            
            RuleFor(x => x.Name).NotEmpty().WithMessage("Category Name is required.")
                .MaximumLength(150).WithMessage("Category Name must be 150 characters or fewer.");
                
            RuleFor(x => x.HsnCode).MaximumLength(20).WithMessage("HSN Code must be 20 characters or fewer.");
            
            RuleFor(x => x.ParentId).NotEmpty().When(x => !x.IsRoot)
                .WithMessage("A Parent Category must be selected for Sub-Categories.");
        }
    }
}
