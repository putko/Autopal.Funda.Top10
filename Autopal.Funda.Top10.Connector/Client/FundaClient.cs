using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autopal.Funda.Top10.Connector.Client.Model;
using Autopal.Funda.Top10.Connector.Model;
using Autopal.Funda.Top10.Connector.Settings;
using Microsoft.Extensions.Logging;

namespace Autopal.Funda.Top10.Connector.Client
{
    /// <inheritdoc cref="IFundaClient" />
    public class FundaClient : ApiClientBase, IFundaClient
    {
        private readonly FundaConnectionSettings _fundaConnectionSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FundaClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="fundaConnectionSettings">The funda connection settings.</param>
        /// <param name="logger">The logger.</param>
        public FundaClient(HttpClient httpClient, FundaConnectionSettings fundaConnectionSettings,
            ILogger<FundaClient> logger) : base(httpClient, logger)
        {
            _fundaConnectionSettings = fundaConnectionSettings;
        }

        /// <inheritdoc />
        public GetOffersResponse GetOffers(OfferType offerType, ExteriorSpaces? exteriorSpace, string region, int page,
            int pageSize)
        {
            var task = Task.Run(async () =>
                await GetOffersAsync(offerType, exteriorSpace, region, page, pageSize, CancellationToken.None));
            return task.Result;
        }

        /// <inheritdoc />
        public virtual async Task<GetOffersResponse> GetOffersAsync(OfferType offerType, ExteriorSpaces? exteriorSpace,
            string region, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (_fundaConnectionSettings.Key == null)
                throw new ArgumentNullException(nameof(_fundaConnectionSettings.Key));

            var urlBuilder = new StringBuilder();
            urlBuilder.Append("feeds/Aanbod.svc/json/[key]/?");
            urlBuilder.Replace("[key]",
                Uri.EscapeDataString(ConvertToString(_fundaConnectionSettings.Key, CultureInfo.InvariantCulture)));
            urlBuilder.Append(Uri.EscapeDataString("type") + "=")
                .Append(Uri.EscapeDataString(
                    ConvertToString(offerType, CultureInfo.InvariantCulture).ToLowerInvariant())).Append("&");
            if (page > 0)
                urlBuilder.Append(Uri.EscapeDataString("page") + "=")
                    .Append(Uri.EscapeDataString(ConvertToString(page, CultureInfo.InvariantCulture))).Append("&");
            if (pageSize > 0)
                urlBuilder.Append(Uri.EscapeDataString("pageSize") + "=")
                    .Append(Uri.EscapeDataString(ConvertToString(pageSize, CultureInfo.InvariantCulture))).Append("&");
            if (exteriorSpace != null || !string.IsNullOrEmpty(region))
            {
                urlBuilder.Append(Uri.EscapeDataString("zo") + "=");
                if (!string.IsNullOrEmpty(region))
                    urlBuilder.Append(
                        Uri.EscapeDataString(
                            $"/{ConvertToString(region, CultureInfo.InvariantCulture).ToLowerInvariant()}"));
                if (exteriorSpace != null)
                    urlBuilder.Append(Uri.EscapeDataString(
                        $"/{ConvertToString(exteriorSpace, CultureInfo.InvariantCulture).ToLowerInvariant()}"));
                urlBuilder.Append("&");
            }

            urlBuilder.Length--;

            var client = HttpClient;
            using var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET")
            };
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            var url = urlBuilder.ToString();
            request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);
            Logger.LogInformation($"{request.Method} is made to the address {request.RequestUri}");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            Logger.LogInformation(
                $"Response with a Status code :{response.StatusCode} is retrieved for the {request.Method} request to the address {request.RequestUri}");

            try
            {
                var headers = response.Headers.ToDictionary(h => h.Key, h => h.Value);
                foreach (var item in response.Content.Headers)
                    headers[item.Key] = item.Value;

                var status = response.StatusCode;
                if (status == HttpStatusCode.OK)
                {
                    var objectResponse = await ReadObjectResponseAsync<GetOffersResponse>(response, headers)
                        .ConfigureAwait(false);
                    if (objectResponse.Object != null) return objectResponse.Object;
                    const string message = "Response was null which was not expected.";
                    Logger.LogError(
                        $"{message} for the {request.Method} request to the address {request.RequestUri}");

                    throw new ApiException(message, status, objectResponse.Text, headers, null);
                }
                else
                {
                    var objectResponse = await ReadObjectResponseAsync<object>(response, headers).ConfigureAwait(false);
                    if (objectResponse.Object != null)
                        throw new ApiException<object>("Error", status, objectResponse.Text, headers,
                            objectResponse.Object,
                            null);
                    const string message = "Response was null which was not expected.";
                    Logger.LogError(
                        $"{message} for the {request.Method} request to the address {request.RequestUri}");

                    throw new ApiException(message, status, objectResponse.Text, headers, null);
                }
            }
            finally
            {
                response.Dispose();
            }
        }
    }
}