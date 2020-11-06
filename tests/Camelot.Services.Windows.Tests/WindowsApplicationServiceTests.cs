using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsApplicationServiceTests
    {
        [Theory]
        [InlineData(".txt")]
        [InlineData(".xml")]
        public async Task TestGetAssociatedApplications(string fileExtension)
        {
            var applicationService = new WindowsApplicationService();

            var associatedApplications = await applicationService.GetAssociatedApplications(fileExtension);

            Assert.NotNull(associatedApplications);
            Assert.True(associatedApplications.Any());
        }

        [Fact]
        public async Task TestGetInstalledApplications()
        {
            var applicationService = new WindowsApplicationService();

            var associatedApplications = await applicationService.GetInstalledApplications();

            Assert.NotNull(associatedApplications);
            Assert.True(associatedApplications.Any());
        }
    }
}
