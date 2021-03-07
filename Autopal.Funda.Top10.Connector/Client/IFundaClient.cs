using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;

namespace Autopal.Funda.Top10.Connector.Client
{
    /// <summary>
    ///     Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IFundaClient : IFundaClientSync, IFundaClientAsync
    {
    }

    [GeneratedCode("NSwag", "13.10.6.0 (NJsonSchema v10.3.8.0 (Newtonsoft.Json v12.0.0.2))")]
    public class ApiException<TResult> : ApiException
    {
        public ApiException(string message, HttpStatusCode statusCode, string response,
            IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }

        public TResult Result { get; }
    }
}