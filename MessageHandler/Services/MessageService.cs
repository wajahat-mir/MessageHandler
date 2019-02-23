using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MessageHandler.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using MessageHandler.Settings;
using Microsoft.Extensions.Options;

namespace MessageHandler.Services
{
	class MessageService : IMessageService
	{
		private readonly ILogger<MessageService> _logger;
		private readonly AppConfig _appConfig;
		private readonly IDBService _dbService;

		public MessageService(ILogger<MessageService> logger, IDBService dbService, IOptions<AppConfig> appConfig)
		{
			_logger = logger;
			_dbService = dbService;
			_appConfig = appConfig.Value;
		}

		public void SaveMessages()
		{
			var factory = new ConnectionFactory() {
				HostName = _appConfig.BrokerHost,
				UserName = _appConfig.BrokerUserName,
				Password = _appConfig.BrokerPassword
			};

			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare(queue: _appConfig.QueueName,
									 durable: true,
									 exclusive: false,
									 autoDelete: false,
									 arguments: null);

					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += (model, ea) =>
					{
						try
						{
							var body = ea.Body;
							var input = Encoding.UTF8.GetString(body);
							Message msg = JsonConvert.DeserializeObject<Message>(input);

							// save message to DB
							_dbService.InsertMessage(msg);

							channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
						}
						catch (JsonException je)
						{
							_logger.LogInformation("Failed to Deserialize JSON message: " + je.Message);
						}

					};

					channel.BasicConsume(queue: _appConfig.QueueName,
								 autoAck: true,
								 consumer: consumer);
				}
			}
		}
	}

	public interface IMessageService
	{
		void SaveMessages();
	}
}
