using System.ComponentModel.DataAnnotations;

namespace DictionaryApp.ViewModels
{
    public class RegisterUserViewModel
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
