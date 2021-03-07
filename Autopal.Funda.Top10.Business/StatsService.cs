using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autopal.Funda.Top10.Business.Model;
using Autopal.Funda.Top10.Connector;
using Autopal.Funda.Top10.Connector.Client;
using Autopal.Funda.Top10.Connector.Model;
using Autopal.Funda.Top10.Connector.Settings;
using Microsoft.Extensions.Logging;

namespace Autopal.Funda.Top10.Business
{
    /// <inheritdoc />
    public class StatsService : IStatsService
    {
        private readonly IFundaConnector _connector;
        private readonly FundaConnectionSettings _fundaConnectionSettings;
        private readonly ILogger<StatsService> _logger;

        /// <summary>Initializes a new instance of the <see cref="StatsService" /> class.</summary>
        /// <param name="connector">The connector.</param>
        /// <param name="fundaConnectionSettings">The funda connection settings.</param>
        /// <param name="logger">The logger.</param>
        public StatsService(IFundaConnector connector, FundaConnectionSettings fundaConnectionSettings,
            ILogger<StatsService> logger)
        {
            _connector = connector;
            _fundaConnectionSettings = fundaConnectionSettings;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<List<Agent>> GetTopNAgentsByOfferCount(string region, bool isWithGarden, int n = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ExteriorSpaces? exteriorSpace = isWithGarden ? ExteriorSpaces.Tuin : null;
                if (n < 1)
                    throw new ArgumentOutOfRangeException(nameof(n));

                if (string.IsNullOrEmpty(region))
                    throw new ArgumentNullException(nameof(region));

                var offerResponse = await _connector.GetOffersAsync(OfferType.Koop, exteriorSpace, region, 1,
                    _fundaConnectionSettings.MaxPageSize, cancellationToken);
                if (offerResponse?.Offers == null)
                    throw new InvalidOperationException("Offers could not be able to retrieved!");
                var offers = offerResponse.Offers.ToList();

                //Parallel.For(2, offerResponse.TotalPage + 1, new ParallelOptions { MaxDegreeOfParallelism = this._fundaConnectionSettings.RateLimit.CallCount }, async i =>
                // {
                for (var i = 2; i < offerResponse.TotalPage + 1; i++)
                {
                    var results = await _connector.GetOffersAsync(OfferType.Koop, exteriorSpace, region, i,
                        _fundaConnectionSettings.MaxPageSize, cancellationToken);
                    if (results?.Offers == null)
                        throw new InvalidOperationException("Offers could not be able to retrieved!");
                    offers.AddRange(results.Offers);
                }

                //});
                var topAgents = offers
                    .GroupBy(offer => offer.AgentId)
                    .Select(grouping => new Agent
                    {
                        Id = grouping.Key,
                        Name = grouping.First().AgentName,
                        OfferCount = grouping.Count()
                    }
                    )
                    .OrderByDescending(agent => agent.OfferCount)
                    .Take(n);
                return topAgents.ToList();
            }
            catch (ApiException apiException)
            {
                _logger.LogError($"An error occurred while connecting to Funda API. Message:{apiException.Message}");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred. Message:{e.Message}");
                throw;
            }
        }
    }
}