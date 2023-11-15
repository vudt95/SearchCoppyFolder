using System;
using NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace SearchCoppyFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var servicesProvider = BuildDi(config);
                using (servicesProvider as IDisposable)
                {
                    Console.Clear();
                    Console.WriteLine("Choose an option:");
                    Console.WriteLine("1) Scanner folder and coppy");
                    Console.WriteLine("2) Scanner folder not found");
                    Console.WriteLine("3) Scanner read name file");
                    Console.WriteLine("4) Scanner coppy all file");
                    Console.WriteLine("5) Scanner and coppy all file with search file");
                    Console.WriteLine("6) Exit");
                    Console.Write("\r\nSelect an option: ");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            var runner = servicesProvider.GetRequiredService<Runner>();
                            runner.DoAction("Scanner folder and coppy", true);
                            break;
                        case "2":
                            var runner1 = servicesProvider.GetRequiredService<Runner>();
                            runner1.DoAction("Scanner folder not found");
                            break;
                        case "3":
                            var runner2 = servicesProvider.GetRequiredService<Runner>();
                            runner2.ReadNameAllFiles("Scanner read name file");
                            break;
                        case "4":
                            var runner3 = servicesProvider.GetRequiredService<Runner>();
                            runner3.CoppyAllFile("Scanner coppy all file");
                            break;
                        case "5":
                            var runner4 = servicesProvider.GetRequiredService<Runner>();
                            runner4.CoppyFolderFile("Scanner and coppy all file with search file");
                            break;

                    }
                    Console.WriteLine("Press ANY key to exit");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }
        private static IServiceProvider BuildDi(IConfiguration config)
        {
            return new ServiceCollection()
                .AddTransient<Runner>() // Runner is the custom class
                .AddLogging(loggingBuilder =>
                {
                    // configure Logging with NLog
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuilder.AddNLog(config);
                })
                .BuildServiceProvider();
        }
    }
}
