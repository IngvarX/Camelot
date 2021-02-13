using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Drives
{
    public class DrivesListViewModelTests
    {
        private readonly AutoMocker _autoMocker;

        public DrivesListViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestDrives()
        {
            var drives = new[]
            {
                new DriveModel
                {
                    RootDirectory = "A"
                },
                new DriveModel
                {
                    RootDirectory = "B"
                },
                new DriveModel
                {
                    RootDirectory = "C"
                }
            };
            _autoMocker
                .Setup<IMountedDriveService, IReadOnlyList<DriveModel>>(m => m.MountedDrives)
                .Returns(drives);
            _autoMocker
                .Setup<IUnmountedDriveService, IReadOnlyList<UnmountedDriveModel>>(m => m.UnmountedDrives)
                .Returns(new UnmountedDriveModel[0]);
            var driveViewModels = new List<IDriveViewModel>();
            foreach (var driveModel in drives)
            {
                var driveViewModelMock = new Mock<IDriveViewModel>();
                _autoMocker
                    .Setup<IDriveViewModelFactory, IDriveViewModel>(m => m.Create(driveModel))
                    .Returns(driveViewModelMock.Object);

                driveViewModels.Add(driveViewModelMock.Object);
            }

            _autoMocker
                .Setup<IApplicationDispatcher>(m => m.Dispatch(It.IsAny<Action>()))
                .Callback<Action>(action => action());

            var viewModel = _autoMocker.CreateInstance<DrivesListViewModel>();

            Assert.NotNull(viewModel.Drives);
            var actualDrivesViewModels = viewModel.Drives.ToArray();
            Assert.Equal(drives.Length, actualDrivesViewModels.Length);
            Assert.Equal(actualDrivesViewModels, driveViewModels);
        }

        // [Fact]
        // public void TestDrivesUpdate()
        // {
        //     var driveServiceMock = new Mock<IMountedDriveService>();
        //     driveServiceMock
        //         .SetupGet(m => m.MountedDrives)
        //         .Returns(new List<DriveModel>());
        //     var driveViewModelFactoryMock = new Mock<IDriveViewModelFactory>();
        //     var applicationDispatcherMock = new Mock<IApplicationDispatcher>();
        //     applicationDispatcherMock
        //         .Setup(m => m.Dispatch(It.IsAny<Action>()))
        //         .Callback<Action>(action => action());
        //     driveServiceMock
        //         .SetupGet(m => m.UnmountedDrives)
        //         .Returns(new UnmountedDriveModel[0]);
        //
        //     var viewModel = new DrivesListViewModel(driveServiceMock.Object,
        //         driveViewModelFactoryMock.Object, applicationDispatcherMock.Object);
        //
        //     Assert.NotNull(viewModel.Drives);
        //     Assert.Empty(viewModel.Drives);
        //
        //     var driveModel = new DriveModel
        //     {
        //         RootDirectory = "B"
        //     };
        //     driveServiceMock
        //         .SetupGet(m => m.MountedDrives)
        //         .Returns(new[] {driveModel});
        //     var driveViewModelMock = new Mock<IDriveViewModel>();
        //     driveViewModelFactoryMock
        //         .Setup(m => m.Create(driveModel))
        //         .Returns(driveViewModelMock.Object);
        //
        //     driveServiceMock
        //         .Raise(m => m.DrivesListChanged += null, EventArgs.Empty);
        //
        //     Assert.NotNull(viewModel.Drives);
        //     Assert.Single(viewModel.Drives);
        // }
    }
}