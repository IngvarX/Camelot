using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Implementations;

public class SuggestedPathViewModelFactory : ISuggestedPathViewModelFactory
{
    private readonly IPathService _pathService;

    public SuggestedPathViewModelFactory(
        IPathService pathService)
    {
        _pathService = pathService;
    }

    public ISuggestedPathViewModel Create(string searchText, SuggestionModel model)
    {
        var root = _pathService.GetParentDirectory(searchText);
        var relativePath = GetRelativePath(root, model.FullPath);
        var text = _pathService.LeftTrimPathSeparators(relativePath);
        var type = CreateFrom(model.Type);

        return new SuggestedPathViewModel(model.FullPath, type, text);
    }

    private static SuggestedPathType CreateFrom(SuggestionType modelType) =>
        modelType switch
        {
            SuggestionType.Directory => SuggestedPathType.Directory,
            SuggestionType.FavouriteDirectory => SuggestedPathType.FavouriteDirectory,
            _ => throw new ArgumentOutOfRangeException(nameof(modelType), modelType, null)
        };

    private string GetRelativePath(string relativeTo, string fullPath) =>
        string.IsNullOrEmpty(relativeTo) ? fullPath : _pathService.GetRelativePath(relativeTo, fullPath);
}