using DictionaryApi.Business.Services;
using DictionaryApi.Controllers;
using DictionaryApi.Models;
using DictionaryApi.Models.AuthenticationDataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

namespace DictionaryApi.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        private readonly Mock<IAuthService> _authenticationService;
        private readonly Mock<IUserCacheService> _userCacheService;
        private readonly UserController _userController;
        private readonly Mock<ClaimsPrincipal> _claimsPrincipal;

        public UserControllerTests()
        {
            _authenticationService = new Mock<IAuthService>();
            _claimsPrincipal = new Mock<ClaimsPrincipal>();
            _userCacheService = new Mock<IUserCacheService>();
            _userController = new UserController(_authenticationService.Object, _userCacheService.Object);           
        }

        [TestMethod]
        public async Task Register_RegisterUserModel_ReturnsConflict()
        {
            //Arrange
            ResponseModel responseModel = new ResponseModel()
            {
                 StatusCode = HttpStatusCode.Conflict,

            };
            _authenticationService.Setup(service => service.RegisterUser(It.IsAny<RegisterUser>())).ReturnsAsync(responseModel);

            //Act
            var response = await _userController.Register(new RegisterUser());

            //Assert
            Assert.AreEqual(StatusCodes.Status409Conflict,(response as ObjectResult).StatusCode);
            Assert.IsNotNull(responseModel);
            Assert.IsInstanceOfType(response, typeof(ConflictObjectResult));
        }

        [TestMethod]
        public async Task Register_RegisterUserModel_ReturnsCreated()
        {
            //Arrange
            ResponseModel responseModel = new ResponseModel()
            {
                StatusCode = HttpStatusCode.Created,

            };
            _authenticationService.Setup(service => service.RegisterUser(It.IsAny<RegisterUser>())).ReturnsAsync(responseModel);

            //Act
            var response = await _userController.Register(new RegisterUser());

            //Assert
            Assert.AreEqual(StatusCodes.Status201Created, (response as ObjectResult).StatusCode);
            Assert.IsNotNull(responseModel);
            Assert.IsInstanceOfType(response,typeof(ObjectResult));
        }

        [TestMethod]
        public void  Login_LoginUserModel_ReturnsUnauthorized()
        {
            //Arrange
            LoginResponseModel responseModel = new LoginResponseModel()
            {
                StatusCode = HttpStatusCode.Unauthorized,

            };
            _authenticationService.Setup(service => service.LoginUser(It.IsAny<LoginUser>())).Returns(responseModel);

            //Act
            var response =  _userController.Login(new LoginUser());

            //Assert
            Assert.AreEqual(StatusCodes.Status401Unauthorized, (response as ObjectResult).StatusCode);
            Assert.IsNotNull(responseModel);
            Assert.IsInstanceOfType(response, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void Login_LoginUserModel_ReturnsOK()
        {
            //Arrange
            LoginResponseModel responseModel = new LoginResponseModel()
            {
                StatusCode = HttpStatusCode.OK,

            };
            _authenticationService.Setup(service => service.LoginUser(It.IsAny<LoginUser>())).Returns(responseModel);

            //Act
            var response = _userController.Login(new LoginUser());

            //Assert
            Assert.AreEqual(StatusCodes.Status200OK, (response as ObjectResult).StatusCode);
            Assert.IsNotNull(responseModel);
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task UserSearchHistory_ReturnsOK()
        {
            //Arrange
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, "dcvcvbne"),
               new Claim(ClaimTypes.Email, "Tanisha.goyal@example.com"),
            };
            _claimsPrincipal.Setup(p => p.Claims).Returns(claims);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userCacheService.Setup(p => p.GetUserHistory(It.IsAny<string>())).ReturnsAsync(new List<string>());

            //Act
            var response = await _userController.History();

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(StatusCodes.Status200OK, (response as ObjectResult).StatusCode);
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));

        }      

    }
}
