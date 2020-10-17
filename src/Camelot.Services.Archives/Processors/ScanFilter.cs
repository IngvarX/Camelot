using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Core;

namespace Camelot.Services.Archives.Processors
{
    public class ScanFilter : IScanFilter
    {
        private readonly ISet<string> _names;

        public ScanFilter(ISet<string> names)
        {
            _names = names;
        }

        public bool IsMatch(string name) => _names.Contains(name);
    }
}