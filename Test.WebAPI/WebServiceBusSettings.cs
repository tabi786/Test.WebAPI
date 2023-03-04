using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
	public class WebServiceBusSettings { 
		public string NotificationQueueName { get; set; }
		public string NotificationConnectionString { get; set; }
		public string UserAnncesstorQueueName { get; set; } 
		public string UserAnncesstorConnectionString { get; set; } 
		public string TradingQueueName { get; set; }
		public string TradingConnectionString { get; set; } 
	
	}
}
