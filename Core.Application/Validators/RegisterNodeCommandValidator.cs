// MeshNetwork.Application/Validators/RegisterNodeCommandValidator.cs

using Core.Application.Commands;
using FluentValidation;

namespace Core.Application.Validators;

public class RegisterNodeCommandValidator : AbstractValidator<RegisterNodeCommand>
{
    public RegisterNodeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Node name is required")
            .MaximumLength(200).WithMessage("Node name must not exceed 200 characters");
        
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
        
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
        
        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("IP address is required")
            .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")
            .WithMessage("Invalid IP address format");
        
        RuleFor(x => x.MacAddress)
            .NotEmpty().WithMessage("MAC address is required")
            .Matches(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$")
            .WithMessage("Invalid MAC address format");
        
        RuleFor(x => x.BandwidthCapacity)
            .GreaterThan(0).WithMessage("Bandwidth capacity must be greater than 0");
    }
}