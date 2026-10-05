using EventCatalog.Application.Events;
using FluentValidation;

namespace EventCatalog.Application.Events.Validators;

public class CreateEventValidator : AbstractValidator<CreateEventDto>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Event name is required.")
            .MaximumLength(200).WithMessage("Event name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.EventDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Event date must be in the future.");

        RuleFor(x => x.VenueId)
            .NotEmpty().WithMessage("VenueId is required.");
    }
}