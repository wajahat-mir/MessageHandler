using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHandler.Settings
{
	class AppConfig
	{
		public string BrokerHost { get; set; }
		public string BrokerUserName { get; set; }
		public string BrokerPassword { get; set; }
		public string QueueName { get; set; }
		public string DBHost { get; set; }
		public string DBKeyspace { get; set; }
	}
}
