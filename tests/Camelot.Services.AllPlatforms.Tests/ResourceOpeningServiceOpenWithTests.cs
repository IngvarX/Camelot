using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.AllPlatforms.Tests
{
    public class ResourceOpeningServiceOpenWithTests
    {
        private const string Command = "Command";
        private const string Arguments = "Args";
        private const string Resource = "Resource";
        private const string Extension = "json";

        private readonly AutoMocker _autoMocker;

        public ResourceOpeningServiceOpenWithTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestOpenNoSelectedApp()
        {
            var service = _autoMocker.CreateInstance<ResourceOpeningServiceOpenWith>();

            service.Open(Resource);

            _autoMocker
                .Verify<IResourceOpeningService>(m => m.Open(Resource),
                    Times.Once);
        }

        [Fact]
        public void TestOpenWithSelectedApp()
        {
            var application = new ApplicationModel
            {
                ExecutePath = Command,
                Arguments = Arguments
            };
            _autoMocker
                .Setup<IPathService, string>(m => m.GetExtension(Resource))
                .Returns(Extension);
            _autoMocker
                .Setup<IOpenWithApplicationService, ApplicationModel>(m => m.GetSelectedApplication(Extension))
                .Returns(application);

            var service = _autoMocker.CreateInstance<ResourceOpeningServiceOpenWith>();

            service.Open(Resource);

            _autoMocker
                .Verify<IResourceOpeningService>(m => m.OpenWith(Command, Arguments, Resource),
                    Times.Once);
        }

        [Fact]
        public void TestOpenWith()
        {
            _autoMocker
                .Setup<IResourceOpeningService>(m => m.OpenWith(Command, Arguments, Resource))
                .Verifiable();

            var service = _autoMocker.CreateInstance<ResourceOpeningServiceOpenWith>();

            service.OpenWith(Command, Arguments, Resource);

            _autoMocker
                .Verify<IResourceOpeningService>(m => m.OpenWith(Command, Arguments, Resource),
                    Times.Once);
        }
    }
}