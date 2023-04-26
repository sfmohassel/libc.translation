using System;
using System.IO;
using System.Reflection;

namespace libc.translation.tests
{
    public class FlatExtractingJsonLocalizationSourceTests: BaseLocalizationSourceTests
    {
        protected override ILocalizer GetLocalizer(PropertyCaseSensitivity caseSensitivity)
        {
            return new Localizer(
                source: new FlatExtractingJsonLocalizationSource(
                    GetLocalPath(), caseSensitivity, "lang.{0}.json"
                )
            );
        }

        private static string GetLocalPath()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var uri = new UriBuilder(assemblyPath);
            var assemblyDirectory = Uri.UnescapeDataString(uri.Path);

            // Assuming the file is located in the same directory as the test assembly
            return Path.GetDirectoryName(assemblyDirectory);
        }
    }
}