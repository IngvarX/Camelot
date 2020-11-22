using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.Services.Linux
{
    public class LinuxApplicationService : IApplicationService
    {
        private readonly IFileService _fileService;

        public LinuxApplicationService(IFileService fileService)
        {
            _fileService = fileService;
        }
        
        public async Task<IEnumerable<ApplicationModel>> GetAssociatedApplications(string fileExtension)
        {
            var desktopEntries = await GetDesktopEntries();

            var applications = desktopEntries
                .Where(m => m.Extensions.Contains(fileExtension));
            return applications;
        }

        public async Task<IEnumerable<ApplicationModel>> GetInstalledApplications() 
            => await GetDesktopEntries();

        private static string ExtractArguments(string startCommand) => "{0}"; // TODO: fix %F => {0}

        private static string ExtractExecutePath(string startCommand) =>
            startCommand.Split().FirstOrDefault();

        private async Task<List<LinuxApplicationModel>> GetDesktopEntries()
        {
            var desktopEntryFiles = new List<LinuxApplicationModel>();

            var specification = GetSpecification();

            var desktopFilePaths = _fileService
                .GetFiles("/usr/share/applications/", specification)
                .Select(f => f.FullPath);

            var iniReader = new IniReader();

            var mimeTypesFile = _fileService.OpenRead("D:\\Download\\mime.types");
            var mimeTypesExtensions = await new MimeTypesReader().Read(mimeTypesFile);

            foreach (var desktopFilePath in desktopFilePaths)
            {
                await using var desktopFile = _fileService.OpenRead(desktopFilePath);
                var desktopEntry = await iniReader.Read(desktopFile);

                var desktopType = desktopEntry.GetValueOrDefault("Desktop Entry:Type");
                if (desktopType is null || !desktopType.Equals("Application", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var displayName = desktopEntry.GetValueOrDefault("Desktop Entry:Name");
                var startCommand = desktopEntry.GetValueOrDefault("Desktop Entry:Exec");

                if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(startCommand))
                {
                    continue;
                }

                var executePath = ExtractExecutePath(startCommand);
                if (string.IsNullOrWhiteSpace(executePath))
                {
                    continue;
                }

                var arguments = ExtractArguments(startCommand);

                var extensions = new List<string>();
                var mimeTypes = desktopEntry.GetValueOrDefault("Desktop Entry:MimeType")?.Split(',');
                if (mimeTypes != null)
                {
                    foreach (var mimeType in mimeTypes)
                    {
                        var extensionsByMimeType = mimeTypesExtensions.GetValueOrDefault(mimeType);
                        if (extensionsByMimeType != null)
                        {
                            extensions.AddRange(extensionsByMimeType);
                        }
                    }
                }

                desktopEntryFiles.Add(new LinuxApplicationModel
                {
                    DisplayName = displayName,
                    ExecutePath = executePath,
                    Arguments = arguments,
                    Extensions = extensions
                });
            }

            return desktopEntryFiles;
        }

        private static ISpecification<FileModel> GetSpecification() => new DesktopFileSpecification();
        
        private class DesktopFileSpecification : ISpecification<FileModel>
        {
            private const string Extension = "desktop";

            public bool IsSatisfiedBy(FileModel fileModel) => fileModel.Extension == Extension;
        }

        private class LinuxApplicationModel : ApplicationModel
        {
            public IEnumerable<string> Extensions { get; set; }
        }

        private class MimeTypesReader
        {
            public async Task<Dictionary<string, List<string>>> Read(Stream stream)
            {
                var data = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                using var streamReader = new StreamReader(stream);
                while (streamReader.Peek() != -1)
                {
                    var rawLine = await streamReader.ReadLineAsync();
                    var line = rawLine?.Trim();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    if (!char.IsLetter(line[0]))
                    {
                        continue;
                    }

                    var separator = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    var key = separator[0];
                    if (!key.Contains("/"))
                    {
                        continue;
                    }

                    var values = separator.Skip(1);

                    if (!data.ContainsKey(key))
                    {
                        data[key] = new List<string>(values);
                    }
                    else
                    {
                        data[key].AddRange(values);
                    }
                }

                return data;
            }
        }

        private class IniReader
        {
            public async Task<Dictionary<string, string>> Read(Stream stream)
            {
                const string keyDelimiter = ":";

                var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                using var reader = new StreamReader(stream);
                var sectionPrefix = string.Empty;

                while (reader.Peek() != -1)
                {
                    var rawLine = await reader.ReadLineAsync();
                    var line = rawLine?.Trim();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    switch (line[0])
                    {
                        case ';':
                        case '#':
                        case '/':
                            continue;
                        case '[' when line[^1] == ']':
                            sectionPrefix = line.Substring(1, line.Length - 2) + keyDelimiter;
                            continue;
                    }

                    var separator = line.IndexOf('=');
                    if (separator < 0)
                    {
                        continue;
                    }

                    var key = sectionPrefix + line.Substring(0, separator).Trim();
                    var value = line.Substring(separator + 1).Trim();

                    if (value.Length > 1 && value[0] == '"' && value[^1] == '"')
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    if (!data.ContainsKey(key))
                    {
                        data[key] = value;
                    }
                }

                return data;
            }
        }
    }
}
