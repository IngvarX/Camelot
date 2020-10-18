using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Camelot.ViewModels.Implementations;
using Camelot.Views;
using Xunit;

namespace Camelot.Tests
{
    public class TestViewModelsAndViews
    {
        [Fact]
        public void TestDialogViewsAndViewModels()
        {
            var viewModelsAssembly = Assembly.GetAssembly(typeof(ViewModelBase));
            var dialogViewModelTypes = GetTypes(viewModelsAssembly, "DialogViewModel");
            Assert.NotEmpty(dialogViewModelTypes);

            var viewsAssembly = Assembly.GetAssembly(typeof(ViewLocator));
            var dialogViewTypes = GetTypes(viewsAssembly, "Dialog");
            Assert.NotEmpty(dialogViewTypes);

            Assert.Equal(dialogViewTypes.Count, dialogViewModelTypes.Count);

            foreach (var dialogViewType in dialogViewTypes)
            {
                var viewType = dialogViewType.Replace("DialogViewModel", "Dialog");
                Assert.True(dialogViewTypes.Contains(viewType));
            }
        }

        private static ISet<string> GetTypes(Assembly assembly, string filter) =>
            assembly
                .GetTypes()
                .Where(t => t.FullName.EndsWith(filter))
                .Select(t => t.Name)
                .ToHashSet();
    }
}