using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests;

public class SuggestionsServiceTests
{
    private const string Substring = "Dir";
    private const string Directory = "Directory";
    private const string DirectoryWithSlash = "Directory/";
    private const string ChildDirectory = "ChildDirectory";
    private const string ParentDirectory = "ParentDirectory";
    private const string FavouriteDirectory = "DirectoryFavourite";
    private const string NotFavouriteDirectory = "DirectoryNotFavourite";

    private readonly AutoMocker _autoMocker;

    public SuggestionsServiceTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(GetConfiguration());
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(It.IsAny<string>()))
            .Returns<string>(s => s);
    }

    [Fact]
    public void TestDistinct()
    {
        _autoMocker
            .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
            .Returns(new HashSet<string> {Directory});
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(Substring))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(ParentDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m =>
                m.GetChildDirectories(ParentDirectory, null))
            .Returns(new[]
            {
                new DirectoryModel
                {
                    FullPath = Directory
                }
            });

        var service = _autoMocker.CreateInstance<SuggestionsService>();
        var suggestions = service.GetSuggestions(Substring).ToArray();

        Assert.NotEmpty(suggestions);
        Assert.Single(suggestions);
        Assert.Equal(Directory, suggestions.Single().FullPath);
    }

    [Fact]
    public void TestDistinctAndTrimming()
    {
        _autoMocker
            .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
            .Returns(new HashSet<string> {DirectoryWithSlash});
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(Substring))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(ParentDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(It.IsAny<string>()))
            .Returns(Directory);
        _autoMocker
            .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m =>
                m.GetChildDirectories(ParentDirectory, null))
            .Returns(new[]
            {
                new DirectoryModel
                {
                    FullPath = Directory
                }
            });

        var service = _autoMocker.CreateInstance<SuggestionsService>();
        var suggestions = service.GetSuggestions(Substring).ToArray();

        Assert.NotEmpty(suggestions);
        Assert.Single(suggestions);
        Assert.Equal(Directory, suggestions.Single().FullPath);
    }

    [Fact]
    public void TestFiltering()
    {
        _autoMocker
            .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
            .Returns(new HashSet<string>());
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(Substring))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(ParentDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m =>
                m.GetChildDirectories(ParentDirectory, null))
            .Returns(new[]
            {
                new DirectoryModel
                {
                    FullPath = Directory
                },
                new DirectoryModel
                {
                    FullPath = ChildDirectory
                }
            });

        var service = _autoMocker.CreateInstance<SuggestionsService>();
        var suggestions = service.GetSuggestions(Substring).ToArray();

        Assert.NotEmpty(suggestions);
        Assert.Single(suggestions);
        Assert.Equal(Directory, suggestions.Single().FullPath);
    }

    [Fact]
    public void TestSorting()
    {
        _autoMocker
            .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
            .Returns(new HashSet<string> {FavouriteDirectory});
        _autoMocker
            .Setup<IFavouriteDirectoriesService, bool>(m => m.ContainsDirectory(FavouriteDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(Substring))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(ParentDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m =>
                m.GetChildDirectories(ParentDirectory, null))
            .Returns(new[]
            {
                new DirectoryModel
                {
                    FullPath = NotFavouriteDirectory
                },
                new DirectoryModel
                {
                    FullPath = Directory
                }
            });

        var service = _autoMocker.CreateInstance<SuggestionsService>();
        var suggestions = service.GetSuggestions(Substring).ToArray();

        Assert.NotEmpty(suggestions);
        Assert.Equal(3, suggestions.Length);

        Assert.Equal(SuggestionType.FavouriteDirectory, suggestions[0].Type);
        Assert.Equal(SuggestionType.Directory, suggestions[1].Type);
        Assert.Equal(SuggestionType.Directory, suggestions[2].Type);

        Assert.Equal(FavouriteDirectory, suggestions[0].FullPath);
        Assert.Equal(Directory, suggestions[1].FullPath);
        Assert.Equal(NotFavouriteDirectory, suggestions[2].FullPath);
    }

    [Fact]
    public void TestNoParent()
    {
        _autoMocker
            .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
            .Returns(new HashSet<string> {FavouriteDirectory});

        var service = _autoMocker.CreateInstance<SuggestionsService>();
        var suggestions = service.GetSuggestions(Substring).ToArray();

        Assert.NotEmpty(suggestions);
        Assert.Single(suggestions);
        Assert.Equal(FavouriteDirectory, suggestions.Single().FullPath);
    }

    private static SuggestionsConfiguration GetConfiguration() =>
        new SuggestionsConfiguration
        {
            SuggestionsCount = 42
        };
}