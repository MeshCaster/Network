// MeshNetwork.Application/Validators/UpdateNodeHeartbeatCommandValidator.cs
using FluentValidation;
using Core.Application.Commands;

namespace Core.Application.Validators;

public class UpdateNodeHeartbeatCommandValidator : AbstractValidator<UpdateNodeHeartbeatCommand>
{
    public UpdateNodeHeartbeatCommandValidator()
    {
        RuleFor(x => x.NodeId)
            .NotEmpty().WithMessage("Node ID is required");
        
        RuleFor(x => x.SignalStrength)
            .InclusiveBetween(0, 100).WithMessage("Signal strength must be between 0 and 100");
        
        RuleFor(x => x.CpuUsage)
            .InclusiveBetween(0, 100).WithMessage("CPU usage must be between 0 and 100");
        
        RuleFor(x => x.MemoryUsage)
            .InclusiveBetween(0, 100).WithMessage("Memory usage must be between 0 and 100");
        
        RuleFor(x => x.Temperature)
            .InclusiveBetween(-50, 150).WithMessage("Temperature must be between -50 and 150 degrees");
    }
}