using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace libc.translation
{
  public class JsonLocalizationSource : ILocalizationSource
  {
    private readonly PropertyCaseSensitivity _caseSensitivity;
    private readonly JsonElement _root;

    /// <summary>
    /// </summary>
    /// <param name="jsonDocument">The json document. You freely can dispose json document after calling this constructor.</param>
    /// <param name="caseSensitivity">Case sensitivity when searching for property names</param>
    public JsonLocalizationSource(JsonDocument jsonDocument, PropertyCaseSensitivity caseSensitivity)
    {
      this._caseSensitivity = caseSensitivity;
      _root = GetRootElement(jsonDocument);
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
    public JsonLocalizationSource(Assembly assembly, string resourceId, PropertyCaseSensitivity caseSensitivity,
        int jsonMaxDepth = 128)
    {
      this._caseSensitivity = caseSensitivity;
      var em = new EmbeddedResource(assembly);
      var json = em.ReadAsString(resourceId);
      _root = ToRootElement(json, jsonMaxDepth);
    }

    /// <summary>
    ///     Load localization texts from an <see cref="Stream" />
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="caseSensitivity">Case sensitivity when searching for property names</param>
    /// <param name="jsonMaxDepth">
    ///     <see cref="JsonDocumentOptions.MaxDepth" />
    /// </param>
    public JsonLocalizationSource(Stream stream, PropertyCaseSensitivity caseSensitivity, int jsonMaxDepth = 128)
    {
      this._caseSensitivity = caseSensitivity;
      string json;

      using (var sr = new StreamReader(stream))
      {
        json = sr.ReadToEnd();
      }

      _root = ToRootElement(json, jsonMaxDepth);
    }

    /// <summary>
    ///     Load localization texts from a json file residing on the disk
    /// </summary>
    /// <param name="file"></param>
    /// <param name="caseSensitivity">Case sensitivity when searching for property names</param>
    /// <param name="jsonMaxDepth">
    ///     <see cref="JsonDocumentOptions.MaxDepth" />
    /// </param>
    public JsonLocalizationSource(FileInfo file, PropertyCaseSensitivity caseSensitivity, int jsonMaxDepth = 128)
        : this(file.OpenRead(), caseSensitivity, jsonMaxDepth)
    {
    }

    /// <inheritdoc />
    public string Get(string culture, string key)
    {
      // try to find {culture} element inside root
      if (!TryFindCultureElement(culture, out var cultureElement)) return null;

      return _caseSensitivity == PropertyCaseSensitivity.CaseInsensitive
          ? GetCaseInsensitive(cultureElement, key)
          : GetCaseSensitive(cultureElement, key);
    }

    /// <inheritdoc />
    public IDictionary<string, string> GetAll(string culture)
    {
      // try to find {culture} element inside root
      if (!TryFindCultureElement(culture, out var cultureElement)) return null;
      var all = new Dictionary<string, string>();

      foreach (var jsonProperty in cultureElement.EnumerateObject())
      {
        DFS(jsonProperty, all);
      }

      return all;
    }

    private static void DFS(JsonProperty root, IDictionary<string, string> dict)
    {
      var stack = new Stack<ElementWithPath>(new[]
      {
                new ElementWithPath(root.Value, root.Name)
            });

      while (stack.Count > 0)
      {
        var el = stack.Pop();

        switch (el.Element.ValueKind)
        {
          case JsonValueKind.String:
          case JsonValueKind.Number:
            dict[el.Path] = el.Element.GetString();

            break;
          case JsonValueKind.Object:
            foreach (var jsonProperty in el.Element.EnumerateObject())
            {
              stack.Push(new ElementWithPath(jsonProperty.Value, $"{el.Path}.{jsonProperty.Name}"));
            }

            break;
        }
      }
    }

    private static JsonElement GetRootElement(JsonDocument jsonDocument)
    {
      // see: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to#jsondocument-is-idisposable
      // to understand the following line
      return jsonDocument.RootElement.Clone();
    }

    private static JsonElement ToRootElement(string json, int jsonMaxDepth)
    {
      var doc = JsonDocument.Parse(json, new JsonDocumentOptions
      {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
        MaxDepth = jsonMaxDepth
      });

      using (doc)
      {
        return GetRootElement(doc);
      }
    }

    private bool TryFindCultureElement(string culture, out JsonElement cultureElement)
    {
      return _caseSensitivity != PropertyCaseSensitivity.CaseInsensitive
          ? _root.TryGetProperty(culture, out cultureElement)
          : TryGetPropertyByKeyCaseInsensitive(_root, culture, out cultureElement);
    }

    private static string GetCaseSensitive(JsonElement cultureElement, string key)
    {
      var properties = key.Split(new[]
      {
                '.'
            }, StringSplitOptions.RemoveEmptyEntries);

      JsonElement current = cultureElement;

      foreach (var property in properties)
      {
        if (!current.TryGetProperty(property, out current))
        {
          return null;
        }
      }

      return current.GetString();
    }

    private static string GetCaseInsensitive(JsonElement cultureElement, string key)
    {
      var properties = key.Split(new[]
      {
                '.'
            }, StringSplitOptions.RemoveEmptyEntries);

      JsonElement current = cultureElement;

      foreach (var property in properties)
      {
        if (!TryGetPropertyByKeyCaseInsensitive(current, property, out current))
        {
          return null;
        }
      }

      return current.GetString();
    }

    private static bool TryGetPropertyByKeyCaseInsensitive(JsonElement element, string key, out JsonElement property)
    {
      property = default;

      foreach (var jsonProperty in element.EnumerateObject())
      {
        if (jsonProperty.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
        {
          property = element.GetProperty(jsonProperty.Name);

          return true;
        }
      }

      return false;
    }

    private class ElementWithPath
    {
      public ElementWithPath(JsonElement element, string path)
      {
        Element = element;
        Path = path;
      }

      public JsonElement Element { get; }
      public string Path { get; }
    }
  }

  public enum PropertyCaseSensitivity
  {
    CaseSensitive,
    CaseInsensitive
  }

}