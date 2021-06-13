using System;
using System.IO;
using System.Reflection;

namespace libc.translation
{
    internal class EmbdRes
    {
        private readonly Assembly ass;

        public EmbdRes(Assembly ass)
        {
            this.ass = ass;
        }

        public string ReadAsString(string resourceId)
        {
            string res;

            using (var stream = ass.GetManifestResourceStream(resourceId))
            {
                using (var reader =
                    new StreamReader(stream ??
                                     throw new InvalidOperationException($"Could read {resourceId} as stream!")))
                {
                    res = reader.ReadToEnd();
                }
            }

            return res;
        }

        public byte[] ReadAsBinary(string resourceId)
        {
            byte[] res;

            using (var stream = ass.GetManifestResourceStream(resourceId))
            {
                res = new byte[stream.Length];
                stream.Read(res, 0, res.Length);
            }

            return res;
        }

        public void PutInFile(string resourceId, string filePath)
        {
            var data = ReadAsBinary(resourceId);
            File.WriteAllBytes(filePath, data);
        }
    }
}