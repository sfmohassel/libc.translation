using System.Collections.Generic;
using System.Globalization;

namespace libc.translation
{
  public interface ILocalizer
  {
    /// <summary>
    ///     The <see cref="ILocalizationSource" /> instance which provides translations
    /// </summary>
    ILocalizationSource Source { get; }

    /// <summary>
    ///     The fallback culture this <see cref="ILocalizer" /> uses
    /// </summary>
    string FallbackCulture { get; }

    /// <summary>
    ///     Creates a new <see cref="ILocalizer" /> from this instance with the given fallback culture
    /// </summary>
    /// <param name="fallbackCulture"></param>
    /// <returns></returns>
    ILocalizer WithFallbackCulture(string fallbackCulture);

    /// <summary>
    ///     Gets the translation text for the given <see cref="culture" /> and <see cref="key" />
    ///     <para>If the translation for <see cref="culture" /> is not found, fallback culture is used</para>
    /// </summary>
    /// <param name="culture">The culture in which you need your translation to be</param>
    /// <param name="key">The key for translation text</param>
    /// <returns></returns>
    string Get(string culture, string key);

    /// <summary>
    ///     Gets the translation text for the given <see cref="key" /> and <see cref="CultureInfo.CurrentCulture" />
    ///     <para>If the translation for <see cref="CultureInfo.CurrentCulture" /> is not found, fallback culture is used</para>
    /// </summary>
    /// <param name="key">The key for translation text</param>
    /// <returns></returns>
    string Get(string key);

    /// <summary>
    ///     Gets the formatted translation text for the given <see cref="culture" /> and <see cref="key" />
    ///     <para>
    ///         The translation text will be formatted using <see cref="string.Format(string,object)" /> given
    ///         <see cref="values" />
    ///     </para>
    ///     <para>If the translation for <see cref="culture" /> is not found, fallback culture is used</para>
    /// </summary>
    /// <param name="culture">The culture in which you need your translation to be</param>
    /// <param name="key">The key for translation text</param>
    /// <param name="values">Parameters for <see cref="string.Format(string,object)" /></param>
    /// <returns></returns>
    string GetFormat(string culture, string key, params object[] values);

    /// <summary>
    ///     Gets the formatted translation text for the given <see cref="key" /> and <see cref="CultureInfo.CurrentCulture" />
    ///     <para>
    ///         The translation text will be formatted using <see cref="string.Format(string,object)" /> given
    ///         <see cref="values" />
    ///     </para>
    ///     <para>If the translation for <see cref="CultureInfo.CurrentCulture" /> is not found, fallback culture is used</para>
    /// </summary>
    /// <param name="key">The key for translation text</param>
    /// <param name="values">Parameters for <see cref="string.Format(string,object)" /></param>
    /// <returns></returns>
    string GetFormat(string key, params object[] values);

    /// <summary>
    ///     Gets all the translation text-values as a <see cref="IDictionary{TKey,TValue}" /> in the given
    ///     <see cref="culture" />
    /// </summary>
    /// <para>If the translation for <see cref="culture" /> is not found, fallback culture is used</para>
    /// <param name="culture">The culture in which you need all translation texts</param>
    /// <returns></returns>
    IDictionary<string, string> GetAll(string culture);

    /// <summary>
    ///     Gets all the translation text-values as a <see cref="IDictionary{TKey,TValue}" /> in
    ///     <see cref="CultureInfo.CurrentCulture" />
    ///     <para>If the translation for <see cref="CultureInfo.CurrentCulture" /> is not found, fallback culture is used</para>
    /// </summary>
    /// <returns></returns>
    IDictionary<string, string> GetAll();
  }
}