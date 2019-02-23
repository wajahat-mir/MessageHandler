using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MessageHandler.Services;
using System.IO;
using Microsoft.Extensions.Configuration;
using MessageHandler.Settings;

namespace MessageHandler
{
	class Program
	{
		static void Main(string[] args)
		{
			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);

			

			var serviceProvider = serviceCollection.BuildServiceProvider();

			var logger = serviceProvider.GetService<ILoggerFactory>()
				.CreateLogger<Program>();
			logger.LogDebug("Starting application");

			serviceProvider.GetService<IMessageService>().SaveMessages();

			Console.WriteLine(" Rabbit MQ Consumer running");
			Console.WriteLine(" Press [enter] to exit.");
			Console.ReadLine();
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			var builder = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			 .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false);

			var configuration = builder.Build();

			services.AddOptions();
			services.Configure<AppConfig>(configuration.GetSection(nameof(AppConfig)));
			services.AddLogging(configure => configure.AddConsole());
			services.AddSingleton<IMessageService, MessageService>();
			services.AddSingleton<IDBService, DBService>();
		} 

	}
}
