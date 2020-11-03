using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.State;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests.State
{
    public class CreateArchiveStateServiceTests
    {
        private readonly AutoMocker _autoMocker;

        public CreateArchiveStateServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(ArchiveType.Gz)]
        [InlineData(ArchiveType.TarBz2)]
        [InlineData(ArchiveType.Zip)]
        public void TestGetState(ArchiveType archiveType)
        {
            var repoMock = new Mock<IRepository<CreateArchiveSettings>>();
            repoMock
                .Setup(m => m.GetById(It.IsAny<string>()))
                .Returns(new CreateArchiveSettings
                {
                    ArchiveType = (int) archiveType
                });
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(m => m.GetRepository<CreateArchiveSettings>())
                .Returns(repoMock.Object);
            _autoMocker
                .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
                .Returns(uowMock.Object);

            var service = _autoMocker.CreateInstance<CreateArchiveStateService>();

            var state = service.GetState();

            Assert.Equal(archiveType, state.ArchiveType);
        }

        [Fact]
        public void TestGetStateEmpty()
        {
            var repoMock = new Mock<IRepository<CreateArchiveSettings>>();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(m => m.GetRepository<CreateArchiveSettings>())
                .Returns(repoMock.Object);
            _autoMocker
                .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
                .Returns(uowMock.Object);

            var service = _autoMocker.CreateInstance<CreateArchiveStateService>();

            var state = service.GetState();

            Assert.Equal(ArchiveType.Zip, state.ArchiveType);
        }

        [Theory]
        [InlineData(ArchiveType.Gz)]
        [InlineData(ArchiveType.TarBz2)]
        [InlineData(ArchiveType.Zip)]
        public void TestSaveState(ArchiveType archiveType)
        {
            var repoMock = new Mock<IRepository<CreateArchiveSettings>>();
            repoMock
                .Setup(m => m.Upsert(It.IsAny<string>(),
                    It.Is<CreateArchiveSettings>(s => s.ArchiveType == (int) archiveType)))
                .Verifiable();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(m => m.GetRepository<CreateArchiveSettings>())
                .Returns(repoMock.Object);
            _autoMocker
                .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
                .Returns(uowMock.Object);

            var service = _autoMocker.CreateInstance<CreateArchiveStateService>();

            var state = new CreateArchiveStateModel
            {
                ArchiveType = archiveType
            };
            service.SaveState(state);

            repoMock
                .Verify(m => m.Upsert(It.IsAny<string>(),
                        It.Is<CreateArchiveSettings>(s => s.ArchiveType == (int) archiveType)),
                    Times.Once);
        }

        [Fact]
        public void TestSaveStateFailed()
        {
            var service = _autoMocker.CreateInstance<CreateArchiveStateService>();

            void SaveState() => service.SaveState(null);

            Assert.Throws<ArgumentNullException>(SaveState);
        }
    }
}