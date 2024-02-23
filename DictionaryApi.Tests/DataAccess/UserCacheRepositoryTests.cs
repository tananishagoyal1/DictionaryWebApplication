using DictionaryApi.Data;
using DictionaryApi.DataAccess.UserCacheRepository;
using DictionaryApi.DataAccess.UserRepository;
using DictionaryApi.Models;
using DictionaryApi.Tests.DataAccess.Helper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApi.Tests.DataAccess
{
    [TestClass]
    public class UserCacheRepositoryTests
    {
        private readonly Mock<ApiDbContext> _apiDbContextMock;
        private readonly Mock<DbSet<UserCache>> _userCacheDbSet;
        private readonly UserCacheRepository _userCacheRepository;

        public UserCacheRepositoryTests()
        {
            _apiDbContextMock = new Mock<ApiDbContext>();
            _userCacheDbSet = new Mock<DbSet<UserCache>>();

            var entries = GetUserCacheList().AsQueryable();

            _userCacheDbSet.As<IAsyncEnumerable<UserCache>>()
            .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<UserCache>(entries.GetEnumerator()));
            _userCacheDbSet.As<IQueryable<UserCache>>()
            .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<UserCache>(entries.Provider));

            _userCacheDbSet.As<IQueryable<UserCache>>().Setup(m => m.Expression).Returns(entries.Expression);
            _userCacheDbSet.As<IQueryable<UserCache>>().Setup(m => m.ElementType).Returns(entries.ElementType);
            _userCacheDbSet.As<IQueryable<UserCache>>().Setup(m => m.GetEnumerator()).Returns(() => entries.GetEnumerator());
            
            _apiDbContextMock.Setup(context => context.UserCache).Returns(_userCacheDbSet.Object);
            _apiDbContextMock.Setup(context => context.SaveChangesAsync(default))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            _userCacheRepository = new UserCacheRepository(_apiDbContextMock.Object);
        }

        [TestMethod]
        public async Task AddUserSearchedWord_UserCache_ReturnsAddWordToDatabase()
        {
            //Arrange
            var history = new UserCache
            {
                UserId = "testUserId"
            };
            var entries = new List<UserCache>();
            _userCacheDbSet.Setup(p => p.AddAsync(It.IsAny<UserCache>(), default))
                .Callback<UserCache, CancellationToken>((history, token) => { entries.Add(history); })
                .ReturnsAsync(() => null)
                .Verifiable(Times.Once);
            _apiDbContextMock.Setup(p => p.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable(Times.Once);

            //Act
            await _userCacheRepository.AddUserSearchedWord(history);

            //Assert
            Assert.AreEqual(1,entries.Count);
            Assert.AreEqual(entries[0].UserId,history.UserId);
            _apiDbContextMock.Verify();
            _userCacheDbSet.Verify();
        }

        [TestMethod]
        public async Task UpdateWord_UserCache_ReturnsUpdateWordToDatabase()
        {
            //Arrange
            var oldUserCacheData = new UserCache
            {
                TimeStamp = DateTime.Now,
            };

            var newUserCacheData = new UserCache
            {
                TimeStamp = DateTime.Now.AddHours(1),
            };
            var entries = new List<UserCache>()
            {
                oldUserCacheData
            };
            _userCacheDbSet.Setup(p => p.Update(It.IsAny<UserCache>()))
                .Callback<UserCache>((newUserCacheData) => { entries[0] = newUserCacheData; })
                .Returns(() => null)
                .Verifiable(Times.Once);
            _apiDbContextMock.Setup(p => p.SaveChangesAsync(default))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            //Act
            await _userCacheRepository.UpdateWord(newUserCacheData);

            //Assert
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual(newUserCacheData.TimeStamp,entries[0].TimeStamp);
            _apiDbContextMock.Verify();
            _userCacheDbSet.Verify();
        }

        [TestMethod]
        public async Task RemoveWord_UserCache_ReturnsRemoveWordFromDatabase()
        {
            //Arrange
            var userCache = new UserCache
            {
                UserId = "testUserId",
            };
            var entries = new List<UserCache>()
            {
                userCache
                
            };
            _userCacheDbSet.Setup(p => p.Remove(It.IsAny<UserCache>()))
                .Callback<UserCache>((userCache) => { entries.Remove(userCache); })
                .Returns(() => null)
                .Verifiable(Times.Once);
            _apiDbContextMock.Setup(p => p.SaveChangesAsync(default))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            //Act
            await _userCacheRepository.RemoveWord(userCache);

            //Assert
            Assert.AreEqual(0, entries.Count);
            _apiDbContextMock.Verify();
            _userCacheDbSet.Verify();
        }


        [TestMethod]
        public async Task GetAllUserCache_UserId_ReturnsUserHistory()
        {
            //Arrange

            var userId = "testUserId";

            //Act
            var result =  await _userCacheRepository.GetAllUserCache(userId);

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(userId, result[0].UserId);
            Assert.AreEqual(userId, result[1].UserId);

        }

        [TestMethod]
        public async Task GetUserCache_UserId_ReturnsSearchedUserCache()
        {
            //Arrange
            var userCache = new UserCache
            {
                UserId = "testUserId",
                Word="test"
            };

            //Act
            var result = await _userCacheRepository.GetUserCache(userCache.Word, userCache.UserId);

            //Assert
            
            Assert.AreEqual(userCache.UserId, result.UserId);
            

        }

 
        private List<UserCache> GetUserCacheList()
        {
            return new List<UserCache>()
            {
                new UserCache { UserId = "testUserId" ,Word="test",TimeStamp = DateTime.Now},
                new UserCache { UserId = "testUserId", Word = "book",TimeStamp = DateTime.Now.AddMinutes(1) },
                new UserCache {UserId="userId", Word = "red", TimeStamp = DateTime.Now.AddMinutes(2)}
            };

        }
    }
}
