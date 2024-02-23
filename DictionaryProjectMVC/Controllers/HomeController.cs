using DictionaryApp.DataAccess;
using DictionaryApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DictionaryApp.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace DictionaryApp.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly IDictionaryApiClient _dictionaryApiClient;
            
        public HomeController(IDictionaryApiClient dictionaryApiClient)
        {
            _dictionaryApiClient = dictionaryApiClient;
        }


  
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string word)
        {
            try
            {
                var model = await _dictionaryApiClient.GetWordDetailsAsync(word);
                if (model == null)
                {
                    return View(nameof(Index));
                }
                return View(new InputWordViewModel
                {
                    InputWord = "",
                    WordMeaning = model,
                });
            }
            catch (Refit.ApiException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var wordNotFoundDetails = await ex.GetContentAsAsync<WordNotFound>();
                    return View(nameof(WordNotFound), wordNotFoundDetails);
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await HttpContext.SignOutAsync();
                    return RedirectToAction(nameof(UserController.Login), "User");
                }
                else
                {
                    return View("Error");
                }
            }

        }

        [HttpPost]
        public async Task<IActionResult> Search(InputWordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Search), new
                {
                    word = model.InputWord
                });

            }

        }
    }
}