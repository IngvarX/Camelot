using System.Collections.Generic;

namespace Camelot.Services.Abstractions.Models
{
    public class ApplicationModel
    {
        public string DisplayName { get; set; }

        public string Arguments { get; set; }

        public string ExecutePath { get; set; }
    }

    public class ApplicationModelComparer : IEqualityComparer<ApplicationModel>
    {
        public bool Equals(ApplicationModel x, ApplicationModel y) 
            => x?.DisplayName == y?.DisplayName;

        public int GetHashCode(ApplicationModel obj) => obj.DisplayName.GetHashCode();
    }
}