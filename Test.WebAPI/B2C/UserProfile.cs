using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Test.WebAPI
{
  public class UserProfile
  {
    [Key]
    public int ID { get; set; }
    public string Email { get; set; }
    public string lang { get; set; }
    public string theme { get; set; }
  }
}
