using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Configuration;

namespace Camelot.Services
{
    public class SuggestionsService : ISuggestionsService
    {
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly SuggestionsConfiguration _configuration;

        public SuggestionsService(
            IDirectoryService directoryService,
            IPathService pathService,
            SuggestionsConfiguration configuration)
        {
            _directoryService = directoryService;
            _pathService = pathService;
            _configuration = configuration;
        }

        public IEnumerable<string> GetSuggestions(string substring) =>
            _directoryService
                .GetChildDirectories(_pathService.GetParentDirectory(substring))
                .Select(d => d.FullPath)
                .Where(n => n.StartsWith(substring))
                .OrderBy(GetPriority)
                .ThenBy(s => s.Length)
                .Take(_configuration.SuggestionsCount);

        private int GetPriority(string path)
        {
            // TODO: support fav dirs and recently used paths
            return 1;
        }
    }
}