namespace Autopal.Funda.Top10.Connector.Settings
{
    /// <summary>
    /// Funda Service connection setting definition.
    /// </summary>
    public class FundaConnectionSettings
    {
        /// <summary>
        ///     The name of the options section in the settings file.
        /// </summary>
        public const string FundaServiceSectionName = "FundaService";

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the page.
        /// </summary>
        /// <value>
        /// The maximum size of the page.
        /// </value>
        public int MaxPageSize { get; set; } = 25;

        /// <summary>
        /// Gets or sets the rate limit.
        /// </summary>
        /// <value>
        /// The rate limit.
        /// </value>
        public RateLimit RateLimit { get; set; }
    }
}