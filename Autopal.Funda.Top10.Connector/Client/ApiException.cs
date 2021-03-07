using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;

namespace Autopal.Funda.Top10.Connector.Client
{
    [GeneratedCode("NSwag", "13.10.6.0 (NJsonSchema v10.3.8.0 (Newtonsoft.Json v12.0.0.2))")]
    public class ApiException : Exception
    {
        public ApiException(string message, HttpStatusCode statusCode, string response,
            IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
            : base(
                message + "\n\nStatus: " + statusCode + "\nResponse: \n" + (response == null
                    ? "(null)"
                    : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public HttpStatusCode StatusCode { get; }

        public string Response { get; }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

        public override string ToString()
        {
            return $"HTTP Response: \n\n{Response}\n\n{base.ToString()}";
        }
    }
}