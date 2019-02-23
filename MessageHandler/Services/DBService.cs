using Cassandra;
using System;
using System.Linq;
using MessageHandler.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MessageHandler.Settings;

namespace MessageHandler.Services
{
	class DBService : IDBService
	{
		private readonly ILogger<DBService> _logger;
		private readonly AppConfig _appConfig;
		public Cluster cluster;
		public ISession session;

		public DBService(ILogger<DBService> logger, IOptions<AppConfig> appConfig)
		{
			_logger = logger;
			_appConfig = appConfig.Value;

			cluster = Cluster.Builder().AddContactPoint(_appConfig.DBHost).Build();
			session = cluster.Connect(_appConfig.DBKeyspace);
		}

		public bool InsertMessage(Message message)
		{
			
			try
			{
				var InsertStatement = session.Prepare("INSERT INTO MESSAGES(DeviceID, TimeStamp, Status, Message) VALUES (DeviceId = ?, TimeStamp = ?, Status = ?, message = ?");
				var rs = session.Execute(InsertStatement.Bind(message.DeviceId, message.TimeStamp, message.Status.ToString(), message.message));
				var row = rs.FirstOrDefault();

				return (row != null ? true : false);
			}
			catch (Exception ex)
			{
				_logger.LogInformation("Database Exception: " + ex.Message);
			}

			return false;
		}
	}

	 interface IDBService
	{
		bool InsertMessage(Message message);
	}
}
