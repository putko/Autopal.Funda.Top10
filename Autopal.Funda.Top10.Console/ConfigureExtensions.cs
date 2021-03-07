using System;
using System.IO;
using Autopal.Funda.Top10.Business;
using Autopal.Funda.Top10.Connector;
using Autopal.Funda.Top10.Connector.Client;
using Autopal.Funda.Top10.Connector.Settings;
using CVV;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autopal.Funda.Top10.Console
{
    public static class ConfigureExtensions
    {
        public static IServiceCollection SetupServices(this IServiceCollection services)
        {
            services.AddLogging(opt =>
                {
                    opt.AddFilter((_, level) => level > LogLevel.Information);
                    opt.AddConsole();
                })
                .AddTransient<IStatsService, StatsService>()
                .AddTransient<IFundaClient, FundaClient>()
                .AddSingleton<IFundaConnector, FundaConnector>()
                .AddTransient<ServiceCaller>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            var fundaServiceConfiguration = configuration.GetSection(FundaConnectionSettings.FundaServiceSectionName)
                .Get<FundaConnectionSettings>();
            services.AddSingleton(fundaServiceConfiguration);
            services.AddHttpClient<IFundaClient, FundaClient>(client =>
            {
                client.BaseAddress = new Uri(fundaServiceConfiguration.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddAutoMapper(typeof(IFundaConnector));

            var rateGate = new RateGate(fundaServiceConfiguration.RateLimit.CallCount,
                TimeSpan.FromSeconds(fundaServiceConfiguration.RateLimit.TimeSpanInSeconds));
            services.AddSingleton(rateGate);
            return services;
        }
    }
}