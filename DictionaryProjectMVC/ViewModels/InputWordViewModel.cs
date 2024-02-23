using DictionaryApp.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace DictionaryApp.ViewModels
{
    public class InputWordViewModel
    {        
        public string? InputWord { get; set; }
        public WordData? WordMeaning { get; set; }
    }
}
