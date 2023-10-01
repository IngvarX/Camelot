using System;
using System.Collections.Generic;
using System.IO;

namespace Camelot.Services.Windows.ShellIcons
{
    public class WindowsIconTypes
    {
        public static bool IsIconThatRequiresExtract(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (filename.Contains("?"))
                return true; // icon in indexed resource
            if (filename.Contains(","))
                return true; // icon in indexed resource

            var ext = Path.GetExtension(filename).ToLower();
            var extensionsOfIcoFiles = new List<string>() { ".ico", ".exe", ".dll" , ".cpl", ".appref-ms", ".msc" };
            if (extensionsOfIcoFiles.Contains(ext))
                return true;
            return false;
        }
    }
}
