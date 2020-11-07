using System;
using System.Collections.Generic;

namespace Camelot.DataAccess.Models
{
    public class OpenWithApplicationSettings
    {
        public Dictionary<string, Application> ApplicationByExtension { get; set; } =
            new Dictionary<string, Application>(StringComparer.OrdinalIgnoreCase);
    }

    public class Application
    {
        public string DisplayName { get; set; }

        public string Arguments { get; set; }

        public string ExecutePath { get; set; }
    }
}
