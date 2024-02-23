using DictionaryApi.Models.AuthenticationDataModels;
using DictionaryApp.DataAccess;
using DictionaryApp.Models;
using DictionaryApp.Services;
using DictionaryApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace DictionaryApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IDictionaryApiClient _dictionaryApiClient;
        private readonly ITokenService _tokenService;

        public UserController(IDictionaryApiClient dictionaryApiClient, ITokenService tokenService)
        {
            _dictionaryApiClient = dictionaryApiClient;
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new RegisterUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = model.Password
                    };

                    var result = await _dictionaryApiClient.Register(user);
                    if (result.StatusCode == HttpStatusCode.Created)
                    {
                        return RedirectToAction(nameof(Login), nameof(User));

                    }
                }
            }
            catch (ApiException ex)
            {
                ResponseModel? error = await ex.GetContentAsAsync<ResponseModel>();
                ViewBag.RegisterFailed = true;
                ViewBag.RegisterErrorMessages = error?.StatusMessage;
                return View();
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUser user, string? returnUrl)
        {
            if (!ModelState.IsValid)
            { 
                return View(user);
            }
            try
            {
                LoginResponseModel result = await _dictionaryApiClient.Login(user);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    _tokenService.SetAccessToken(result.JwtToken);
                    List<Claim> claims = new JwtSecurityTokenHandler().ReadJwtToken(result.JwtToken).Claims.ToList();
                    ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = false,
                            ExpiresUtc = DateTime.UtcNow.AddDays(1),
                        });
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }
            catch (ApiException ex)
            {
                LoginResponseModel? error = await ex.GetContentAsAsync<LoginResponseModel>();
                ViewBag.LoginFailed = true;
                ViewBag.LoginErrorMessages = error?.StatusMessage;

                return View();

            }
            return View();
        }

        [Microsoft.AspNetCore.Authorization.Authorize]

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userHistory = await _dictionaryApiClient.History();
            return View(userHistory);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("Token");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
