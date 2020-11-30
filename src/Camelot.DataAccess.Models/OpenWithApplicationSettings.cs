using System;
using System.Collections.Generic;

namespace Camelot.DataAccess.Models
{
    public class OpenWithApplicationSettings
    {
        public static OpenWithApplicationSettings Empty => new OpenWithApplicationSettings();

        public Dictionary<string, Application> ApplicationByExtension { get; set; }

        public OpenWithApplicationSettings()
        {
            ApplicationByExtension = new Dictionary<string, Application>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
