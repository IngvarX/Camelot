using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Moq;
using Xunit;

namespace Camelot.Services.Tests.State
{
    public class FilesPanelStateServiceTests
    {
        private const string PanelKey = "Key";

        [Fact]
        public void TestEmptyState()
        {
            var repositoryMock = new Mock<IRepository<PanelModel>>();
            repositoryMock
                .Setup(m => m.GetById(PanelKey))
                .Returns((PanelModel)null);

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
            var collection = new List<TabModel>();
            var repositoryMock = new Mock<IRepository<PanelModel>>();
            repositoryMock
                .Setup(m => m.GetById(PanelKey))
                .Returns(new PanelModel {Tabs = collection});
            repositoryMock
                .Setup(m => m.Upsert(PanelKey, It.IsAny<PanelModel>()))
                .Callback<string, PanelModel>((key, panelState) =>
                {
                    collection.Clear();
                    collection.AddRange(panelState.Tabs);
                });

            var unitOfWorkFactory = GetUnitOfWorkFactory(repositoryMock);

            IFilesPanelStateService filesPanelStateService = new FilesPanelStateService(
                unitOfWorkFactory, PanelKey);

            var tabs = Enumerable
                .Range(0, 10)
                .Select(_ => new TabStateModel
                {
                    SortingSettings = new SortingSettingsStateModel()
                })
                .ToList();
            var state = new PanelStateModel {Tabs = tabs};

            filesPanelStateService.SavePanelState(state);

            var savedState = filesPanelStateService.GetPanelState();

            Assert.NotNull(savedState);
            Assert.NotNull(savedState.Tabs);
            Assert.Equal(tabs.Count, savedState.Tabs.Count);
        }

        private static IUnitOfWorkFactory GetUnitOfWorkFactory(IMock<IRepository<PanelModel>> repositoryMock)
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.GetRepository<PanelModel>())
                .Returns(repositoryMock.Object);
            var unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            unitOfWorkFactoryMock
                .Setup(m => m.Create())
                .Returns(unitOfWorkMock.Object);

            return unitOfWorkFactoryMock.Object;
        }
    }
}