﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
  internal class B2cCustomAttributeHelper
  {
    internal readonly string _b2cExtensionAppClientId;

    internal B2cCustomAttributeHelper(string b2cExtensionAppClientId)
    {
      _b2cExtensionAppClientId = b2cExtensionAppClientId.Replace("-", "");
    }

    internal string GetCompleteAttributeName(string attributeName)
    {
      if (string.IsNullOrWhiteSpace(attributeName))
      {
        throw new System.ArgumentException("Parameter cannot be null", nameof(attributeName));
      }

      return $"extension_{_b2cExtensionAppClientId}_{attributeName}";
    }
  }
}
