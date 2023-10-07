using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Camelot.ViewModels.Windows.ShellIcons;

public class UWPIcon
{
    // "PRI" = "Package resource indexing"
    // https://docs.microsoft.com/en-us/windows/uwp/app-resources/pri-apis-scenario-1
    public static string ReslovePackageResource(string pri)
    {
        if (string.IsNullOrEmpty(pri))
        {
            throw new ArgumentNullException(nameof(pri));
        }

        if (!IsPriString(pri))
        {
            throw new ArgumentException(nameof(pri));
        }

        var outBuffer = new StringBuilder(512);
        var capacity = (uint)outBuffer.Capacity;
        var hResult = SHLoadIndirectString(pri, outBuffer, capacity, IntPtr.Zero);

        return hResult == Hresult.Ok ? outBuffer.ToString() : null;
    }

    public static bool IsPriString(string pri)
    {
        if (string.IsNullOrEmpty(pri))
        {
            throw new ArgumentNullException(nameof(pri));
        }

        if (!pri.StartsWith("@{"))
        {
            return false;
        }

        if (!pri.EndsWith("}"))
        {
            return false;
        }

        if (!pri.Contains("ms-resource:"))
        {
            return false;
        }

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