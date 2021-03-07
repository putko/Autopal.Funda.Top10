using System;
using System.IO;
using Autopal.Funda.Top10.Connector;
using Autopal.Funda.Top10.Connector.Client;
using Autopal.Funda.Top10.Connector.Settings;
using CVV;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autopal.Funda.Top10.Business.IntegrationTests
{
    public class ServiceProviderTestBase
    {
        private IServiceProvider ServiceProvider { get; set; }

        protected IServiceCollection ServiceCollection { get; }

        public ServiceProviderTestBase()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddLogging(opt =>
                {
                    opt.AddFilter((_, level) => level > LogLevel.Information);
                })
                .AddTransient<IStatsService, StatsService>()
                .AddTransient<IFundaClient, FundaClient>()
                .AddSingleton<IFundaConnector, FundaConnector>();


            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            var fundaServiceConfiguration = configuration.GetSection(FundaConnectionSettings.FundaServiceSectionName)
                .Get<FundaConnectionSettings>();
            ServiceCollection.AddSingleton(fundaServiceConfiguration);
            ServiceCollection.AddHttpClient<IFundaClient, FundaClient>(client =>
            {
                client.BaseAddress = new Uri(fundaServiceConfiguration.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            ServiceCollection.AddAutoMapper(typeof(IFundaConnector));

            var rateGate = new RateGate(fundaServiceConfiguration.RateLimit.CallCount,
                TimeSpan.FromSeconds(fundaServiceConfiguration.RateLimit.TimeSpanInSeconds));
            ServiceCollection.AddSingleton(rateGate);
        }

        protected T GetService<T>()
        {
            var serviceProvider = GetServiceProvider();
            return (T)serviceProvider.GetService(typeof(T));
        }

        protected IServiceProvider GetServiceProvider() => ServiceProvider ??= ServiceCollection.BuildServiceProvider();
    }
}