using DictionaryApi.Data;
using DictionaryApi.DataAccess.DictionaryRepo;
using DictionaryApi.DataAccess.UserCacheRepository;
using DictionaryApi.Models;
using DictionaryApi.Models.DTO;
using DictionaryApi.Tests.DataAccess.Helper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApi.Tests.DataAccess.DictionaryRepo.Tests
{
    [TestClass]
    public class DictionaryRepositoryTests
    {
        private readonly Mock<ApiDbContext> _apiDbContextMock;
        private readonly Mock<DbSet<WordDetailsModel>> _wordDetailsDbSet;
        private readonly DictionaryRepository _dictionaryRepository;

        public DictionaryRepositoryTests()
        {
            _apiDbContextMock = new Mock<ApiDbContext>();
            _wordDetailsDbSet = new Mock<DbSet<WordDetailsModel>>();
            var entries = WordDetailsList().AsQueryable();

            _wordDetailsDbSet.As<IAsyncEnumerable<WordDetailsModel>>()
            .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<WordDetailsModel>(entries.GetEnumerator()));
            _wordDetailsDbSet.As<IQueryable<WordDetailsModel>>()
            .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<WordDetailsModel>(entries.Provider));

            _wordDetailsDbSet.As<IQueryable<WordDetailsModel>>().Setup(m => m.Expression).Returns(entries.Expression);
            _wordDetailsDbSet.As<IQueryable<WordDetailsModel>>().Setup(m => m.ElementType).Returns(entries.ElementType);
            _wordDetailsDbSet.As<IQueryable<WordDetailsModel>>().Setup(m => m.GetEnumerator()).Returns(() => entries.GetEnumerator());
            _apiDbContextMock.Setup(context => context.WordDetails).Returns(_wordDetailsDbSet.Object);
            _apiDbContextMock.Setup(context => context.SaveChangesAsync(default))
                .ReturnsAsync(1)
                .Verifiable(Times.Once);

            _dictionaryRepository = new DictionaryRepository(_apiDbContextMock.Object);
        }

        [TestMethod]
        public async Task AddWordData_WordDetailsModel_ReturnsAddWordDataToDatabase()
        {
            //Arrange
            var wordData = new WordDetailsModel
            {
                Word = "test"

            };
            var entries = new List<WordDetailsModel>();
            _wordDetailsDbSet.Setup(p => p.AddAsync(It.IsAny<WordDetailsModel>(), default))
                .Callback<WordDetailsModel, CancellationToken>((wordData, token) => { entries.Add(wordData); })
                .ReturnsAsync(() => null)
                .Verifiable(Times.Once);
            _apiDbContextMock.Setup(p => p.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable(Times.Once);

            //Act
            await _dictionaryRepository.AddWordData(wordData);

            //Assert
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual(entries[0].Word, wordData.Word);
            _apiDbContextMock.Verify();
            _wordDetailsDbSet.Verify();
        }

        [TestMethod]
        public async Task GetWordData_ReturnsListOfWordDetailsMode()
        {
            var data = WordDetailsList();

            var result = await _dictionaryRepository.GetWordData();

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Count, result.Count);
        }


        private List<WordDetailsModel> WordDetailsList()
        {
            return new List<WordDetailsModel>()
            {
                new WordDetailsModel
                {
                    Word = "test",
                    Phonetics = new List<WordPhoneticModel>() { new() },

                    Meanings = new List<WordMeaningModel>()
                    {
                        new WordMeaningModel
                        {
                            Definitions = new List<WordDefinitionModel>()
                            {
                                new WordDefinitionModel
                                {
                                    Synonyms = new List<SynonymWord> { new SynonymWord { } },
                                    Antonyms = new List<AntonymWord> { new AntonymWord { } }
                                }
                            },
                            Synonyms = new List<SynonymWord> { new SynonymWord { } },
                            Antonyms = new List<AntonymWord> { new AntonymWord { } }
                        }
                    }
                }
            };
        }
    }
}
