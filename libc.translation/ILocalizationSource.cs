using System.Collections.Generic;

namespace libc.translation
{
  public interface ILocalizationSource
  {
    /// <summary>
    ///     Returns value for the given <see cref="key" /> in the given <see cref="culture" />. If not found, null is returned
    /// </summary>
    /// <param name="culture">The culture you need your translation text</param>
    /// <param name="key"></param>
    /// <returns>null if not found</returns>
    string Get(string culture, string key);

    /// <summary>
    ///     Returns all translations of the given culture
    /// </summary>
    /// <returns></returns>
    IDictionary<string, string> GetAll(string culture);
  }
}