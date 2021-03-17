using System.Collections.Generic;
using System.Globalization;
using Camelot.Configuration;
using Camelot.DependencyInjection;
using Camelot.Properties;
using Splat;
using Xunit;

namespace Camelot.Tests
{
    public class LanguagesTests
    {
        [Fact]
        public void TestConfiguration()
        {
            var includedLanguages = new HashSet<string> {"en"};
            var excludedLanguages = new HashSet<string>();

            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (cultureInfo.Equals(CultureInfo.InvariantCulture))
                {
                    continue;
                }

                var resourceSet = Resources.ResourceManager.GetResourceSet(cultureInfo, true, false);
                if (resourceSet is null)
                {
                    continue;
                }

                includedLanguages.Add(cultureInfo.IetfLanguageTag);
            }

            Bootstrapper.Register(Locator.CurrentMutable, Locator.Current);

            var config = Locator.Current.GetRequiredService<LanguagesConfiguration>();

            Assert.NotNull(config);
            Assert.NotNull(config.AvailableLocales);

            excludedLanguages.ExceptWith(includedLanguages);
            var allAvailable = includedLanguages.SetEquals(config.AvailableLocales);

            Assert.True(allAvailable);
        }
    }
}