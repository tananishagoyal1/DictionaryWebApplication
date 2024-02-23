using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace DictionaryApp.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch(statusCode)
            {
                case 405:
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                    break;
            }
            return View("NotFound");
        }

        [Route("Error")]
        public async Task<IActionResult> Error()
        {
            var error = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if(error?.Error is ApiException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await HttpContext.SignOutAsync();
                    return RedirectToAction(nameof(UserController.Login), "User");
                }
            }
           
            return View("Error");

        }
    }
}
