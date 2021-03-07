using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Autopal.Funda.Top10.Connector.Client;
using Autopal.Funda.Top10.Connector.Client.Model;
using Autopal.Funda.Top10.Connector.Model;
using Autopal.Funda.Top10.Connector.Model.MappingProfiles;
using Autopal.Funda.Top10.Connector.Settings;
using CVV;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Object = Autopal.Funda.Top10.Connector.Client.Model.Object;

namespace Autopal.Funda.Top10.Connector.UnitTests
{
   [TestClass]
    public class FundaConnectorTest
    {
        private Mock<ILogger<IFundaConnector>> _loggerMock;
        private Mock<FundaConnector> _connectorMock;
        private Mock<FundaClient> _clientMock;
        private Mock<RateGate> _rateGateMock;
        private FundaConnectionSettings _settings;
        private IFundaConnector _connector;

        [TestInitialize]
        public void TestInitialize()
        {
            _loggerMock = new Mock<ILogger<IFundaConnector>>();
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("testContent")
                });

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
            _clientMock = new Mock<FundaClient>(new HttpClient(mockMessageHandler.Object), _settings, new Mock<ILogger<FundaClient>>().Object);

            _rateGateMock = new Mock<RateGate>(_settings.RateLimit.CallCount,
                TimeSpan.FromSeconds(_settings.RateLimit.TimeSpanInSeconds));
            var myProfile = new OffersResponseMappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper mapper = new Mapper(configuration);
            _connectorMock = new Mock<FundaConnector>(_clientMock.Object, _rateGateMock.Object, mapper, _loggerMock.Object);
            _connector = _connectorMock.Object;
        }

        [TestMethod]
        public async Task GetTopNAgentsByOfferCount()
        {
            // Arrange
            var response = new GetOffersResponse()
            {
               TotaalAantalObjecten = 5000,
               Objects = new List<Object>(),
               Paging = new Paging
               {
                   AantalPaginas = 200,
                   HuidigePagina = 3,
               }
            };
           
            _clientMock.Setup(fc => fc.GetOffersAsync(OfferType.Koop, ExteriorSpaces.Tuin, "amsterdam", 3, 25, CancellationToken.None))
                .Returns(Task.FromResult(response));

            // Act
            var resultObjects = await _connector.GetOffersAsync(OfferType.Koop, ExteriorSpaces.Tuin, "amsterdam", 3, 25, CancellationToken.None);

            // Assert
            Assert.AreEqual(response.TotaalAantalObjecten, resultObjects.TotalCount);
            Assert.AreEqual(response.Paging.AantalPaginas, resultObjects.TotalPage);
        }
    }
}
