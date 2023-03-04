using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
	public class StorageQueueSettings
	{
		public string QueueName { get; set; }
		public string QueueConnectionString { get; set; }
		public string TenantId { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
	}
}
