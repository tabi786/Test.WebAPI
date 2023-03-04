using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
  public class Result
  {
    public dynamic Value { set; get; }

    public string Message { set; get; }

    public string Error { set; get; }

    public bool IsSucess { set; get; }
  }
  public class Config
  {
    public const string EmailMobileNumberAlreadyExists = "EmailMobileNumberAlreadyExists";
    public const string MobileNumberAlreadyExists = "MobileNumberAlreadyExists";
    public const string EmailAlreadyExists = "EmailAlreadyExists";
    public const string NotExists = "NotExists";
  }
}
