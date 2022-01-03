using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Camelot.Configuration;
using Camelot.DependencyInjection;
using Camelot.Properties;
using Splat;
using Xunit;

namespace Camelot.Tests;

[Collection("DiTests")]
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

        Bootstrapper.Register(Locator.CurrentMutable, Locator.Current, new DataAccessConfiguration());

        var config = Locator.Current.GetRequiredService<LanguagesConfiguration>();

        Assert.NotNull(config);
        Assert.NotNull(config.AvailableLocales);
        Assert.NotEmpty(config.AvailableLocales);

        var culture = Thread.CurrentThread.CurrentCulture;
        if (!config.AvailableLocales.Contains(culture.IetfLanguageTag))
        {
            includedLanguages.Remove(culture.IetfLanguageTag);
        }

        excludedLanguages.ExceptWith(includedLanguages);
        var availableLocales = config.AvailableLocales.ToHashSet();
        var allAvailable = includedLanguages.IsSubsetOf(availableLocales);

        Assert.True(allAvailable);
    }
}