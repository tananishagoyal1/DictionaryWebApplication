using DictionaryApp.ViewModels;
using FluentValidation;

namespace DictionaryApp.Validator
{
    public class LoginValidator : AbstractValidator<LoginUser>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
              .NotEmpty().WithMessage("The Email field is required.")
              .Matches(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")
              .WithMessage("Invalid Email Format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("The Password field is required");

        }
    }
}
