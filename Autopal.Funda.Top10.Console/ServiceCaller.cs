using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autopal.Funda.Top10.Business;
using Autopal.Funda.Top10.Business.Model;

namespace Autopal.Funda.Top10.Console
{
    public class ServiceCaller
    {
        private readonly IStatsService _service;

        public ServiceCaller(IStatsService service)
        {
            _service = service;
        }

        public async Task Call(string region, bool isWithGarden, int topN)
        {
            var top10AgentsByOfferCount =
                await _service.GetTopNAgentsByOfferCount("amsterdam", isWithGarden, topN, CancellationToken.None);
            WriteAgentList(top10AgentsByOfferCount,
                $"Top {topN} agents by offer {(isWithGarden ? "with a garden " : string.Empty)}count for sale  in {region}");
        }

        private static void WriteAgentList(IList<Agent> top10AgentsByOfferCount, string title)
        {
            System.Console.WriteLine("---------------------------------------------------------------------");
            System.Console.WriteLine(title);

            System.Console.WriteLine("{0,-5} {1,-50} {2,5}\n", "Rank", "Name", "Offer Count");
            for (var index = 0; index < top10AgentsByOfferCount.Count; index++)
            {
                var agent = top10AgentsByOfferCount[index];
                System.Console.WriteLine("{0,-5:N0} {1,-50} {2,5:N0}", index + 1, agent.Name, agent.OfferCount);
            }
        }
    }
}