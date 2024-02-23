using DictionaryApi.Business.ExternalApiHandler;
using DictionaryApi.Business.Services;
using DictionaryApi.Controllers;
using DictionaryApi.DictionaryDataModels;
using DictionaryApi.Models.WordNotFound;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Refit;
using System.Net;
using System.Security.Claims;


namespace DictionaryApi.Tests.Controllers
{
    [TestClass]
    public class DictionaryControllerTests
    {
        private readonly Mock<IExternalDictionaryApiService> _externalApiService;
        private readonly Mock<IUserCacheService> _userCacheService;
        private readonly DictionaryController _dictionaryController;
        private readonly Mock<ClaimsPrincipal> _claimsPrincipal;

        public DictionaryControllerTests()
        {
            _externalApiService = new Mock<IExternalDictionaryApiService>();
            _userCacheService = new Mock<IUserCacheService>();
            _claimsPrincipal = new Mock<ClaimsPrincipal>();
            _dictionaryController = new DictionaryController(_externalApiService.Object, _userCacheService.Object);
        }

        [TestMethod]
        public async Task GetWordData_Word_ReturnsOk()
        {
            //Arrange          
            _externalApiService.Setup(service => service.GetWordDataFromApiAsync(It.IsAny<string>())).ReturnsAsync(new WordData());
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, "testid"),
               new Claim(ClaimTypes.Email, "Test.goyal@example.com"),
            };
            _claimsPrincipal.Setup(p => p.Claims).Returns(claims);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _dictionaryController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            _userCacheService.Setup(service => service.HandleUserHistory(It.IsAny<string>(), It.IsAny<string>()));

            //Act
            var response = await _dictionaryController.GetWordData(It.IsAny<string>());  
            
            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull((response as ObjectResult).Value);
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (response as ObjectResult).StatusCode);
            _userCacheService.Verify(p => p.HandleUserHistory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]

        public async Task GetWordData_Word_ReturnsWordNotFound()
        {
            RefitSettings refitSettings = new();
            var exception = await ApiException.Create(
              new HttpRequestMessage(),
             HttpMethod.Get,
             new HttpResponseMessage
             {
                 StatusCode = HttpStatusCode.NotFound,
                 Content = refitSettings.ContentSerializer.ToHttpContent(new WordNotFound())
             },
             refitSettings);
            _externalApiService.Setup(service => service.GetWordDataFromApiAsync(It.IsAny<string>()))
                  .ThrowsAsync(exception);
          
                var result = await _dictionaryController.GetWordData(It.IsAny<string>());
        
               
                Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
                Assert.IsInstanceOfType((result as NotFoundObjectResult).Value,typeof(WordNotFound));
        }

        [TestMethod]
        public async Task GetWordData_Word_ReturnsInternalServerError()
        {
            RefitSettings refitSettings = new();
            var exception = await ApiException.Create(
              new HttpRequestMessage(),
             HttpMethod.Get,
             new HttpResponseMessage
             {
                 StatusCode = HttpStatusCode.InternalServerError,
          
             },
             refitSettings);
            _externalApiService.Setup(service => service.GetWordDataFromApiAsync(It.IsAny<string>()))
                  .ThrowsAsync(exception);

            var result = await _dictionaryController.GetWordData(It.IsAny<string>());


            Assert.AreEqual((result as ObjectResult).StatusCode,StatusCodes.Status500InternalServerError);
           
        }

    }
}

