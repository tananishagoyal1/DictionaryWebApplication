using System.Security.Claims;

namespace DictionaryApp.Services
{
    public interface ITokenService
    {
        public string GetAccessToken();

        public void SetAccessToken(string token);


    }
}
