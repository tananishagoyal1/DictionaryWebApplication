using DictionaryApi.Business.Services;
using DictionaryApi.DataAccess.UserRepository;
using DictionaryApi.Helper;
using DictionaryApi.Models;
using DictionaryApi.Models.AuthenticationDataModels;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApi.Tests.Business.AuthenticationService.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IConfiguration> _config;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _config = new Mock<IConfiguration>();
            _authService = new AuthService(_config.Object, _userRepository.Object);
        }


        [TestMethod]
        public async Task RegisterUser_RegisterUserModel_ReturnsRegisterResponseModelConflict()
        {
            //Arrange
            _userRepository.Setup(repo => repo.IsUserExists(It.IsAny<string>())).Returns(true);

            //Act
            var registerResponseModel = await _authService.RegisterUser(new RegisterUser());

            //Assert
            Assert.AreEqual(HttpStatusCode.Conflict, registerResponseModel.StatusCode);
            _userRepository.Verify(repo => repo.AddUser(new RegisterUser()),Times.Never);
        }

        [TestMethod]
        public async Task RegisterUser_RegisterUserModel_ReturnsRegisterResponseModelCreated()
        {
            //Arrange
            _userRepository.Setup(repo => repo.IsUserExists(It.IsAny<string>())).Returns(false);
            _userRepository.Setup(repo =>repo.AddUser(It.IsAny<RegisterUser>()));

            //Act
            var registerResponseModel = await _authService.RegisterUser(new RegisterUser());

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, registerResponseModel.StatusCode);
            _userRepository.Verify(repo => repo.AddUser(It.IsAny<RegisterUser>()), Times.Once);
        }

        [TestMethod]
        public void LoginUser_LoginUsereModel_ReturnsLoginResponseModelUnAuthorized()
        {
            //Arrange
            _userRepository.Setup(repo => repo.GetCurrentUser(It.IsAny<string>(),It.IsAny<string>())).Returns(()=>null);

            //Act
            var loginResponseModel =  _authService.LoginUser(new LoginUser());

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, loginResponseModel.StatusCode);

        }

        [TestMethod]
        public void LoginUser_LoginUsereModel_ReturnsLoginResponseModelOk()
        {
            //Arrange
            var registerUser = new RegisterUser()
            {
                FirstName = "Test",
                Id = "dfghj"

            };
            _userRepository.Setup(repo => repo.GetCurrentUser(It.IsAny<string>(), It.IsAny<string>())).Returns(registerUser);
            _config.Setup(p => p[Constants.JWT__Key]).Returns("sdfghjk45678jhyhjgf4hd4hdfg56DFGH");
            _config.Setup(p => p[Constants.JWT__Issuer]).Returns("hxcvbngj");
            _config.Setup(p => p[Constants.JWT__Audience]).Returns("3fgfydrt");

            //Act
            var loginResponseModel = _authService.LoginUser(new LoginUser());

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, loginResponseModel.StatusCode);

        }

    }
}
