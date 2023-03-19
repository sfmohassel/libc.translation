using System.Collections.Generic;
using System.Globalization;

namespace libc.translation
{

  public class Localizer : ILocalizer
  {
    /// <summary>
    ///     Create a new <see cref="ILocalizer" /> implementation using a given <see cref="ILocalizationSource" /> with
    ///     <see cref="fallbackCulture" />
    /// </summary>
    /// <param name="source">The <see cref="ILocalizationSource" /> implementation</param>
    /// <param name="fallbackCulture">The fallback culture. Defaults to "en"</param>
    public Localizer(ILocalizationSource source, string fallbackCulture = "en")
    {
      Source = source;
      FallbackCulture = fallbackCulture;
    }

    /// <inheritdoc />
    public ILocalizationSource Source { get; }

    /// <inheritdoc />
    public string FallbackCulture { get; private set; }

    /// <inheritdoc />
    public ILocalizer WithFallbackCulture(string culture)
    {
      FallbackCulture = culture.ToLower();

      return this;
    }

    /// <inheritdoc />
    public string Get(string culture, string key)
    {
      Get(culture, key, out var res);

      return res;
    }

    /// <inheritdoc />
    public string Get(string key)
    {
      return Get(GetThreadCulture(), key);
    }

    /// <inheritdoc />
    public string GetFormat(string culture, string key, params object[] values)
    {
      var found = Get(culture, key, out var res);

      return found ? string.Format(res, values) : res;
    }

    /// <inheritdoc />
    public string GetFormat(string key, params object[] values)
    {
      return GetFormat(GetThreadCulture(), key, values);
    }

    /// <inheritdoc />
    public IDictionary<string, string> GetAll(string culture)
    {
      return Source.GetAll(culture) ?? Source.GetAll(FallbackCulture);
    }

    /// <inheritdoc />
    public IDictionary<string, string> GetAll()
    {
      return GetAll(GetThreadCulture());
    }

    private static string GetThreadCulture()
    {
      return CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
    }

    private bool Get(string culture, string key, out string value)
    {
      value = Source.Get(culture.ToLower(), key);

      if (value != null) return true;

      var fallbackValue = Source.Get(FallbackCulture, key);

      if (fallbackValue == null)
      {
        value = key;

        return false;
      }

      value = fallbackValue;

      return true;
    }
  }

}