using FluentValidation;
using Core.Application.DTOs.Requests;

namespace Core.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty().WithMessage("Email or username is required");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}