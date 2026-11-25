using FluentValidation;
using Time2Pay.DTO.CompensationRates;

namespace Time2Pay.BLL.Validators.CompensationRates;

public class CreateCompensationRateValidator : AbstractValidator<CreateCompensationRateModel>
{
    public CreateCompensationRateValidator()
    {
        RuleFor(model => model.Amount)
            .GreaterThan(0)
            .PrecisionScale(12, 2, true);

        RuleFor(model => model.RateType)
            .IsInEnum();

        RuleFor(model => model.Currency)
            .IsInEnum();

        When(model => model.ValidToDate is not null, () =>
        {
            RuleFor(model => model.ValidFromDate)
                .Must((model, validFrom) => validFrom <= model.ValidToDate)
                .WithMessage("ValidFromDate must be greater than or equal to ValidToDate");
        });
    }
}
