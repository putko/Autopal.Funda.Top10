using System.Collections.Generic;

namespace Autopal.Funda.Top10.Connector.Model
{
    /// <summary>
    ///     Offer response definition.
    /// </summary>
    public class OffersResponse
    {
        /// <summary>
        ///     Gets or sets the total count.
        /// </summary>
        /// <value>
        ///     The total count.
        /// </value>
        public int TotalCount { get; set; }

        /// <summary>
        ///     Gets or sets the offers.
        /// </summary>
        /// <value>
        ///     The offers.
        /// </value>
        public IList<Offer> Offers { get; set; }

        /// <summary>
        ///     Gets or sets the total page.
        /// </summary>
        /// <value>
        ///     The total page.
        /// </value>
        public int TotalPage { get; set; }
    }
}