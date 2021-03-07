using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Autopal.Funda.Top10.Connector.Client;
using Autopal.Funda.Top10.Connector.Client.Model;
using Autopal.Funda.Top10.Connector.Model;
using CVV;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Autopal.Funda.Top10.Connector
{
    /// <inheritdoc />
    public class FundaConnector : IFundaConnector
    {
        private readonly ILogger<IFundaConnector> _logger;
        private readonly IMapper _mapper;
        private readonly RateGate _rateGate;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IFundaClient _serviceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FundaConnector"/> class.
        /// </summary>
        /// <param name="serviceClient">The service client.</param>
        /// <param name="rateGate">The rate gate.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public FundaConnector(IFundaClient serviceClient, RateGate rateGate, IMapper mapper,
            ILogger<IFundaConnector> logger)
        {
            _serviceClient = serviceClient;
            _rateGate = rateGate;
            _mapper = mapper;
            _logger = logger;
            _retryPolicy = GetRetryPolicy();
        }

        /// <inheritdoc />
        public async Task<OffersResponse> GetOffersAsync(OfferType offerType, ExteriorSpaces? exteriorSpace,
            string region, int page, int pageSize, CancellationToken cancellationToken)
        {
            var offerResponse = await _retryPolicy.ExecuteAsync(() =>
                CallGetOffersAsync(offerType, exteriorSpace, region, page, pageSize, cancellationToken));

            if (offerResponse == null)
                _logger.LogWarning(
                    $"No offers retrieved in page {page} in region: {region} for offerType: {offerType}, exterior space:{exteriorSpace} ");
            else
                _logger.LogInformation(
                    $"{offerResponse.Objects.Count} offers retrieved in page {page} in region: {region} for offerType: {offerType}, exterior space:{exteriorSpace} ");

            var result = _mapper.Map<OffersResponse>(offerResponse);
            return result;
        }

        private async Task<GetOffersResponse> CallGetOffersAsync(OfferType offerType, ExteriorSpaces? exteriorSpace,
            string region, int page, int pageSize, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Waiting for call slot...");
            _rateGate.WaitToProceed();
            _logger.LogDebug("Call slot obtained!");
            return await _serviceClient.GetOffersAsync(offerType, exteriorSpace, region, page, pageSize,
                cancellationToken);
        }

        private AsyncRetryPolicy GetRetryPolicy()
        {
            return Policy
                .Handle<ApiException>(a => a.StatusCode == (HttpStatusCode) 429)
                .WaitAndRetryAsync(10, // Retry 10 times with a delay between retries before ultimately giving up
                    attempt => TimeSpan.FromSeconds(0.25 * Math.Pow(2,
                        attempt)), // Back off!  2, 4, 8, 16 etc times 1/4-second
                    //attempt => TimeSpan.FromSeconds(6), // Wait 6 seconds between retries
                    (exception, calculatedWaitDuration) =>
                    {
                        _logger.LogInformation(
                            $"Funda Aanbod API server is throttling our requests. Automatically delaying for {calculatedWaitDuration.TotalMilliseconds}ms");
                    }
                );
        }
    }
}