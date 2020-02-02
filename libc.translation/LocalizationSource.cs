using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
namespace libc.translation {
    public class LocalizationSource : ILocalizationSource {
        private readonly JObject root;
        public LocalizationSource(JObject root) {
            this.root = root;
        }
        /// <summary>
        /// Load localization texts from and embedded json file
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceId"></param>
        public LocalizationSource(Assembly assembly, string resourceId) {
            var em = new EmbdRes(assembly);
            root = JObject.Parse(em.ReadAsString(resourceId));
        }
        /// <summary>
        /// Load localization texts from an <see cref="Stream"/>
        /// </summary>
        /// <param name="stream"></param>
        public LocalizationSource(Stream stream) {
            using (var sr = new StreamReader(stream)) {
                root = JObject.Parse(sr.ReadToEnd());
            }
        }
        /// <summary>
        /// Load localization texts from a json file residing on the disk
        /// </summary>
        /// <param name="file"></param>
        public LocalizationSource(FileInfo file) {
            root = JObject.Parse(File.ReadAllText(file.FullName));
        }
        /// <inheritdoc />
        public string Get(string culture, string key) {
            if (!root.ContainsKey(culture)) return null;
            var j = (JObject) root[culture];
            if (!j.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var res)) return null;
            return res.Value<string>();
        }
        /// <inheritdoc />
        public Dictionary<string, string> GetAll(string culture) {
            if (!root.ContainsKey(culture)) return null;
            var j = (IDictionary<string, JToken>) root[culture];
            return j.ToDictionary(a => a.Key, a => a.Value.Value<string>());
        }
    }
}