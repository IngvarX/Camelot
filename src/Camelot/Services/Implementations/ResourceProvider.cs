using System;
using Camelot.Properties;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class ResourceProvider : IResourceProvider
    {
        public string GetResourceByName(string name) =>
            Resources.ResourceManager.GetString(name) ?? throw new ArgumentException(name);
    }
}