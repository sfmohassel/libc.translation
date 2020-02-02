using System;
using System.Globalization;
using System.Reflection;
using Xunit;
namespace libc.translation.tests {
    public class TranslationTests {
        [Fact]
        public void EmbeddedResource() {
            var localizer = new Localizer(new LocalizationSource(Assembly.GetExecutingAssembly(),
                $"{typeof(TranslationTests).Namespace}.embedded.json"));
            
            const string key1 = "InvalidInput";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo("ar");
            Assert.Equal("إدخال غير صالح", localizer.Get(key1), StringComparer.Ordinal);
            Assert.Equal("خطا در اطلاعات ورودی", localizer.Get("fa", key1), StringComparer.Ordinal);

            // set threads current culture to "en"
            CultureInfo.CurrentCulture = new CultureInfo("en");
            Assert.Equal("Invalid input", localizer.Get(key1), StringComparer.Ordinal);
            
            const string key2 = "unknownerror";

            // set thread's current culture to "ar"
            CultureInfo.CurrentCulture = new CultureInfo("ar");
            Assert.Equal(string.Format("خطأ غير معروف {0}", "some word"), localizer.GetFormat(key2, new []{
                    "some word"
                }),
                StringComparer.Ordinal);
            Assert.Equal(string.Format("خطای نامشخص {0}", "test"), localizer.GetFormat("fa", key2, "test"),
                StringComparer.Ordinal);

            // set threads current culture to "en"
            CultureInfo.CurrentCulture = new CultureInfo("en");
            Assert.Equal(string.Format("Unknown error {0}", "!!"), localizer.GetFormat(key2, new [] {
                "!!"
            }), StringComparer.Ordinal);

            Assert.Equal(string.Format("Unknown error {0}", "!!"), localizer.GetFormat("de", key2, new[] {
                "!!"
            }), StringComparer.Ordinal);
        }
    }
}