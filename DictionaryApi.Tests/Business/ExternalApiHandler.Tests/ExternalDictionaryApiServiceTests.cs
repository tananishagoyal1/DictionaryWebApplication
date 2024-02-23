using DictionaryApi.Business.Cache;
using DictionaryApi.Business.ExternalApiHandler;
using DictionaryApi.DataAccess.DictionaryRepo;
using DictionaryApi.DictionaryDataModels;
using DictionaryApi.ExternalApiManager;
using DictionaryApi.Models.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApi.Tests.Business.ExternalApiHandler.Tests
{
    [TestClass]
    public class ExternalDictionaryApiServiceTests
    {

        private readonly Mock<IDictionaryApiClient> _dictionaryApiClient;
        private readonly Mock<ICacheService> _cacheService;
        private readonly Mock<IDictionaryRepository> _dictionaryRepository;
        private readonly IExternalDictionaryApiService _dictionaryApiService;


        public ExternalDictionaryApiServiceTests()
        {
            _dictionaryApiClient = new Mock<IDictionaryApiClient>();
            _cacheService = new Mock<ICacheService>();
            _dictionaryRepository = new Mock<IDictionaryRepository>();
            _dictionaryApiService= new ExternalDictionaryApiService
                (_dictionaryApiClient.Object,
                _cacheService.Object,
                _dictionaryRepository.Object);
        }

        [TestMethod]
        public async Task GetWordDataFRomApiAsync_Word_ReturnsCachedWordData()
        {
            //Arrange
            _cacheService.Setup(cache => cache.GetCachedWordData(It.IsAny<string>())).ReturnsAsync(new DictionaryDataModels.WordData());

            //Act
            var cachedWordData= await _dictionaryApiService.GetWordDataFromApiAsync(It.IsAny<string>());

            //Assert
            Assert.IsNotNull(cachedWordData);
            Assert.AreEqual(It.IsAny<string>(), cachedWordData.Word);

        }

        [TestMethod]

        public async Task GetWordDataFRomApiAsync_Word_ReturnsWordData()
        {
            //Arrange
            _cacheService.Setup(cachedData => cachedData.GetCachedWordData(It.IsAny<string>())).ReturnsAsync(()=>null);
            _dictionaryApiClient.Setup(p => p.GetWordDetailsAsync(It.IsAny<string>())).ReturnsAsync(new List<WordData>
            {
                new WordData()
                {
                    Phonetics = new List<Phonetic>(),
                    Meanings = new List<Meaning>() { 
                        new Meaning() { 
                        Definitions = new() { 
                            new() { 
                                Antonyms = new(), 
                                Synonyms = new() 
                            } 
                        },
                        Antonyms = new(),
                        Synonyms = new()
                        } 
                    },
                }
            });
            
            _dictionaryRepository.Setup(repo => repo.AddWordData(new WordDetailsModel()));
                 

            //Act
            var wordData = await _dictionaryApiService.GetWordDataFromApiAsync(It.IsAny<string>());

            //Assert
            Assert.IsNotNull(wordData);
            _dictionaryApiClient.Verify(client => client.GetWordDetailsAsync(It.IsAny<string>()), Times.Once);
            _cacheService.Verify(service => service.GetCachedWordData(It.IsAny<string>()), Times.Once);
            _dictionaryRepository.Verify(repo => repo.AddWordData(It.IsAny<WordDetailsModel>()), Times.Once);


        }
    }
}
