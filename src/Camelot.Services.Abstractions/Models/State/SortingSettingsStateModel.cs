using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.State
{
    public class SortingSettingsStateModel
    {
        public SortingMode SortingMode { get; set; }

        public bool IsAscending { get; set; }
    }
}