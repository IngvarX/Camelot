using System.Collections.Generic;

namespace Camelot.Services.Abstractions
{
    public interface ISuggestionsService
    {
        IEnumerable<string> GetSuggestions(string substring);
    }
}