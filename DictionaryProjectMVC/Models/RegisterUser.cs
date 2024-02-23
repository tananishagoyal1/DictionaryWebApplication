using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DictionaryApp.Models
{
    public class RegisterUser
    {     
        public string FirstName { get; set; }

        public string? LastName { get; set; }
       
        public string Email { get; set; }
        
        public string Password { get; set; }

    }
}
