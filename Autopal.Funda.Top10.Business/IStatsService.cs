using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autopal.Funda.Top10.Business.Model;

namespace Autopal.Funda.Top10.Business
{
    /// <summary>
    ///   <para>Definition for StatsService</para>
    /// </summary>
    public interface IStatsService
    {
        /// <summary>Gets the top n agents by offer count.</summary>
        /// <param name="region">The region in which the search will be made.</param>
        /// <param name="isWithGarden">if set to <c>true</c> gets only the houses that has garden.</param>
        /// <param name="n">The number of top agents to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   List of top agents with most offer count.
        /// </returns>
        Task<List<Agent>> GetTopNAgentsByOfferCount(string region, bool isWithGarden, int n = 10,
            CancellationToken cancellationToken = default);
    }
}