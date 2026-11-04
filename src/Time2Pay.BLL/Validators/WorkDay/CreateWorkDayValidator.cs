using FluentValidation;
using Time2Pay.DTO.WorkDay;
using Time2Pay.Shared;
using Time2Pay.Shared.Enums;

namespace Time2Pay.BLL.Validators.WorkDay;

public class CreateWorkDayValidator : AbstractValidator<CreateWorkDayModel>
{
    public CreateWorkDayValidator()
    {
        RuleFor(model => model.EmploymentId)
            .NotEmpty();

        RuleFor(model => model.Date)
            .NotEmpty();

        // FluentValidation’s length validators treat null as valid and skip the check
        RuleFor(model => model.Comment)
            .MaximumLength(Constants.CommentMaxLength);

        RuleFor(model => model.DayType)
            .IsInEnum();

        When(model => model.DayType == DayType.Work, () =>
            {
                RuleFor(model => model.StartedAt).NotNull();
                RuleFor(model => model.EndedAt).NotNull();

                RuleFor(model => model.EndedAt)
                    .Must((model, endedAt) => endedAt > model.StartedAt);

                RuleFor(model => model.BreakMinutes)
                    .Must((model, breakMinutes) => breakMinutes <= (model.EndedAt!.Value - model.StartedAt!.Value).TotalMinutes)
                    .When(model => model.StartedAt is not null && model.EndedAt is not null)
                    .WithMessage("Break cannot be longer than the tracked period");
            })
            .Otherwise(() =>
            {
                RuleFor(model => model.StartedAt).Null();
                RuleFor(model => model.EndedAt).Null();
                RuleFor(model => model.BreakMinutes).Equal(0);
            });
    }
}
