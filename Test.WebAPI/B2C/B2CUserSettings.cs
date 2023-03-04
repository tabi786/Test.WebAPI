using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
  public class B2CUserSettings
  {
    public string tenant { get; set; }
    public string clientId { get; set; }
    public string clientSecret { get; set; }
    public string B2cExtensionAppClientId { get; set; }
    public string BlobLoginConnectionString { get; set; }
    //public string QueueConnectionString { get; set; }
    public string BlobLoginAccountName { get; set; }
		public string BlobUsersConnectionString { get; set; }
		public string BlobContainerProfilePics { get; set; }
		public string BlobContainerUserDocs { get; set; }


	}
}
