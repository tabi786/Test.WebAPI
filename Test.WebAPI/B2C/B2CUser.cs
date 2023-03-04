using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
  public class B2CUser
  {
    public string AadOjbectId { set; get; }
    public string GivenName { get; set; }
    public string Surname { get; set; }
    public string AlternateEmail { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string PostalCode { get; set; }
    public string City { set; get; }
    public string Country { get; set; }
  }
}
