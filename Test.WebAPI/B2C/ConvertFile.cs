using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WebAPI
{
  public static class ConvertFile
  {
    public static byte[] GetBinaryFile(string filename)
    {
      byte[] bytes;
      using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
      {
        bytes = new byte[file.Length];
        file.Read(bytes, 0, (int)file.Length);
      }
      return bytes;
    }
  }
}
