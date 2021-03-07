namespace Autopal.Funda.Top10.Connector.Settings
{
    /// <summary>
    /// Rate limit definition.
    /// </summary>
    public class RateLimit
    {
        /// <summary>
        /// Gets or sets the call count.
        /// </summary>
        /// <value>
        /// The call count.
        /// </value>
        public int CallCount { get; set; }
        
        /// <summary>
        /// Gets or sets the time span in seconds.
        /// </summary>
        /// <value>
        /// The time span in seconds.
        /// </value>
        public int TimeSpanInSeconds { get; set; }
    }
}