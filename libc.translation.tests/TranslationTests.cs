using System;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace libc.translation.tests
{
    public class TranslationTests
    {
        [Fact]
        public void EmbeddedResource_CaseInsensitive()
        {
            var localizer = new Localizer(new LocalizationSource(Assembly.GetExecutingAssembly(),
                $"{typeof(TranslationTests).Namespace}.embedded.json",
                LocalizationSourcePropertyCaseSensitivity.CaseInsensitive));

            // first
            const string key1 = "InvalidInput";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo("ar");
            Assert.Equal("إدخال غير صالح", localizer.Get(key1), StringComparer.Ordinal);
            Assert.Equal("خطا در اطلاعات ورودی", localizer.Get("fa", key1), StringComparer.Ordinal);

            // set threads current culture to "en"
            CultureInfo.CurrentCulture = new CultureInfo("en");
            Assert.Equal("Invalid input", localizer.Get(key1), StringComparer.Ordinal);

            // second
            const string key2 = "unknownerror";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo("ar");

            Assert.Equal("خطأ غير معروف some word", localizer.GetFormat(key2, new object[]
                {
                    "some word"
                }),
                StringComparer.Ordinal);

            Assert.Equal("خطای نامشخص test", localizer.GetFormat("fa", key2, "test"),
                StringComparer.Ordinal);

            // set threads current culture to "en"
            CultureInfo.CurrentCulture = new CultureInfo("en");

            Assert.Equal("Unknown error !!", localizer.GetFormat(key2, new object[]
            {
                "!!"
            }), StringComparer.Ordinal);

            Assert.Equal("Unknown error !!", localizer.GetFormat("de", key2, "!!"), StringComparer.Ordinal);
        }

        [Fact]
        public void EmbeddedResource_CaseSensitive()
        {
            var localizer = new Localizer(new LocalizationSource(Assembly.GetExecutingAssembly(),
                $"{typeof(TranslationTests).Namespace}.embedded.json",
                LocalizationSourcePropertyCaseSensitivity.CaseSensitive));

            // first
            const string key1 = "InvalidInput";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo("ar");
            Assert.Equal("إدخال غير صالح", localizer.Get(key1), StringComparer.Ordinal);
            Assert.Equal("خطا در اطلاعات ورودی", localizer.Get("fa", key1), StringComparer.Ordinal);

            // set threads current culture to "en"
            CultureInfo.CurrentCulture = new CultureInfo("en");
            Assert.Equal("Invalid input", localizer.Get(key1), StringComparer.Ordinal);

            // second
            const string key2 = "unknownerror";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo("ar");

            var x = localizer.GetFormat(key2, new object[]
            {
                "some word"
            });

            Assert.Equal(key2, x);
        }
    }
}