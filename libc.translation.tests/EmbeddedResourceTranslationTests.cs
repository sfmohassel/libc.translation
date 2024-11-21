using System;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace libc.translation.tests;

public class EmbeddedResourceTranslationTests
{
  private static ILocalizer GetLocalizer(PropertyCaseSensitivity caseSensitivity)
  {
    return new Localizer(
      new JsonLocalizationSource(
        Assembly.GetExecutingAssembly(),
        $"{typeof(EmbeddedResourceTranslationTests).Namespace}.sample.json",
        caseSensitivity
      )
    );
  }

  [Fact]
  public void CaseInsensitive()
  {
    var localizer = GetLocalizer(PropertyCaseSensitivity.CaseInsensitive);

    // first....................................................
    const string key1 = "InvalidInput";

    // set thread's current culture to "ar"
    CultureInfo.CurrentCulture = new CultureInfo("ar");

    Assert.Equal(
      "إدخال غير صالح",
      localizer.Get(key1),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Ungültige Eingabe",
      localizer.Get("de", key1),
      StringComparer.Ordinal
    );

    // set threads current culture to "en"
    CultureInfo.CurrentCulture = new CultureInfo("en");

    Assert.Equal(
      "Invalid input",
      localizer.Get(key1),
      StringComparer.Ordinal
    );

    // second.................................................
    const string key2 = "unknownerror";

    // set thread's current culture to "ar"
    CultureInfo.CurrentCulture = new CultureInfo("ar");

    Assert.Equal(
      "خطأ غير معروف some word",
      localizer.GetFormat(key2, new object[]
      {
        "some word"
      }),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Unbekannter Fehler test",
      localizer.GetFormat("de", key2, "test"),
      StringComparer.Ordinal
    );

    // set threads current culture to "en"
    CultureInfo.CurrentCulture = new CultureInfo("en");

    Assert.Equal(
      "Unknown error !!",
      localizer.GetFormat(key2, new object[]
      {
        "!!"
      }),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Unbekannter Fehler !!",
      localizer.GetFormat("de", key2, "!!"),
      StringComparer.Ordinal
    );
  }

  [Fact]
  public void CaseSensitive()
  {
    var localizer = GetLocalizer(PropertyCaseSensitivity.CaseSensitive);

    // first...................................................
    const string key1 = "InvalidInput";

    // set thread's current culture to "ar"
    CultureInfo.CurrentCulture = new CultureInfo("ar");

    Assert.Equal(
      "إدخال غير صالح",
      localizer.Get(key1),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Ungültige Eingabe",
      localizer.Get("de", key1),
      StringComparer.Ordinal
    );

    // set threads current culture to "en"
    CultureInfo.CurrentCulture = new CultureInfo("en");

    Assert.Equal(
      "Invalid input",
      localizer.Get(key1),
      StringComparer.Ordinal
    );

    // second..................................................
    const string key2 = "unknownerror";

    // set thread's current culture to "ar"
    CultureInfo.CurrentCulture = new CultureInfo("ar");

    var x = localizer.GetFormat(key2, new object[]
    {
      "some word"
    });

    Assert.Equal(key2, x);
  }

  [Fact]
  public void Nested_CaseSensitive()
  {
    var localizer = GetLocalizer(PropertyCaseSensitivity.CaseSensitive);

    // set thread's current culture
    CultureInfo.CurrentCulture = new CultureInfo("de");

    Assert.Equal(
      "Willkommen",
      localizer.Get("home-page.title"),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "أهلا بك",
      localizer.Get("ar", "home-page.title"),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Etwas Text",
      localizer.Get("home-page.body.text"),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "بعض النصوص",
      localizer.Get("ar", "home-page.body.text"),
      StringComparer.Ordinal
    );
  }

  [Fact]
  public void Nested_CaseInsensitive()
  {
    var localizer = GetLocalizer(PropertyCaseSensitivity.CaseInsensitive);

    // set thread's current culture
    CultureInfo.CurrentCulture = new CultureInfo("de");

    Assert.Equal(
      "Willkommen",
      localizer.Get("home-page.title"),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "أهلا بك",
      localizer.Get("ar", "home-page.title"),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Etwas Text",
      localizer.Get("home-page.body.text"),
      StringComparer.Ordinal
    );

    Assert.Equal(
      "بعض النصوص",
      localizer.Get("ar", "home-page.body.text"),
      StringComparer.Ordinal
    );
  }

  [Fact]
  public void GetAll()
  {
    var localizer = GetLocalizer(PropertyCaseSensitivity.CaseSensitive);
    CultureInfo.CurrentCulture = new CultureInfo("de");
    var allDe = localizer.GetAll();

    Assert.Equal(
      4,
      allDe.Keys.Count
    );

    Assert.Equal(
      "Ungültige Eingabe",
      allDe["InvalidInput"],
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Unbekannter Fehler {0}",
      allDe["UnknownError"],
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Willkommen",
      allDe["home-page.title"],
      StringComparer.Ordinal
    );

    Assert.Equal(
      "Etwas Text",
      allDe["home-page.body.text"],
      StringComparer.Ordinal
    );
  }
}