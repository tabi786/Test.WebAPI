using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
	public class ServiceBusQueueSettings
	{
		public string TradingQueueName { get; set; }
		public string TradingConnectionString { get; set; }
		//public string TenantId { get; set; }
		//public string ClientId { get; set; }
		//public string ClientSecret { get; set; }
	}

}
