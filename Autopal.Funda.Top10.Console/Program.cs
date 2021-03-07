using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NDesk.Options;

namespace Autopal.Funda.Top10.Console
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .SetupServices()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogInformation("Starting application");

            var showHelp = false;
            var region = string.Empty;
            var n = 10;
            var p = new OptionSet
            {
                {
                    "r|region=", "the {REGION} that will be investigated.",
                    v => region = v
                },
                {
                    "n|N=",
                    "the number of top {N} agents to retrieve.\n" +
                    "this must be an integer. Default is 10.",
                    (int v) => n = v
                },
                {
                    "h|help", "show this message and exit",
                    v => showHelp = v != null
                }
            };

            p.Parse(args);
            if (showHelp || string.IsNullOrEmpty(region))
            {
                ShowHelp(p);
                System.Console.ReadKey();
                return;
            }

            var serviceCaller = serviceProvider.GetRequiredService<ServiceCaller>();
            await serviceCaller.Call(region, false, n);

            await serviceCaller.Call(region, true, n);

            logger.LogInformation("All done!");

            System.Console.ReadKey();
        }

        private static void ShowHelp(OptionSet p)
        {
            System.Console.WriteLine("Usage: program [OPTIONS]");
            System.Console.WriteLine("Gets top 10 agents by their offer count.");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            p.WriteOptionDescriptions(System.Console.Out);
        }
    }
}