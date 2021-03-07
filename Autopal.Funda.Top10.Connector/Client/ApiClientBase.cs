using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Autopal.Funda.Top10.Connector.Client
{
    public abstract class ApiClientBase
    {
        protected readonly HttpClient HttpClient;
        protected readonly ILogger<ApiClientBase> Logger;
        protected readonly Lazy<JsonSerializerSettings> Settings;

        protected ApiClientBase(HttpClient httpClient, ILogger<ApiClientBase> logger)
        {
            HttpClient = httpClient;
            Logger = logger;
            Settings = new Lazy<JsonSerializerSettings>(CreateSerializerSettings);
        }

        protected JsonSerializerSettings JsonSerializerSettings => Settings.Value;
        public bool ReadResponseAsString { get; set; }

        protected JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            return settings;
        }

        protected static string ConvertToString(object value, CultureInfo cultureInfo)
        {
            switch (value)
            {
                case null:
                    return string.Empty;
                case Enum:
                {
                    var name = Enum.GetName(value.GetType(), value);
                    if (name == null) return null;
                    var field = value.GetType().GetTypeInfo().GetDeclaredField(name);
                    if (field == null) return name;
                    if (field.GetCustomAttribute(typeof(EnumMemberAttribute)) is
                        EnumMemberAttribute attribute) return attribute.Value ?? name;
                    return name;
                }
                case bool b:
                    return Convert.ToString(b, cultureInfo).ToLowerInvariant();
                case byte[] bytes:
                    return Convert.ToBase64String(bytes);
                default:
                {
                    if (value.GetType().IsArray)
                    {
                        var array = ((Array) value).OfType<object>();
                        return string.Join(",", array.Select(o => ConvertToString(o, cultureInfo)));
                    }

                    break;
                }
            }

            var result = Convert.ToString(value, cultureInfo);
            return result ?? string.Empty;
        }

        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response,
            IReadOnlyDictionary<string, IEnumerable<string>> headers)
        {
            if (response == null) return new ObjectResponseResult<T>(default, string.Empty);

            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText, JsonSerializerSettings);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, response.StatusCode, responseText, headers, exception);
                }
            }

            try
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync()
                    .ConfigureAwait(false);
                using var streamReader = new StreamReader(responseStream);
                using var jsonTextReader = new JsonTextReader(streamReader);
                var serializer = JsonSerializer.Create(JsonSerializerSettings);
                var typedBody = serializer.Deserialize<T>(jsonTextReader);
                return new ObjectResponseResult<T>(typedBody, string.Empty);
            }
            catch (JsonException exception)
            {
                var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                throw new ApiException(message, response.StatusCode, string.Empty, headers, exception);
            }
        }

        protected readonly struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                Object = responseObject;
                Text = responseText;
            }

            public T Object { get; }

            public string Text { get; }
        }
    }
}