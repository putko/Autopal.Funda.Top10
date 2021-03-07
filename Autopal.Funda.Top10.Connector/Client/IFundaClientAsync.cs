using System.Threading;
using System.Threading.Tasks;
using Autopal.Funda.Top10.Connector.Client.Model;
using Autopal.Funda.Top10.Connector.Model;

namespace Autopal.Funda.Top10.Connector.Client
{

    /// <summary>
    /// Async Funda Client definition.
    /// </summary>
    public interface IFundaClientAsync
    {
        #region Asynchronous Operations

        /// <summary>
        /// Gets the offers asynchronous.
        /// </summary>
        /// <param name="offerType">Type of the offer.</param>
        /// <param name="exteriorSpace">The exterior space.</param>
        /// <param name="region">The region.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<GetOffersResponse> GetOffersAsync(OfferType offerType, ExteriorSpaces? exteriorSpace, string region,
            int page, int pageSize, CancellationToken cancellationToken = default);

        #endregion Asynchronous Operations
    }
}