using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
  public interface IUserProfileSettings
  {
    Task AddUserSetting(UserProfile model);
    UserProfile getUserSetting(string email);
  }
}
