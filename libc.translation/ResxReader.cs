using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace libc.translation
{
  /// <summary>
  ///     This can be used to parse a .resx file which only has text values
  /// </summary>
  public class ResxReader
  {
    public static IDictionary<string, string> Read(FileInfo file)
    {
      var res = new Dictionary<string, string>();
      var doc = XDocument.Load(file.FullName);

      if (doc.Root != null)
      {
        var nodes = doc.Root.Elements("data");

        foreach (var node in nodes)
          res[node.Attribute("name")?.Value ?? throw new Exception()] = node.Element("value")?.Value;
      }

      return res;
    }
  }
}