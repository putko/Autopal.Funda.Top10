using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autopal.Funda.Top10.Business;
using Autopal.Funda.Top10.Business.Model;
using Autopal.Funda.Top10.Connector;
using Autopal.Funda.Top10.Connector.Model;
using Autopal.Funda.Top10.Connector.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Autopal.Funda.Top10.Console.UnitTests
{

    [TestClass]
    public class ServiceCallerTest
    {
        private Mock<IStatsService> _statServiceMock;
        private Mock<ServiceCaller> _serviceCallerMock;
        private ServiceCaller _serviceCaller;

        [TestInitialize]
        public void TestInitialize()
        {
            _statServiceMock = new Mock<IStatsService>();
            _serviceCallerMock = new Mock<ServiceCaller>(_statServiceMock.Object);
            _serviceCaller = _serviceCallerMock.Object;
        }

        [TestMethod]
        public async Task GetTopNAgentsByOfferCount()
        {
            // Arrange
            var expected = "---------------------------------------------------------------------\r\nTop 10 agents by offer with a garden count for sale  in amsterdam\r\nRank  Name                                               Offer Count\n\r\n1     One                                                   50\r\n2     Two                                                   40\r\n3     Three                                                 30\r\n4     Four                                                  20\r\n5     Five                                                  10\r\n";
            string output;
            var result = new List<Agent>
            {
                new() {Id = 1, Name = "One", OfferCount = 50},
                new() {Id = 2, Name = "Two", OfferCount = 40},
                new() {Id = 3, Name = "Three", OfferCount = 30},
                new() {Id = 4, Name = "Four", OfferCount = 20},
                new() {Id = 5, Name = "Five", OfferCount = 10},
            };
            _statServiceMock.Setup(service => service.GetTopNAgentsByOfferCount("amsterdam", true, 10, CancellationToken.None))
                .Returns(Task.FromResult(result));
            // Act
            await using (var sw = new StringWriter())
            {
                System.Console.SetOut(sw);

                await _serviceCaller.Call("amsterdam", true, 10);

                output = sw.ToString();
            }

            // Assert
            Assert.AreEqual<string>(expected, output);

        }

    }
}
