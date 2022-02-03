using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models;

public class SuggestionModel
{
    public string FullPath { get; }

    public SuggestionType Type { get; }

    public SuggestionModel(string fullPath, SuggestionType type)
    {
        FullPath = fullPath;
        Type = type;
    }
}