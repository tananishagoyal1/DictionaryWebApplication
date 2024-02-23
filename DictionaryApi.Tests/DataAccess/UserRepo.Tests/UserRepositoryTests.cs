using DictionaryApi.Data;
using DictionaryApi.Models.AuthenticationDataModels;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using DictionaryApi.DataAccess.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DictionaryApi.Models;

namespace DictionaryApi.Tests.DataAccess.UserRepo.Tests
{
    [TestClass]

    public class UserRepositoryTests
    {
        private readonly Mock<ApiDbContext> _apiDbContextMock;
        private readonly Mock<DbSet<RegisterUser>> _usersDbSet;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _apiDbContextMock = new Mock<ApiDbContext>();
            _usersDbSet = new Mock<DbSet<RegisterUser>>();
            var entries = GetUserList().AsQueryable();
            _usersDbSet.As<IQueryable<RegisterUser>>().Setup(m => m.Provider).Returns(entries.Provider);
            _usersDbSet.As<IQueryable<RegisterUser>>().Setup(m => m.Expression).Returns(entries.Expression);
            _usersDbSet.As<IQueryable<RegisterUser>>().Setup(m => m.ElementType).Returns(entries.ElementType);
            _usersDbSet.As<IQueryable<RegisterUser>>().Setup(m => m.GetEnumerator()).Returns(() => entries.GetEnumerator());
            _apiDbContextMock.Setup(context => context.Users).Returns(_usersDbSet.Object);
            _apiDbContextMock.Setup(context => context.SaveChangesAsync(default))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            _userRepository = new UserRepository(_apiDbContextMock.Object);
        }

        [TestMethod]
        public async Task AddUser_RegisterUser_ReturnsAddUserToDatabase()
        {
            //Arrange
            var registerUser = new RegisterUser
            {
                Email = "test@example.com"
            };
            var registerUserEntries = new List<RegisterUser>();
            _usersDbSet.Setup(p => p.AddAsync(It.IsAny<RegisterUser>(), default))
                .Callback<RegisterUser, CancellationToken>((registerUser, token) => { registerUserEntries.Add(registerUser); })
                .ReturnsAsync(() => null)
                .Verifiable(Times.Once);
            _apiDbContextMock.Setup(p => p.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable(Times.Once);

            //Act
            await _userRepository.AddUser(registerUser);

            //Assert
            Assert.AreEqual(1, registerUserEntries.Count);
            Assert.AreEqual(registerUserEntries[0].Email, registerUser.Email);
            _apiDbContextMock.Verify();
            _usersDbSet.Verify();
        }

        [TestMethod]
        public void GetCurrentUser_Email_ReturnsCurrentUser()
        {
            //Arrange

             var email = "test@example.com";
             string hashedPassword = "test123";

            //Act
            var result = _userRepository.GetCurrentUser(email,hashedPassword);

            //Assert
            Assert.AreEqual(email,result.Email);
            Assert.IsNotNull(result);


        }



        [TestMethod]
        public void GetCurrentUser_Email_ReturnsNull()
        {
            //Arrange

            var email = "test2@example.com";
            string hashedPassword = "test123";

            //Act
            var result = _userRepository.GetCurrentUser(email, hashedPassword);

            //Assert
            Assert.IsNull(result);


        }

        [TestMethod]
        public void IsUserExists_Email_ReturnsTrue()
        {
            //Arrange
            string email = "test@example.com";

            //Act
            var result = _userRepository.IsUserExists(email);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsUserExists_Email_ReturnsFalse()
        {
            //Arrange
            string email = "test2@emaple.com";

            //Act
            var result = _userRepository.IsUserExists(email);

            //Assert
            Assert.IsFalse(result);
        }


        private List<RegisterUser> GetUserList()
        {
            return new List<RegisterUser>()
            {
                new RegisterUser { Email = "test@example.com", Password="test123"},
                new RegisterUser { Email = "abc2example.com",Password = "pink12"}
               
            };

        }


    }
}
