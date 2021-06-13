using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace libc.translation
{
    public class LocalizationSource : ILocalizationSource
    {
        private readonly LocalizationSourcePropertyCaseSensitivity caseSensitivity;
        private readonly JsonElement root;

        /// <summary>
        /// </summary>
        /// <param name="jsonDocument">The json document. You freely can dispose json document after calling this constructor.</param>
        /// <param name="caseSensitivity">Case sensitivity when searching for property names</param>
        public LocalizationSource(JsonDocument jsonDocument, LocalizationSourcePropertyCaseSensitivity caseSensitivity)
        {
            this.caseSensitivity = caseSensitivity;
            root = getRootElement(jsonDocument);
        }

        /// <summary>
        ///     Load localization texts from and embedded json file
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceId"></param>
        /// <param name="caseSensitivity">Case sensitivity when searching for property names</param>
        /// <param name="jsonMaxDepth">
        ///     <see cref="JsonDocumentOptions.MaxDepth" />
        /// </param>
        public LocalizationSource(Assembly assembly, string resourceId,
            LocalizationSourcePropertyCaseSensitivity caseSensitivity,
            int jsonMaxDepth = 128)
        {
            this.caseSensitivity = caseSensitivity;
            var em = new EmbdRes(assembly);
            var json = em.ReadAsString(resourceId);
            root = toRootElement(json, jsonMaxDepth);
        }

        /// <summary>
        ///     Load localization texts from an <see cref="Stream" />
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="caseSensitivity">Case sensitivity when searching for property names</param>
        /// <param name="jsonMaxDepth">
        ///     <see cref="JsonDocumentOptions.MaxDepth" />
        /// </param>
        public LocalizationSource(Stream stream, LocalizationSourcePropertyCaseSensitivity caseSensitivity,
            int jsonMaxDepth = 128)
        {
            this.caseSensitivity = caseSensitivity;
            string json;

            using (var sr = new StreamReader(stream))
            {
                json = sr.ReadToEnd();
            }

            root = toRootElement(json, jsonMaxDepth);
        }

        /// <summary>
        ///     Load localization texts from a json file residing on the disk
        /// </summary>
        /// <param name="file"></param>
        /// <param name="caseSensitivity">Case sensitivity when searching for property names</param>
        /// <param name="jsonMaxDepth">
        ///     <see cref="JsonDocumentOptions.MaxDepth" />
        /// </param>
        public LocalizationSource(FileInfo file, LocalizationSourcePropertyCaseSensitivity caseSensitivity,
            int jsonMaxDepth = 128)
        {
            this.caseSensitivity = caseSensitivity;
            var json = File.ReadAllText(file.FullName);
            root = toRootElement(json, jsonMaxDepth);
        }

        /// <inheritdoc />
        public string Get(string culture, string key)
        {
            // try to find {culture} element inside root
            if (!tryFindCultureElement(culture, out var cultureElement)) return null;

            if (caseSensitivity == LocalizationSourcePropertyCaseSensitivity.CaseInsensitive)
                return getCaseInsensitive(cultureElement, key);

            return getCaseSensitive(cultureElement, key);
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetAll(string culture)
        {
            // try to find {culture} element inside root
            if (!tryFindCultureElement(culture, out var cultureElement)) return null;
            var res = new Dictionary<string, string>();
            foreach (var x in cultureElement.EnumerateObject()) res[x.Name] = x.Value.GetString();

            return res;
        }

        private static JsonElement getRootElement(JsonDocument jsonDocument)
        {
            // see: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to#jsondocument-is-idisposable
            // to understand the following line
            return jsonDocument.RootElement.Clone();
        }

        private JsonElement toRootElement(string json, int jsonMaxDepth)
        {
            var doc = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
                MaxDepth = jsonMaxDepth
            });

            using (doc)
            {
                return getRootElement(doc);
            }
        }

        private bool tryFindCultureElement(string culture, out JsonElement cultureElement)
        {
            cultureElement = default;

            if (caseSensitivity == LocalizationSourcePropertyCaseSensitivity.CaseInsensitive)
            {
                foreach (var x in root.EnumerateObject())
                    if (x.Name.Equals(culture, StringComparison.OrdinalIgnoreCase))
                    {
                        cultureElement = root.GetProperty(x.Name);

                        return true;
                    }

                return false;
            }

            return root.TryGetProperty(culture, out cultureElement);
        }

        private static string getCaseSensitive(JsonElement cultureElement, string key)
        {
            return !cultureElement.TryGetProperty(key, out var res) ? null : res.GetString();
        }

        private static string getCaseInsensitive(JsonElement cultureElement, string key)
        {
            foreach (var x in cultureElement.EnumerateObject())
                if (x.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    var res = cultureElement.GetProperty(x.Name);

                    return res.GetString();
                }

            return null;
        }
    }

    public enum LocalizationSourcePropertyCaseSensitivity
    {
        CaseSensitive,
        CaseInsensitive
    }
}