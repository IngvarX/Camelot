using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests
{
    public class OpenWithApplicationServiceTests
    {
        private const string Extension = "txt";
        private const string AnotherExtension = "pdf";
        private const string DisplayName = "App";
        private const string Arguments = "Args";
        private const string ExecutePath = "app";

        private readonly AutoMocker _autoMocker;

        public OpenWithApplicationServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestEmptyState()
        {
            var repositoryMock = new Mock<IRepository<OpenWithApplicationSettings>>();
            repositoryMock
                .Setup(m => m.GetById(It.IsAny<string>()))
                .Returns((OpenWithApplicationSettings)null);

            var unitOfWorkFactory = GetUnitOfWorkFactory(repositoryMock);
            _autoMocker.Use(unitOfWorkFactory);
            var service = _autoMocker.CreateInstance<OpenWithApplicationService>();

            var application = service.GetSelectedApplication(Extension);

            Assert.Null(application);
        }

        [Fact]
        public void TestNonEmptyStateAppFound()
        {
            var dbApplication = new Application
            {
                DisplayName = DisplayName,
                Arguments = Arguments,
                ExecutePath = ExecutePath
            };
            var settings = new OpenWithApplicationSettings
            {
                ApplicationByExtension = {[Extension] = dbApplication}
            };
            var repositoryMock = new Mock<IRepository<OpenWithApplicationSettings>>();
            repositoryMock
                .Setup(m => m.GetById(It.IsAny<string>()))
                .Returns(settings);

            var unitOfWorkFactory = GetUnitOfWorkFactory(repositoryMock);
            _autoMocker.Use(unitOfWorkFactory);
            var service = _autoMocker.CreateInstance<OpenWithApplicationService>();

            var application = service.GetSelectedApplication(Extension);

            Assert.NotNull(application);
            Assert.Equal(DisplayName, application.DisplayName);
            Assert.Equal(Arguments, application.Arguments);
            Assert.Equal(ExecutePath, application.ExecutePath);
        }

        [Fact]
        public void TestNonEmptyStateAppNotFound()
        {
            var dbApplication = new Application
            {
                DisplayName = DisplayName,
                Arguments = Arguments,
                ExecutePath = ExecutePath
            };
            var settings = new OpenWithApplicationSettings
            {
                ApplicationByExtension = {[Extension] = dbApplication}
            };
            var repositoryMock = new Mock<IRepository<OpenWithApplicationSettings>>();
            repositoryMock
                .Setup(m => m.GetById(It.IsAny<string>()))
                .Returns(settings);

            var unitOfWorkFactory = GetUnitOfWorkFactory(repositoryMock);
            _autoMocker.Use(unitOfWorkFactory);
            var service = _autoMocker.CreateInstance<OpenWithApplicationService>();

            var application = service.GetSelectedApplication(AnotherExtension);

            Assert.Null(application);
        }

        [Fact]
        public void TestSave()
        {
            var isCallbackCalled = false;
            var repositoryMock = new Mock<IRepository<OpenWithApplicationSettings>>();
            repositoryMock
                .Setup(m => m.Upsert(It.IsAny<string>(),
                    It.IsAny<OpenWithApplicationSettings>()))
                .Callback<string, OpenWithApplicationSettings>((id, entity) =>
                {
                    if (id is null || entity?.ApplicationByExtension is null)
                    {
                        return;
                    }

                    if (!entity.ApplicationByExtension.TryGetValue(Extension, out var app) || app is null)
                    {
                        return;
                    }

                    isCallbackCalled = app.DisplayName == DisplayName
                                       && app.Arguments == Arguments
                                       && app.ExecutePath == ExecutePath;
                });

            var unitOfWorkFactory = GetUnitOfWorkFactory(repositoryMock);
            _autoMocker.Use(unitOfWorkFactory);
            var service = _autoMocker.CreateInstance<OpenWithApplicationService>();

            var application = new ApplicationModel
            {
                DisplayName = DisplayName,
                Arguments = Arguments,
                ExecutePath = ExecutePath
            };
            service.SaveSelectedApplication(Extension, application);

            Assert.True(isCallbackCalled);
        }

        private static IUnitOfWorkFactory GetUnitOfWorkFactory(IMock<IRepository<OpenWithApplicationSettings>> repositoryMock)
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.GetRepository<OpenWithApplicationSettings>())
                .Returns(repositoryMock.Object);
            var unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            unitOfWorkFactoryMock
                .Setup(m => m.Create())
                .Returns(unitOfWorkMock.Object);

            return unitOfWorkFactoryMock.Object;
        }
    }
}