using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Configuration
{
    public class ThemesNamesConfiguration
    {
        public Dictionary<Theme, string> ThemeToResourceMapping { get; set; }
    }
}