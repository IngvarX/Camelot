using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using AppxPackaing;
//using Shell;
//using Rect = System.Windows.Rect;
//using LoggerLib;

namespace Camelot.Services.Windows.ShellIcons;

[SupportedOSPlatform("windows")]
public class UWPIcon
{
    // "PRI" = "Package resource indexing"
    // https://docs.microsoft.com/en-us/windows/uwp/app-resources/pri-apis-scenario-1
    static public string ReslovePackageResource(string pri)
    {
        if (string.IsNullOrEmpty(pri))
            throw new ArgumentNullException(nameof(pri));
        if (!IsPriString(pri))
            throw new ArgumentOutOfRangeException(nameof(pri));

        var outBuffer = new StringBuilder(512);
        string source = pri;
        var capacity = (uint)outBuffer.Capacity;
        var hResult = SHLoadIndirectString(source, outBuffer, capacity, IntPtr.Zero);
        if (hResult == Hresult.Ok)
            return outBuffer.ToString();
        return null;
    }

    static public bool IsPriString(string pri)
    {
        if (string.IsNullOrEmpty(pri))
            throw new ArgumentNullException(nameof(pri));

        if (!pri.StartsWith("@{"))
            return false;
        if (!pri.EndsWith("}"))
            return false;
        if (!pri.Contains("ms-resource:"))
            return false;
        return true;
    }

    private enum Hresult : uint
    {
        Ok = 0x0000,
    }
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern Hresult SHLoadIndirectString(string pszSource,
        StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);
}