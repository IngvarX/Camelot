using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacSoftwareService : ISoftwareService
    {
        private readonly IProcessService _processService;
        private readonly ITerminalService _terminalService;

        public MacSoftwareService(IProcessService processService, ITerminalService terminalService)
        {
            _processService = processService;
            _terminalService = terminalService;
        }

        public async Task<IEnumerable<SoftwareModel>> GetAllInstalledSoftwares()
        {
            var (command, arguments) = _terminalService.GetTerminalSettings();
            var applicationsJson = await _processService.ExecuteAndGetOutputAsync(
                command, string.Format(arguments, "system_profiler -json SPApplicationsDataType");

            var applications = JsonSerializer.Deserialize<IList<MacApplicationDataType>>(applicationsJson);

            var installedSoftwares = applications
                .Select(app => new SoftwareModel
                {
                    DisplayName = app.Name,
                    DisplayVersion = app.Version,
                    InstallLocation = app.Path
                });

            return installedSoftwares;
        }

        private class MacApplicationDataType
        {
            [JsonPropertyName("_name")]
            public string Name { get; set; }

            [JsonPropertyName("path")]
            public string Path { get; set; }

            [JsonPropertyName("version")]
            public string Version { get; set; }
        }
    }
}
