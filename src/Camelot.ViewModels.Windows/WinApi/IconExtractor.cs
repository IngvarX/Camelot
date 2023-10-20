using System.Drawing;
using System.Runtime.InteropServices;

namespace Camelot.ViewModels.Windows.WinApi;

internal class IconExtractor
{
    public static Icon ExtractIcon(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        Icon result;

        // See if path contains seperator '?' or ',' for index inside file.
        // (both ? and , are used, depending on app)
        // namely the icon is a numbered/index in a file with more icons,
        // or the file contains just one icon.
        var delimiter = FindIndexDelimiter(filename);
        if (delimiter >= 0)
        {
            var iconPath = filename[..delimiter];
            iconPath = iconPath.Trim('"');
            var strIndex = filename[(delimiter + 1)..];
            var index = int.Parse(strIndex);
            result = ExtractIconImpl(iconPath, index, true);
        }
        else
        {
            result = ExtractIconImpl(filename);
        }

        return result;
    }

    private static int FindIndexDelimiter(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        var questionMark = filename.IndexOf('?');
        if (questionMark >= 0)
        {
            return questionMark;
        }

        var comma = filename.IndexOf(',');
        if (comma >= 0)
        {
            return comma;
        }

        return -1;
    }
    private static Icon ExtractIconImpl(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
        {
            throw new ArgumentException(nameof(fileName));
        }

        return Icon.ExtractAssociatedIcon(fileName);
    }

    // needed for indexed icons (when more than one in file, and there's a number/index for the icon we need)
    // src: https://stackoverflow.com/questions/6872957/how-can-i-use-the-images-within-shell32-dll-in-my-c-sharp-project
    private static Icon ExtractIconImpl(string fileName, int number, bool largeIcon)
    {
        if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
        {
            throw new ArgumentOutOfRangeException(nameof(fileName));
        }

        try
        {
            IntPtr large;
            IntPtr small;
            Icon result;
            ExtractIconEx(fileName, number, out large, out small, 1);

            // no need to destroy after "FromHandle", but need to free the non-used one
            // https://stackoverflow.com/questions/30979653/icon-fromhandle-should-i-dispose-it-or-call-destroyicon
            if (largeIcon)
            {
                result = Icon.FromHandle(large);
                DestroyIcon(small);
            }
            else
            {
                result = Icon.FromHandle(small);
                DestroyIcon(large);
            }

            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);
}
