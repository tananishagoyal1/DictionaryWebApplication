using DictionaryApi.Business.Cache;
using DictionaryApi.DataAccess.DictionaryRepo;
using DictionaryApi.Models.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApi.Tests.Business.Cache.Tests
{
    [TestClass]
    public class CacheServiceTests
    {
        private readonly Mock<IDictionaryRepository> _dictionaryRepository;

        private readonly ICacheService _cacheService;

        public CacheServiceTests()
        {
            _dictionaryRepository = new Mock<IDictionaryRepository>();
            _cacheService = new CacheService(_dictionaryRepository.Object);
        }

        [TestMethod]      
        public async Task GetCachedWordData_wordInput_ReturnsNull()
        {
            //Arrange
            _dictionaryRepository.Setup(p => p.GetWordData()).ReturnsAsync( new List<WordDetailsModel>());

            //Act
            var cachedWordData = await _cacheService.GetCachedWordData(It.IsAny<string>());   

            //Assert
            Assert.IsNull(cachedWordData);

        }

        [TestMethod]
        [DataRow("book")]
        public async Task GetCachedWordData_wordInput_ReturnsWordData(string word)
        {
            //Arrange
            var allWords = new List<WordDetailsModel>()
            {
                new WordDetailsModel
                {
                    Word = word,
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

            _dictionaryRepository.Setup(repo => repo.GetWordData()).ReturnsAsync(allWords);

            //Act
            var cachedWordData = await _cacheService.GetCachedWordData(word);

            //Assert
            Assert.IsNotNull(cachedWordData);
            Assert.AreEqual(word, cachedWordData.Word);

        }

    }
}
