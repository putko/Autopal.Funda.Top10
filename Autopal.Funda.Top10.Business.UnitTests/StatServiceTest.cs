using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autopal.Funda.Top10.Connector;
using Autopal.Funda.Top10.Connector.Model;
using Autopal.Funda.Top10.Connector.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Autopal.Funda.Top10.Business.UnitTests
{
    [TestClass]
    public class StatServiceTest
    {
        private Mock<ILogger<StatsService>> _loggerMock;
        private Mock<IFundaConnector> _connectorMock;
        private FundaConnectionSettings _settings;
        private Mock<StatsService> _statServiceMock;
        private StatsService _statService;

        [TestInitialize]
        public void TestInitialize()
        {
            _loggerMock = new Mock<ILogger<StatsService>>();
            _connectorMock = new Mock<IFundaConnector>();
            _settings = new FundaConnectionSettings
            {
                BaseUrl = "BASEURL",
                Key = "KEY",
                RateLimit = new RateLimit
                {
                    CallCount = 100,
                    TimeSpanInSeconds = 60
                }
            };
            _statServiceMock = new Mock<StatsService>(_connectorMock.Object, _settings, _loggerMock.Object);
            _statService = _statServiceMock.Object;
        }

        [TestMethod]
        [DataRow(10)]
        [DataRow(15)]
        public async Task GetTopNAgentsByOfferCount(int n)
        {
            // Arrange
            var offerResponse = new OffersResponse()
            {
                Offers = new List<Offer>(),
                TotalCount = 50,
                TotalPage = 2
            };
            offerResponse.Offers = GetOffersResponses();
            
            _connectorMock.Setup(fc => fc.GetOffersAsync(OfferType.Koop, ExteriorSpaces.Tuin, "amsterdam", It.IsAny<int>(), 25, CancellationToken.None))
                .Returns(Task.FromResult(offerResponse));

            // Act
            var resultObjects = await _statService.GetTopNAgentsByOfferCount("amsterdam", true, n, CancellationToken.None);

            // Assert
            Assert.AreEqual(n, resultObjects.Count);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task GetTopNAgentsByOfferCount_InvalidN(int n)
        {
            // Arrange
            var offerResponse = new OffersResponse()
            {
                Offers = new List<Offer>(),
                TotalCount = 50,
                TotalPage = 2
            };
            offerResponse.Offers = GetOffersResponses();

            _connectorMock.Setup(fc => fc.GetOffersAsync(OfferType.Koop, ExteriorSpaces.Tuin, "amsterdam", It.IsAny<int>(), 25, CancellationToken.None))
                .Returns(Task.FromResult(offerResponse));

            // Act
            var resultObjects = await _statService.GetTopNAgentsByOfferCount("amsterdam", true, n, CancellationToken.None);

            // Assert
            Assert.AreEqual(n, resultObjects.Count);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetTopNAgentsByOfferCount_InvalidRegion(string region)
        {
            // Arrange
            var offerResponse = new OffersResponse()
            {
                Offers = new List<Offer>(),
                TotalCount = 50,
                TotalPage = 2
            };
            offerResponse.Offers = GetOffersResponses();

            _connectorMock.Setup(fc => fc.GetOffersAsync(OfferType.Koop, ExteriorSpaces.Tuin, region, It.IsAny<int>(), 25, CancellationToken.None))
                .Returns(Task.FromResult(offerResponse));

            // Act
            var resultObjects = await _statService.GetTopNAgentsByOfferCount(region, true, 10, CancellationToken.None);

            // Assert
            Assert.AreEqual(10, resultObjects.Count);
        }
        
        [TestMethod]
        [DataRow(100)]
        public async Task GetTopNAgentsByOfferCount_NBiggerThanOfferCount(int n)
        {
            // Arrange
            var offerResponse = new OffersResponse()
            {
                Offers = new List<Offer>(),
                TotalCount = 50,
                TotalPage = 2
            };
            offerResponse.Offers = GetOffersResponses();

            _connectorMock.Setup(fc => fc.GetOffersAsync(OfferType.Koop, ExteriorSpaces.Tuin, "amsterdam", It.IsAny<int>(), 25, CancellationToken.None))
                .Returns(Task.FromResult(offerResponse));

            // Act
            var resultObjects = await _statService.GetTopNAgentsByOfferCount("amsterdam", true, n, CancellationToken.None);

            // Assert
            Assert.IsTrue(offerResponse.TotalCount > resultObjects.Count);
        }
        
        private static IList<Offer> GetOffersResponses()
        {
            var result = new List<Offer>();
            for (var i = 0; i < 25; i++)
            {
                var id = new Random().Next(75);
                result.Add(new Offer
                {
                    Address = new Guid().ToString(),
                    AgentId = id,
                    AgentName = id.ToString(),
                });
            }

            return result;
        }
    }
}
