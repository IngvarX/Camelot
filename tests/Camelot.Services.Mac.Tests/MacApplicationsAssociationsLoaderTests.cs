using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Mac.Configuration;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Mac.Tests
{
    public class MacApplicationsAssociationsLoaderTests
    {
        private const string Extension = "txt";

        private readonly AutoMocker _autoMocker;

        public MacApplicationsAssociationsLoaderTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("path: /home/app", 0)]
        [InlineData("path: /home/app\n/home/app", 0)]
        [InlineData("path: /home/app\nclaimed UTIs: public.text\n", 0)]
        [InlineData("path: /home/app (0x2)\nclaimed UTIs: public.text \n", 1)]
        [InlineData("path:     /home/app    (0x255)  \nclaimed UTIs:   public.text    \n", 1)]
        public async Task TestLoadAssociatedApplicationsAsync(string output, int resultsCount)
        {
            var configuration = new UtiToExtensionsMappingConfiguration
            {
                UtiToExtensionsMapping = new Dictionary<string, List<string>>
                {
                    ["public.text"] = new List<string>
                    {
                        Extension
                    }
                }
            };
            var apps = new[]
            {
                new ApplicationModel
                {
                    ExecutePath = "/home/app"
                }
            };
            _autoMocker.Use(configuration);
            _autoMocker
                .Setup<IProcessService, Task<string>>(m =>
                    m.ExecuteAndGetOutputAsync(
                        "/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister",
                        "-dump Bundle"))
                .ReturnsAsync(output);

            var service = _autoMocker.CreateInstance<MacApplicationsAssociationsLoader>();

            var associations = await service.LoadAssociatedApplicationsAsync(apps);

            Assert.NotNull(associations);
            Assert.Equal(resultsCount, associations.Count);

            if (resultsCount == 1)
            {
                Assert.True(associations.ContainsKey(Extension));
                Assert.Single(associations[Extension]);

                Assert.Equal(apps.Single(), associations[Extension].Single());
            }
        }
    }
}