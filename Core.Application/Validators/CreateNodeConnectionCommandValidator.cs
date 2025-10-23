// MeshNetwork.Application/Validators/CreateNodeConnectionCommandValidator.cs
using FluentValidation;
using Core.Application.Commands;

namespace Core.Application.Validators;

public class CreateNodeConnectionCommandValidator : AbstractValidator<CreateNodeConnectionCommand>
{
    public CreateNodeConnectionCommandValidator()
    {
        RuleFor(x => x.SourceNodeId)
            .NotEmpty().WithMessage("Source node ID is required");
        
        RuleFor(x => x.TargetNodeId)
            .NotEmpty().WithMessage("Target node ID is required");
        
        RuleFor(x => x.SourceNodeId)
            .NotEqual(x => x.TargetNodeId)
            .WithMessage("Source and target nodes must be different");
        
        RuleFor(x => x.Quality)
            .InclusiveBetween(0, 100).WithMessage("Quality must be between 0 and 100");
        
        RuleFor(x => x.Latency)
            .GreaterThanOrEqualTo(0).WithMessage("Latency must be non-negative");
        
        RuleFor(x => x.Throughput)
            .GreaterThan(0).WithMessage("Throughput must be greater than 0");
        
        RuleFor(x => x.PacketLoss)
            .InclusiveBetween(0, 100).WithMessage("Packet loss must be between 0 and 100");
    }
}
