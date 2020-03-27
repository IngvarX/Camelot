using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class FilesPanelStateServiceTests
    {
        private const string PanelKey = "Key";

        [Fact]
        public void TestEmptyState()
        {
            var repositoryMock = new Mock<IRepository<PanelState>>();
            repositoryMock
                .Setup(m => m.GetById(PanelKey))
                .Returns((PanelState)null);

            var unitOfWorkFactory = GetUnitOfWorkFactory(repositoryMock);

            IFilesPanelStateService filesPanelStateService = new FilesPanelStateService(
                unitOfWorkFactory, PanelKey);

            var emptyState = filesPanelStateService.GetPanelState();

            Assert.NotNull(emptyState);
            Assert.NotNull(emptyState.Tabs);
            Assert.Empty(emptyState.Tabs);
        }

        [Fact]
        public void TestInvalidStateSaving()
        {
            var unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            IFilesPanelStateService filesPanelStateService = new FilesPanelStateService(
                unitOfWorkFactoryMock.Object, PanelKey);

            Assert.Throws<ArgumentNullException>(() => filesPanelStateService.SavePanelState(null));
        }

        [Fact]
        public void TestSettingsPersistence()
        {
            var collection = new List<string>();
            var repositoryMock = new Mock<IRepository<PanelState>>();
            repositoryMock
                .Setup(m => m.GetById(PanelKey))
                .Returns(new PanelState {Tabs = collection});
            repositoryMock
                .Setup(m => m.Upsert(PanelKey, It.IsAny<PanelState>()))
                .Callback<string, PanelState>((key, panelState) =>
                {
                    collection.Clear();
                    collection.AddRange(panelState.Tabs);
                });

            var unitOfWorkFactory = GetUnitOfWorkFactory(repositoryMock);

            IFilesPanelStateService filesPanelStateService = new FilesPanelStateService(
                unitOfWorkFactory, PanelKey);

            var tabs = Enumerable.Range(0, 10).Select(n => n.ToString()).ToArray();
            var state = new PanelState {Tabs = tabs.ToList()};

            filesPanelStateService.SavePanelState(state);

            var savedState = filesPanelStateService.GetPanelState();

            Assert.NotNull(savedState);
            Assert.NotNull(savedState.Tabs);
            Assert.True(savedState.Tabs.Count == tabs.Length);
            Assert.Equal(tabs, savedState.Tabs);
        }

        private static IUnitOfWorkFactory GetUnitOfWorkFactory(IMock<IRepository<PanelState>> repositoryMock)
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.GetRepository<PanelState>())
                .Returns(repositoryMock.Object);
            var unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            unitOfWorkFactoryMock
                .Setup(m => m.Create())
                .Returns(unitOfWorkMock.Object);

            return unitOfWorkFactoryMock.Object;
        }
    }
}