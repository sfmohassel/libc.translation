using System;
using System.IO;
using System.Reflection;

namespace libc.translation
{
  internal class EmbeddedResource
  {
    private readonly Assembly _assembly;

    public EmbeddedResource(Assembly assembly)
    {
      _assembly = assembly;
    }

    public string ReadAsString(string resourceId)
    {
      using var stream = _assembly.GetManifestResourceStream(resourceId);
      if (stream == null)
        throw new InvalidOperationException("Could read " + resourceId + " as stream!");
      using var streamReader = new StreamReader(stream);
      return streamReader.ReadToEnd();
    }

    public byte[] ReadAsBinary(string resourceId)
    {
      using var stream = _assembly.GetManifestResourceStream(resourceId);
      if (stream == null)
        throw new InvalidOperationException("Could read " + resourceId + " as stream!");
      using var ms = new MemoryStream();
      stream.CopyTo(ms);
      return ms.ToArray();
    }

    public void PutInFile(string resourceId, string filePath)
    {
      var data = ReadAsBinary(resourceId);
      File.WriteAllBytes(filePath, data);
    }
  }
}