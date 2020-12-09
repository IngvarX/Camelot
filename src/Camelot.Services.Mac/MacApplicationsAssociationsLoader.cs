using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Mac.Configuration;
using Camelot.Services.Mac.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacApplicationsAssociationsLoader : IApplicationsAssociationsLoader
    {
        private const string Command = "/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister";
        private const string Arguments = "-dump Bundle";
        private const string PathSubstring = "path:";
        private const string UtiSubstring = "claimed UTIs:";

        private readonly IProcessService _processService;
        private readonly UtiToExtensionsMappingConfiguration _configuration;

        public MacApplicationsAssociationsLoader(
            IProcessService processService,
            UtiToExtensionsMappingConfiguration configuration)
        {
            _processService = processService;
            _configuration = configuration;
        }

        public async Task<IReadOnlyDictionary<string, ISet<ApplicationModel>>> LoadAssociatedApplicationsAsync(
            IEnumerable<ApplicationModel> installedApps)
        {
            var output = await _processService.ExecuteAndGetOutputAsync(Command, Arguments);
            var result = new Dictionary<string, ISet<ApplicationModel>>();
            var installedAppsDictionary = installedApps.ToDictionary(a => a.ExecutePath, a => a);

            var lines = output.Split('\n');
            string currentAppName = null;

            foreach (var line in lines)
            {
                if (line.StartsWith(PathSubstring))
                {
                    currentAppName = ExtractAppName(line);
                }
                else if (line.StartsWith(UtiSubstring) && currentAppName != null)
                {
                    var app = installedAppsDictionary.GetValueOrDefault(currentAppName);
                    if (app is null)
                    {
                        continue;
                    }

                    var utis = ExtractUtis(line);
                    var extensions = utis.SelectMany(MapToExtension).ToImmutableHashSet();

                    foreach (var extension in extensions)
                    {
                        if (!result.ContainsKey(extension))
                        {
                            result[extension] = new HashSet<ApplicationModel>();
                        }

                        result[extension].Add(app);
                    }
                }
            }

            return result;
        }

        private static string ExtractAppName(string line)
        {
            var startIndex = line.IndexOf('/');
            var endIndex = line.IndexOf('(') - 2;

            return startIndex < 0 || endIndex < 0
                ? null :
                line.Substring(startIndex, endIndex - startIndex + 1).Trim();
        }

        private static IEnumerable<string> ExtractUtis(string line) =>
            line
                .Substring(UtiSubstring.Length)
                .Trim()
                .Split(", ", StringSplitOptions.RemoveEmptyEntries);

        private IEnumerable<string> MapToExtension(string uti) =>
            _configuration.UtiToExtensionsMapping.GetValueOrDefault(uti, new List<string>(0));
    }
}