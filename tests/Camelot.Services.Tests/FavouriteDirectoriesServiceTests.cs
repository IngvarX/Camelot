using System.Collections.Generic;
using System.Linq;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests;

public class FavouriteDirectoriesServiceTests
{
    private const string DirectoryPath = "Dir";
    private const string DirectoryWithSlashPath = "Dir/";
    private const string SecondDirectory = "SecondDir";
    private const string ThirdDirectory = "ThirdDir";
    private const string FavouriteDirectoriesKey = "FavouriteDirectories";

    private readonly AutoMocker _autoMocker;

    public FavouriteDirectoriesServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestLoadDefaultDirectory()
    {
        var repository = new Mock<IRepository<FavouriteDirectories>>();
        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(m => m.GetRepository<FavouriteDirectories>())
            .Returns(repository.Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uow.Object);
        _autoMocker
            .Setup<IHomeDirectoryProvider, string>(m => m.HomeDirectoryPath)
            .Returns(DirectoryPath);

        var service = _autoMocker.CreateInstance<FavouriteDirectoriesService>();

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Single(service.FavouriteDirectories);
        var path = service.FavouriteDirectories.Single();

        Assert.Equal(DirectoryPath, path);
    }

    [Fact]
    public void TestLoadDirectory()
    {
        var repository = new Mock<IRepository<FavouriteDirectories>>();
        repository
            .Setup(m => m.GetById(FavouriteDirectoriesKey))
            .Returns(new FavouriteDirectories
            {
                Directories = new List<FavouriteDirectory>
                {
                    new FavouriteDirectory {FullPath = DirectoryPath}
                }
            });
        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(m => m.GetRepository<FavouriteDirectories>())
            .Returns(repository.Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uow.Object);

        var service = _autoMocker.CreateInstance<FavouriteDirectoriesService>();

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Single(service.FavouriteDirectories);
        var path = service.FavouriteDirectories.Single();

        Assert.Equal(DirectoryPath, path);
    }

    [Fact]
    public void TestAddDirectory()
    {
        var repository = new Mock<IRepository<FavouriteDirectories>>();
        repository
            .Setup(m => m.GetById(FavouriteDirectoriesKey))
            .Returns(new FavouriteDirectories
            {
                Directories = new List<FavouriteDirectory>()
            });
        repository
            .Setup(m => m.Upsert(FavouriteDirectoriesKey,
                It.Is<FavouriteDirectories>(fd => fd.Directories.Single().FullPath == DirectoryPath)))
            .Verifiable();
        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(m => m.GetRepository<FavouriteDirectories>())
            .Returns(repository.Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uow.Object);
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(DirectoryWithSlashPath))
            .Returns(DirectoryPath);

        var service = _autoMocker.CreateInstance<FavouriteDirectoriesService>();

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Empty(service.FavouriteDirectories);

        var isCallbackCalled = false;
        service.DirectoryAdded += (sender, args) => isCallbackCalled = args.FullPath == DirectoryPath;

        service.AddDirectory(DirectoryWithSlashPath);

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Single(service.FavouriteDirectories);
        Assert.Equal(DirectoryPath, service.FavouriteDirectories.Single());

        Assert.True(isCallbackCalled);

        repository
            .Verify(m => m.Upsert(FavouriteDirectoriesKey,
                    It.Is<FavouriteDirectories>(fd => fd.Directories.Single().FullPath == DirectoryPath)),
                Times.Once);
    }

    [Fact]
    public void TestAddDirectoryMultipleTimes()
    {
        var repository = new Mock<IRepository<FavouriteDirectories>>();
        repository
            .Setup(m => m.GetById(FavouriteDirectoriesKey))
            .Returns(new FavouriteDirectories
            {
                Directories = new List<FavouriteDirectory>()
            });
        repository
            .Setup(m => m.Upsert(FavouriteDirectoriesKey,
                It.Is<FavouriteDirectories>(fd => fd.Directories.Single().FullPath == DirectoryPath)))
            .Verifiable();
        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(m => m.GetRepository<FavouriteDirectories>())
            .Returns(repository.Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uow.Object);
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(DirectoryWithSlashPath))
            .Returns(DirectoryPath);

        var service = _autoMocker.CreateInstance<FavouriteDirectoriesService>();

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Empty(service.FavouriteDirectories);

        var callbackCalledTimes = 0;
        service.DirectoryAdded += (sender, args) => callbackCalledTimes++;

        for (var i = 0; i < 10; i++)
        {
            service.AddDirectory(DirectoryWithSlashPath);
        }

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Single(service.FavouriteDirectories);
        Assert.Equal(DirectoryPath, service.FavouriteDirectories.Single());

        Assert.Equal(1, callbackCalledTimes);

        repository
            .Verify(m => m.Upsert(FavouriteDirectoriesKey,
                    It.Is<FavouriteDirectories>(fd => fd.Directories.Single().FullPath == DirectoryPath)),
                Times.Once);
    }

    [Fact]
    public void TestRemoveDirectory()
    {
        var repository = new Mock<IRepository<FavouriteDirectories>>();
        repository
            .Setup(m => m.GetById(FavouriteDirectoriesKey))
            .Returns(new FavouriteDirectories
            {
                Directories = new List<FavouriteDirectory>
                {
                    new FavouriteDirectory {FullPath = DirectoryPath}
                }
            });
        repository
            .Setup(m => m.Upsert(FavouriteDirectoriesKey,
                It.Is<FavouriteDirectories>(fd => !fd.Directories.Any())))
            .Verifiable();
        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(m => m.GetRepository<FavouriteDirectories>())
            .Returns(repository.Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uow.Object);
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(DirectoryWithSlashPath))
            .Returns(DirectoryPath);

        var service = _autoMocker.CreateInstance<FavouriteDirectoriesService>();

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Single(service.FavouriteDirectories);

        var isCallbackCalled = false;
        service.DirectoryRemoved += (sender, args) => isCallbackCalled = args.FullPath == DirectoryPath;

        service.RemoveDirectory(DirectoryWithSlashPath);

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Empty(service.FavouriteDirectories);

        Assert.True(isCallbackCalled);

        repository
            .Verify(m => m.Upsert(FavouriteDirectoriesKey,
                    It.Is<FavouriteDirectories>(fd => !fd.Directories.Any())),
                Times.Once);
    }

    [Fact]
    public void TestRemoveDirectoryMultipleTimes()
    {
        var repository = new Mock<IRepository<FavouriteDirectories>>();
        repository
            .Setup(m => m.GetById(FavouriteDirectoriesKey))
            .Returns(new FavouriteDirectories
            {
                Directories = new List<FavouriteDirectory>
                {
                    new FavouriteDirectory {FullPath = DirectoryPath}
                }
            });
        repository
            .Setup(m => m.Upsert(FavouriteDirectoriesKey,
                It.Is<FavouriteDirectories>(fd => !fd.Directories.Any())))
            .Verifiable();
        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(m => m.GetRepository<FavouriteDirectories>())
            .Returns(repository.Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uow.Object);
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(DirectoryWithSlashPath))
            .Returns(DirectoryPath);

        var service = _autoMocker.CreateInstance<FavouriteDirectoriesService>();

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Single(service.FavouriteDirectories);

        var callbackCalledTimes = 0;
        service.DirectoryRemoved += (sender, args) => callbackCalledTimes++;

        for (var i = 0; i < 10; i++)
        {
            service.RemoveDirectory(DirectoryWithSlashPath);
        }

        Assert.NotNull(service.FavouriteDirectories);
        Assert.Empty(service.FavouriteDirectories);

        Assert.Equal(1, callbackCalledTimes);

        repository
            .Verify(m => m.Upsert(FavouriteDirectoriesKey,
                    It.Is<FavouriteDirectories>(fd => !fd.Directories.Any())),
                Times.Once);
    }

    [Theory]
    [InlineData(0, 2, ThirdDirectory, DirectoryPath, SecondDirectory)]
    [InlineData(1, 1, DirectoryPath, SecondDirectory, ThirdDirectory)]
    [InlineData(0, 1, SecondDirectory, DirectoryPath, ThirdDirectory)]
    [InlineData(2, 1, DirectoryPath, ThirdDirectory, SecondDirectory)]
    [InlineData(2, 2, DirectoryPath, SecondDirectory, ThirdDirectory)]
    [InlineData(0, 0, DirectoryPath, SecondDirectory, ThirdDirectory)]
    public void TestMoveDirectory(int fromIndex, int toIndex, string newFirst, string newSecond, string newThird)
    {
        var repository = new Mock<IRepository<FavouriteDirectories>>();
        repository
            .Setup(m => m.GetById(FavouriteDirectoriesKey))
            .Returns(new FavouriteDirectories
            {
                Directories = new List<FavouriteDirectory>
                {
                    new() {FullPath = DirectoryPath},
                    new() {FullPath = SecondDirectory},
                    new() {FullPath = ThirdDirectory}
                }
            });
        var updatedDirectories = new List<FavouriteDirectory>
        {
            new() { FullPath = newFirst },
            new() { FullPath = newSecond },
            new() { FullPath = newThird }
        };
        repository
            .Setup(m => m.Upsert(FavouriteDirectoriesKey,
                It.Is<FavouriteDirectories>(fd =>
                    fd.Directories.Equals(updatedDirectories))))
            .Verifiable();
        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(m => m.GetRepository<FavouriteDirectories>())
            .Returns(repository.Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uow.Object);
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(It.IsAny<string>()))
            .Returns<string>(s => s);

        var service = _autoMocker.CreateInstance<FavouriteDirectoriesService>();

        Assert.NotNull(service.FavouriteDirectories);

        service.MoveDirectory(fromIndex, toIndex);

        repository
            .Setup(m => m.Upsert(FavouriteDirectoriesKey,
                It.Is<FavouriteDirectories>(fd =>
                    fd.Directories.Equals(updatedDirectories))))
            .Verifiable();
    }

    [Theory]
    [InlineData(DirectoryPath, DirectoryPath, true)]
    [InlineData(DirectoryWithSlashPath, DirectoryPath, true)]
    [InlineData(SecondDirectory, SecondDirectory, false)]
    public void TestContains(string directory, string directoryWithoutSlash, bool expectedResult)
    {
        var repository = new Mock<IRepository<FavouriteDirectories>>();
        repository
            .Setup(m => m.GetById(FavouriteDirectoriesKey))
            .Returns(new FavouriteDirectories
            {
                Directories = new List<FavouriteDirectory>
                {
                    new FavouriteDirectory
                    {
                        FullPath = DirectoryPath
                    }
                }
            });
        var uow = new Mock<IUnitOfWork>();
        uow
            .Setup(m => m.GetRepository<FavouriteDirectories>())
            .Returns(repository.Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uow.Object);
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(directory))
            .Returns(directoryWithoutSlash);

        var service = _autoMocker.CreateInstance<FavouriteDirectoriesService>();

        var actualResult = service.ContainsDirectory(directory);
        Assert.Equal(expectedResult, actualResult);
    }
}