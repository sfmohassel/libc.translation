using System;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace libc.translation.tests
{
    public class EmbeddedResourceTranslationTests
    {
        private static ILocalizer GetLocalizer(PropertyCaseSensitivity caseSensitivity)
        {
            return new Localizer(
                source: new JsonLocalizationSource(
                    assembly: Assembly.GetExecutingAssembly(),
                    resourceId: $"{typeof(EmbeddedResourceTranslationTests).Namespace}.sample.json",
                    caseSensitivity: caseSensitivity
                )
            );
        }

        [Fact]
        public void CaseInsensitive()
        {
            var localizer = GetLocalizer(caseSensitivity: PropertyCaseSensitivity.CaseInsensitive);

            // first....................................................
            const string key1 = "InvalidInput";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo(name: "ar");

            Assert.Equal(
                expected: "إدخال غير صالح",
                actual: localizer.Get(key: key1),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Ungültige Eingabe",
                actual: localizer.Get(culture: "de", key: key1),
                comparer: StringComparer.Ordinal
            );

            // set threads current culture to "en"
            CultureInfo.CurrentCulture = new CultureInfo(name: "en");

            Assert.Equal(
                expected: "Invalid input",
                actual: localizer.Get(key: key1),
                comparer: StringComparer.Ordinal
            );

            // second.................................................
            const string key2 = "unknownerror";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo(name: "ar");

            Assert.Equal(
                expected: "خطأ غير معروف some word",
                actual: localizer.GetFormat(key: key2, values: new object[]
                {
                    "some word"
                }),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Unbekannter Fehler test",
                actual: localizer.GetFormat(culture: "de", key: key2, "test"),
                comparer: StringComparer.Ordinal
            );

            // set threads current culture to "en"
            CultureInfo.CurrentCulture = new CultureInfo(name: "en");

            Assert.Equal(
                expected: "Unknown error !!",
                actual: localizer.GetFormat(key: key2, values: new object[]
                {
                    "!!"
                }),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Unbekannter Fehler !!",
                actual: localizer.GetFormat(culture: "de", key: key2, "!!"),
                comparer: StringComparer.Ordinal
            );
        }

        [Fact]
        public void CaseSensitive()
        {
            var localizer = GetLocalizer(caseSensitivity: PropertyCaseSensitivity.CaseSensitive);

            // first...................................................
            const string key1 = "InvalidInput";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo(name: "ar");

            Assert.Equal(
                expected: "إدخال غير صالح",
                actual: localizer.Get(key: key1),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Ungültige Eingabe",
                actual: localizer.Get(culture: "de", key: key1),
                comparer: StringComparer.Ordinal
            );

            // set threads current culture to "en"
            CultureInfo.CurrentCulture = new CultureInfo(name: "en");

            Assert.Equal(
                expected: "Invalid input",
                actual: localizer.Get(key: key1),
                comparer: StringComparer.Ordinal
            );

            // second..................................................
            const string key2 = "unknownerror";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo(name: "ar");

            var x = localizer.GetFormat(key: key2, values: new object[]
            {
                "some word"
            });

            Assert.Equal(expected: key2, actual: x);
        }

        [Fact]
        public void Nested_CaseSensitive()
        {
            var localizer = GetLocalizer(caseSensitivity: PropertyCaseSensitivity.CaseSensitive);

            // set thread's current culture
            CultureInfo.CurrentCulture = new CultureInfo(name: "de");

            Assert.Equal(
                expected: "Willkommen",
                actual: localizer.Get(key: "home-page.title"),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "أهلا بك",
                actual: localizer.Get(culture: "ar", key: "home-page.title"),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Etwas Text",
                actual: localizer.Get(key: "home-page.body.text"),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "بعض النصوص",
                actual: localizer.Get(culture: "ar", key: "home-page.body.text"),
                comparer: StringComparer.Ordinal
            );
        }

        [Fact]
        public void Nested_CaseInsensitive()
        {
            var localizer = GetLocalizer(caseSensitivity: PropertyCaseSensitivity.CaseInsensitive);

            // set thread's current culture
            CultureInfo.CurrentCulture = new CultureInfo(name: "de");

            Assert.Equal(
                expected: "Willkommen",
                actual: localizer.Get(key: "home-page.title"),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "أهلا بك",
                actual: localizer.Get(culture: "ar", key: "home-page.title"),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Etwas Text",
                actual: localizer.Get(key: "home-page.body.text"),
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "بعض النصوص",
                actual: localizer.Get(culture: "ar", key: "home-page.body.text"),
                comparer: StringComparer.Ordinal
            );
        }

        [Fact]
        public void GetAll()
        {
            var localizer = GetLocalizer(caseSensitivity: PropertyCaseSensitivity.CaseSensitive);
            CultureInfo.CurrentCulture = new CultureInfo(name: "de");
            var allDe = localizer.GetAll();

            Assert.Equal(
                expected: 4,
                actual: allDe.Keys.Count
            );

            Assert.Equal(
                expected: "Ungültige Eingabe",
                actual: allDe[key: "InvalidInput"],
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Unbekannter Fehler {0}",
                actual: allDe[key: "UnknownError"],
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Willkommen",
                actual: allDe[key: "home-page.title"],
                comparer: StringComparer.Ordinal
            );

            Assert.Equal(
                expected: "Etwas Text",
                actual: allDe[key: "home-page.body.text"],
                comparer: StringComparer.Ordinal
            );
        }
    }
}