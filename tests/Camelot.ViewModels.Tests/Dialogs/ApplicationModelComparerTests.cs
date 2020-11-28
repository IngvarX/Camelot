using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.Comparers;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class ApplicationModelComparerTests
    {
        private readonly AutoMocker _autoMocker;

        public ApplicationModelComparerTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("app1", "app2", false)]
        [InlineData("app1", null, false)]
        [InlineData(null, "app2", false)]
        [InlineData(null, null, true)]
        [InlineData("app", "app", true)]
        public void TestEquals(string firstAppName, string secondAppName, bool expected)
        {
            var comparer = _autoMocker.CreateInstance<ApplicationModelComparer>();
            var firstApp = new ApplicationModel {DisplayName = firstAppName};
            var secondApp = new ApplicationModel {DisplayName = secondAppName};

            var actual = comparer.Equals(firstApp, secondApp);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("app1", "app2", false)]
        [InlineData("app", "app", true)]
        public void TestGetHashCode(string firstAppName, string secondAppName, bool expected)
        {
            var comparer = _autoMocker.CreateInstance<ApplicationModelComparer>();
            var firstApp = new ApplicationModel {DisplayName = firstAppName};
            var secondApp = new ApplicationModel {DisplayName = secondAppName};

            var actual = comparer.GetHashCode(firstApp) == comparer.GetHashCode(secondApp);
            Assert.Equal(expected, actual);
        }
    }
}