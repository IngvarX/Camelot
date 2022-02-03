using System.Threading.Tasks;
using Camelot.Services.RecursiveSearch;
using Xunit;

namespace Camelot.Services.Tests.RecursiveSearch;

public class RecursiveSearchResultFactoryTests
{
    [Fact]
    public void TestCreate()
    {
        var factory = new RecursiveSearchResultFactory();

        var result = factory.Create(r => Task.CompletedTask);

        Assert.NotNull(result);
        Assert.IsType<RecursiveSearchResult>(result);
    }
}