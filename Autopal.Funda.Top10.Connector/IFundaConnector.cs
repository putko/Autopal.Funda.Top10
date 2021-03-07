using System.Threading;
using System.Threading.Tasks;
using Autopal.Funda.Top10.Connector.Model;

namespace Autopal.Funda.Top10.Connector
{
    /// <summary>
    /// Funda Connector definition.
    /// </summary>
    public interface IFundaConnector
    {
        /// <summary>
        /// Gets the offers asynchronous.
        /// </summary>
        /// <param name="offerType">Type of the offer.</param>
        /// <param name="exteriorSpace">The exterior space.</param>
        /// <param name="region">The region.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Offers in given page and information about total objects.</returns>
        Task<OffersResponse> GetOffersAsync(OfferType offerType, ExteriorSpaces? exteriorSpace, string region, int page,
            int pageSize, CancellationToken cancellationToken = default);
    }
}