using System.Reflection;

namespace libc.translation.tests
{
    public class EmbeddedResourceTranslationTests : BaseLocalizationSourceTests
    {
        protected override ILocalizer GetLocalizer(PropertyCaseSensitivity caseSensitivity)
        {
            return new Localizer(
                source: new JsonLocalizationSource(
                    assembly: Assembly.GetExecutingAssembly(),
                    resourceId: $"{typeof(EmbeddedResourceTranslationTests).Namespace}.sample.json",
                    caseSensitivity: caseSensitivity
                )
            );
        }
    }
}