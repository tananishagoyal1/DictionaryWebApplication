using DictionaryApi.Models.AuthenticationDataModels;
using DictionaryApp.Models;
using DictionaryApp.Models.DTO;
using DictionaryApp.ViewModels;
using Refit;

namespace DictionaryApp.DataAccess
{
    public interface IDictionaryApiClient
    {
        [Get("/api/Dictionary/Search?word={word}")]
        Task<WordData> GetWordDetailsAsync(string word);

        [Post("/api/User/Register")]
        Task<ResponseModel> Register([Body] RegisterUser user);

        [Post("/api/User/Login")]
        Task<LoginResponseModel> Login([Body]LoginUser model);

        [Get("/api/user/History")]
        Task<List<string>> History();


    }
}
