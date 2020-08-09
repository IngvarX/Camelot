using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Camelot.DependencyInjection;
using Camelot.ViewModels.Implementations;
using Splat;
using Xunit;

namespace Camelot.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void TestRegistrations()
        {
            var resolver = new MutableDependencyResolver(Locator.CurrentMutable);
            Bootstrapper.Register(resolver, Locator.Current);

            resolver
                .RegisteredTypes
                .ForEach(type => Locator.Current.GetRequiredService(type));
        }

        [Fact]
        public void TestDialogRegistrations()
        {
            var resolver = new MutableDependencyResolver(Locator.CurrentMutable);
            Bootstrapper.Register(resolver, Locator.Current);

            var viewModelsAssembly = Assembly.GetAssembly(typeof(ViewModelBase));
            var dialogTypes = viewModelsAssembly
                .GetTypes()
                .Where(t => t.FullName.EndsWith("DialogViewModel"))
                .ToArray();
            Assert.NotEmpty(dialogTypes);

            var areAllDialogsRegistered = dialogTypes.All(t => resolver.RegisteredTypes.Contains(t));
            Assert.True(areAllDialogsRegistered);
        }

        private class MutableDependencyResolver : IMutableDependencyResolver
        {
            private readonly IMutableDependencyResolver _inner;

            public List<Type> RegisteredTypes { get; }

            public MutableDependencyResolver(IMutableDependencyResolver inner)
            {
                _inner = inner;

                RegisteredTypes = new List<Type>();
            }

            public bool HasRegistration(Type serviceType, string contract = null) =>
                _inner.HasRegistration(serviceType, contract);

            public void Register(Func<object> factory, Type serviceType, string contract = null)
            {
                RegisteredTypes.Add(serviceType);

                _inner.Register(factory, serviceType, contract);
            }

            public void UnregisterCurrent(Type serviceType, string contract = null) =>
                _inner.UnregisterCurrent(serviceType, contract);

            public void UnregisterAll(Type serviceType, string contract = null) =>
                _inner.UnregisterAll(serviceType, contract);

            public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback) =>
                _inner.ServiceRegistrationCallback(serviceType, contract, callback);
        }
    }
}