using FluentValidation;
using Time2Pay.DAL;
using Time2Pay.DTO.Employments;
using Time2Pay.Shared;

namespace Time2Pay.BLL.Validators.Employments;

public abstract class EmploymentModelValidatorBase<TModel> : AbstractValidator<TModel>
    where TModel : IEmploymentModel
{
    protected EmploymentModelValidatorBase()
    {
        RuleFor(model => model.Name)
            .NotEmpty()
            .MinimumLength(Constants.NameMinLength)
            .MaximumLength(Constants.NameMaxLength);

        RuleFor(model => model.Description)
            .MaximumLength(Constants.DescriptionMaxLength);

        RuleFor(dto => dto.StartDate)
            .NotEmpty();

        RuleFor(dto => dto.EndDate)
            .GreaterThanOrEqualTo(dto => dto.StartDate);
    }
}
