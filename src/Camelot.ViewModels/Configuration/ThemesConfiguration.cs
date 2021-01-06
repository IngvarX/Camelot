using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Configuration
{
    public class ThemesConfiguration
    {
        public Theme DefaultTheme { get; set; }
        
        public Dictionary<Theme, string> ThemeToResourceMapping { get; set; }
    }
}