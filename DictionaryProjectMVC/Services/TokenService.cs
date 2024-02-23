using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DictionaryApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenService()
        {
            _contextAccessor = new HttpContextAccessor();
        }
        public string? GetAccessToken()
        {
            return _contextAccessor.HttpContext.Request.Cookies["Token"];
        }

        public void SetAccessToken(string token)
        {
            _contextAccessor.HttpContext.Response.Cookies.Append("Token", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays( 1)
            });
        }
    }
}
