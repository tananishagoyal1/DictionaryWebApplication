using DictionaryApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace DictionaryApi.Tests.Controllers
{
    [TestClass]
    public class ErrorControllerTests
    {
        private readonly ErrorController _errorController;
        public ErrorControllerTests()
        {
            _errorController = new ErrorController();
        }
        [TestMethod]
        public void Error_ReturnsInternalServerError()
        {
          //Act
            var response = _errorController.Error();

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (response as ObjectResult).StatusCode);
            Assert.IsInstanceOfType(response, typeof(ObjectResult));
        }
    }
}
