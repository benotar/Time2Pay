using FluentValidation;
using Time2Pay.DAL;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Validators.Employments;

public abstract class EmploymentDtoValidatorBase<TDto> : AbstractValidator<TDto>
    where TDto : IEmploymentDto
{
    protected EmploymentDtoValidatorBase()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .MinimumLength(EntityLimits.NameMinLength)
            .MaximumLength(EntityLimits.NameMaxLength);

        RuleFor(dto => dto.Description)
            .MaximumLength(EntityLimits.DescriptionMaxLength);

        RuleFor(dto => dto.StartDate)
            .NotEmpty();

        When(dto => dto.EndDate is not null, () =>
        {
            RuleFor(dto => dto.EndDate)
                .GreaterThanOrEqualTo(dto => dto.StartDate);
        });
    }
}
