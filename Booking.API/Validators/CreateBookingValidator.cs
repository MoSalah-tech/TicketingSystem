using Booking.API.Services;
using FluentValidation;
using System.Linq;

namespace Booking.API.Validators;

public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required.");

        RuleFor(x => x.SeatIds)
            .NotNull().WithMessage("SeatIds cannot be null.")
            .NotEmpty().WithMessage("At least one seat must be selected.");

        RuleFor(x => x.SeatIds)
            .Must(seats => seats == null || seats.Count <= 10)
            .WithMessage("You can book a maximum of 10 seats at once.");

        RuleFor(x => x.SeatIds)
            .Must(seats => seats == null || seats.Distinct().Count() == seats.Count)
            .WithMessage("Duplicate seat IDs are not allowed.");
    }
}