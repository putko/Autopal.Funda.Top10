using Autopal.Funda.Top10.Connector.Client.Model;
using Autopal.Funda.Top10.Connector.Model;

namespace Autopal.Funda.Top10.Connector.Client
{

    /// <summary>
    /// Sync funda client definition.
    /// </summary>
    public interface IFundaClientSync
    {
        #region Synchronous Operations

        /// <summary>
        /// Gets the offers.
        /// </summary>
        /// <param name="offerType">Type of the offer.</param>
        /// <param name="exteriorSpace">The exterior space.</param>
        /// <param name="region">The region.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>Calls the async method and waits the result and returns it.</returns>
        GetOffersResponse GetOffers(OfferType offerType, ExteriorSpaces? exteriorSpace, string region, int page,
            int pageSize);

        #endregion Synchronous Operations
    }
}