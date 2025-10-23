using Core.Application.Commands;
using FluentValidation;

namespace Core.Application.Validators;

public class CalculateRouteCommandValidator : AbstractValidator<CalculateRouteCommand>
{
    public CalculateRouteCommandValidator()
    {
        RuleFor(x => x.SourceNodeId)
            .NotEmpty().WithMessage("Source node ID is required");
        
        RuleFor(x => x.DestinationNodeId)
            .NotEmpty().WithMessage("Destination node ID is required");
        
        RuleFor(x => x.SourceNodeId)
            .NotEqual(x => x.DestinationNodeId)
            .WithMessage("Source and destination nodes must be different");
        
        RuleFor(x => x.MaxHops)
            .InclusiveBetween(1, 10).WithMessage("Max hops must be between 1 and 10");
    }
}