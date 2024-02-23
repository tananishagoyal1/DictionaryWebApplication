using DictionaryApp.ViewModels;
using FluentValidation;

namespace DictionaryApp.Validator
{
    public class RegistrationValidator : AbstractValidator<RegisterUserViewModel>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("The FirstName field is required.")
                .Matches(@"^[a-zA-Z']+$")
                .WithMessage("Invalid Name Format")
                .WithName("First Name");

            RuleFor(x => x.LastName)
                .Matches(@"^[a-zA-Z']+$")
                .WithMessage("Invalid Name Format")
                .WithName("Last Name");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("The Email field is required.")
                .Matches(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")
                .WithMessage("Invalid Email Format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("The Password field is required")
                .Matches(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
                .WithMessage("Password must contain Capital, small, number and special Characters with minimum length 8.");

            RuleFor(x => x.ConfirmPassword)
                 .NotEmpty().WithMessage("Confirm Password is required.")
                 .Equal(x => x.Password)
                 .WithMessage("Password and confirm password do not match.")
                 .WithName("Confirm Password");
        }
    }
}
