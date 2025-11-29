using FluentValidation;
using Products.Application.CQRS.Commands;

namespace Products.Application.Validators
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            When(x => x.Dto.Name != null, () =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty().WithMessage("Product name cannot be empty.")
                    .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");
            });

            When(x => x.Dto.Description != null, () =>
            {
                RuleFor(x => x.Dto.Description)
                    .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
            });
        }
    }
}