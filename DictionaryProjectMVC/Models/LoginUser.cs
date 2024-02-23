using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DictionaryApp.ViewModels
{
    public class LoginUser
    { 
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
