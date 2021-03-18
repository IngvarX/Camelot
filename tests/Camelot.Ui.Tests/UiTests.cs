using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Threading;
using Xunit;

namespace Camelot.Ui.Tests
{
    public class UiTests : IDisposable
    {
        private const int DelayMilliseconds = 100;

        private readonly IReadOnlyList<IUiTest> _tests;

        public UiTests()
        {
            _tests = GetTests();
        }

        [Fact]
        public void TestUi()
        {
            AvaloniaApp.RegisterDependencies();
            AvaloniaApp
                .BuildAvaloniaApp()
                .AfterSetup(_ =>
                {
                    DispatcherTimer.RunOnce(() =>
                    {
                        var window = AvaloniaApp.GetMainWindow();

                        foreach (var test in _tests)
                        {
                            test.Execute(window);

                            if (test is IDisposable disposable)
                            {
                                disposable.Dispose();
                            }
                        }

                        window.Close();
                    }, TimeSpan.FromMilliseconds(DelayMilliseconds));
                })
                .StartWithClassicDesktopLifetime(new string[0]);
        }

        public void Dispose() => AvaloniaApp.Stop();

        private IReadOnlyList<IUiTest> GetTests()
        {
            var assembly = GetType().Assembly;

            return assembly
                .GetTypes()
                .Where(t => t.IsClass && typeof(IUiTest).IsAssignableFrom(t))
                .Select(t => (IUiTest) Activator.CreateInstance(t))
                .ToArray();
        }
    }
}