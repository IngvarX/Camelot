using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;

namespace Camelot.Services;

public class SuggestionsService : ISuggestionsService
{
    private readonly IDirectoryService _directoryService;
    private readonly IPathService _pathService;
    private readonly IFavouriteDirectoriesService _favouriteDirectoriesService;
    private readonly SuggestionsConfiguration _configuration;

    public SuggestionsService(
        IDirectoryService directoryService,
        IPathService pathService,
        IFavouriteDirectoriesService favouriteDirectoriesService,
        SuggestionsConfiguration configuration)
    {
        _directoryService = directoryService;
        _pathService = pathService;
        _favouriteDirectoriesService = favouriteDirectoriesService;
        _configuration = configuration;
    }

    public IEnumerable<SuggestionModel> GetSuggestions(string substring) =>
        GetChildDirectories(substring)
            .Concat(_favouriteDirectoriesService.FavouriteDirectories)
            .Where(n => n.StartsWith(substring))
            .Select(Preprocess)
            .Distinct()
            .Select(CreateFrom)
            .OrderByDescending(m => m.Type)
            .ThenBy(m => m.FullPath.Length)
            .Take(_configuration.SuggestionsCount);

    private IEnumerable<string> GetChildDirectories(string substring)
    {
        var parentDirectory = _pathService.GetParentDirectory(substring);

        return parentDirectory is null || !_directoryService.CheckIfExists(parentDirectory)
            ? Enumerable.Empty<string>()
            : _directoryService
                .GetChildDirectories(parentDirectory)
                .Select(d => d.FullPath);
    }

    private string Preprocess(string path) => _pathService.RightTrimPathSeparators(path);

    private SuggestionModel CreateFrom(string fullPath)
    {
        var type = GetSuggestionType(fullPath);

        return new SuggestionModel(fullPath, type);
    }

    private SuggestionType GetSuggestionType(string fullPath) =>
        _favouriteDirectoriesService.ContainsDirectory(fullPath)
            ? SuggestionType.FavouriteDirectory
            : SuggestionType.Directory;
}