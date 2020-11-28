using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;

namespace Camelot.ViewModels.Implementations.Dialogs.Comparers
{
    public class ApplicationModelComparer : IEqualityComparer<ApplicationModel>
    {
        public bool Equals(ApplicationModel x, ApplicationModel y) =>
            x?.DisplayName == y?.DisplayName;

        public int GetHashCode(ApplicationModel model) => model.DisplayName.GetHashCode();
    }
}