using DictionaryApp.ViewModels;
using FluentValidation;

namespace DictionaryApp.Validator
{
    public class InputWordValidator:AbstractValidator<InputWordViewModel>
    {
        public InputWordValidator()
        {
            RuleFor(x=>x.InputWord)
                .NotEmpty()
                .WithMessage("The InputWord field is required.");
        }
    }
}
