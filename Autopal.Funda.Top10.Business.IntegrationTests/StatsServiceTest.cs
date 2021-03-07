using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Autopal.Funda.Top10.Business.IntegrationTests
{
    [TestClass]
    public class StatsServiceTest : ServiceProviderTestBase
    {
        private IStatsService _statService;

        [TestInitialize]
        public void TestInitialize()
        {
            _statService = GetService<IStatsService>();
        }

        [TestMethod]
        public async Task HappyFlow()
        {
            var results = await _statService.GetTopNAgentsByOfferCount("amsterdam", true, 10, CancellationToken.None);
            Assert.AreEqual(10, results.Count);
        }
    }
}
