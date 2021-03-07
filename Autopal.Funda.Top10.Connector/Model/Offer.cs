namespace Autopal.Funda.Top10.Connector.Model
{
    /// <summary>
    /// Offer definition.
    /// </summary>
    public class Offer
    {
        /// <summary>
        /// Gets or sets the agent identifier.
        /// </summary>
        /// <value>
        /// The agent identifier.
        /// </value>
        public int AgentId { get; set; }
        /// <summary>
        /// Gets or sets the name of the agent.
        /// </summary>
        /// <value>
        /// The name of the agent.
        /// </value>
        public string AgentName { get; set; }
        /// <summary>
        /// Gets or sets the type of the house.
        /// </summary>
        /// <value>
        /// The type of the house.
        /// </value>
        public string HouseType { get; set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }
    }
}