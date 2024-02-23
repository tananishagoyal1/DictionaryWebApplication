using DictionaryApi.Business.Services;
using DictionaryApi.DataAccess.UserCacheRepository;
using DictionaryApi.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApi.Tests.Business.Services.Tests
{

    [TestClass]
    public class UserCacheServiceTests
    {
        private readonly Mock<IUserCacheRepository> _userCacheRepository;
        private readonly IUserCacheService _userCacheService;

        public UserCacheServiceTests()
        {
            _userCacheRepository = new Mock<IUserCacheRepository>();
            _userCacheService = new UserCacheService(_userCacheRepository.Object);


        }     
        
        [TestMethod]
        [DataRow("book","abcdef")]
        public async Task UpdateUserHistory_userCache(string word, string userId)
        {
            //Arrange
            UserCache userCache = new UserCache()
            {
                Word = word,
                UserId = userId,
                TimeStamp = DateTime.Now,
            };
            _userCacheRepository.Setup(repo => repo.UpdateWord(userCache));

            //Act
            await _userCacheService.UpdateUserHistory(userCache);

            //Assert
            _userCacheRepository.Verify(repo=>repo.UpdateWord(userCache),Times.Once);

        }

        [TestMethod]
        [DataRow("abchf34")]
        public async Task GetUserHistory_UserId_ReturnsListOfWords(string userId)
        {
            //Arrange
          
            List<UserCache> userCache = new List<UserCache>() { new UserCache { UserId =userId  } };
            _userCacheRepository.Setup(repo => repo.GetAllUserCache(It.IsAny<string>()))
                .ReturnsAsync(userCache);

            //Act
            var words = await _userCacheService.GetUserHistory(It.IsAny<string>());

            //Assert
            Assert.IsNotNull(words);
            _userCacheRepository.Verify(repo=>repo.GetAllUserCache(It.IsAny<string>()), Times.Once);


        }

        [TestMethod]

        public async Task GetUserHistory_UserId_ReturnsEmptyList()
        {
            //Arrange
            _userCacheRepository.Setup(repo => repo.GetAllUserCache(It.IsAny<string>())).ReturnsAsync(new List<UserCache>());

            //Act
            var words = await _userCacheService.GetUserHistory(It.IsAny<string>());

            //Assert
            Assert.IsNotNull(words);
            _userCacheRepository.Verify(repo => repo.GetAllUserCache(It.IsAny<string>()), Times.Once);

        }

        [TestMethod]
        [DataRow("abchfed")]
        public async Task AddUserHistory_WordAndIserId_AddNewCacheData(string userId)
        {
            //Arrange
            List<UserCache> userCache = new List<UserCache>() { new UserCache { UserId = userId } };
            _userCacheRepository.Setup(repo => repo.GetAllUserCache(It.IsAny<string>()))
                .ReturnsAsync(userCache);

            _userCacheRepository.Setup(p => p.AddUserSearchedWord(It.IsAny<UserCache>()));

            //Act
            await _userCacheService.AddUserHistory(It.IsAny<string>(), userId);

            //Assert
            _userCacheRepository.Verify(repo => repo.AddUserSearchedWord(It.IsAny<UserCache>()), Times.Once);
            _userCacheRepository.Verify(repo => repo.RemoveWord(It.IsAny<UserCache>()), Times.Never);

        }

        [TestMethod]
        [DataRow("abcgf56")]
        public async Task AddUserHistory_WordAndUserId_AddNewUserData_RemoveCacheData(string userId)
        {
            //Arrange
            List<UserCache> userCache = new List<UserCache>()
            { 
                new UserCache { },
                new UserCache { },
                new UserCache { },
                new UserCache { },
                new UserCache { },
                new UserCache { } 
            };
            _userCacheRepository.Setup(repo => repo.GetAllUserCache(It.IsAny<string>()))
                .ReturnsAsync(userCache);

            _userCacheRepository.Setup(repo => repo.AddUserSearchedWord(It.IsAny<UserCache>()));

            //Act
            await _userCacheService.AddUserHistory(It.IsAny<string>(), userId);

            //Assert
            _userCacheRepository.Verify(repo => repo.AddUserSearchedWord(It.IsAny<UserCache>()), Times.Once);
            _userCacheRepository.Verify(repo => repo.RemoveWord(It.IsAny<UserCache>()), Times.AtLeastOnce);

        }



        //TODO
        [TestMethod]
        public async Task HandleUserHistory_SearchedExistingWordAndUserId()
        {
            //Arrange
            _userCacheRepository.Setup(repo => repo.GetUserCache(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserCache());

            //Act
            await _userCacheService.HandleUserHistory(It.IsAny<string>(), It.IsAny<string>());

        }

        [TestMethod]
        public async Task HandleUserHistory_AddNonExistingSearchedWordAndUserId()
        {
            //Arrange

            _userCacheRepository.Setup(repo => repo.GetUserCache(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(()=>null);
            List<UserCache> userCache = new List<UserCache>() { new UserCache () };
            _userCacheRepository.Setup(p => p.GetAllUserCache(It.IsAny<string>()))
                .ReturnsAsync(userCache);

            //Act
            await _userCacheService.HandleUserHistory(It.IsAny<string>(), It.IsAny<string>());

        }

    }
}
