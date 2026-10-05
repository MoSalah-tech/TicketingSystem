using EventCatalog.Application.Seats;
using FluentValidation;

namespace EventCatalog.Application.Seats.Validators;

public class GenerateSeatsValidator : AbstractValidator<GenerateSeatsDto>
{
    public GenerateSeatsValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required.");

        RuleFor(x => x.NumberOfRows)
            .GreaterThan(0).WithMessage("Number of rows must be greater than zero.")
            .LessThanOrEqualTo(26).WithMessage("Maximum of 26 rows allowed (A-Z).");

        RuleFor(x => x.SeatsPerRow)
            .GreaterThan(0).WithMessage("Seats per row must be greater than zero.")
            .LessThanOrEqualTo(50).WithMessage("Maximum of 50 seats per row.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
    }
}