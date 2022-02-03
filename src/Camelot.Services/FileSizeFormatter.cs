using System;
using System.Globalization;
using Camelot.Services.Abstractions;

namespace Camelot.Services;

public class FileSizeFormatter : IFileSizeFormatter
{
    public string GetFormattedSize(long bytes)
    {
        if (bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), bytes, null);
        }

        string suffix;
        double readable;
        switch (bytes)
        {
            case >= 0x10000000000:
                suffix = "TB";
                readable = bytes >> 30;
                break;
            case >= 0x40000000:
                suffix = "GB";
                readable = bytes >> 20;
                break;
            case >= 0x100000:
                suffix = "MB";
                readable = bytes >> 10;
                break;
            case >= 0x400:
                suffix = "KB";
                readable = bytes;
                break;
            default:
                return bytes.ToString("0 B");
        }

        readable /= 1024;

        return readable.ToString("0.# ", CultureInfo.InvariantCulture) + suffix;
    }

    public string GetSizeAsNumber(long bytes)
    {
        if (bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), bytes, null);
        }

        var numberFormatInfo = new NumberFormatInfo {NumberGroupSeparator = " "};

        return bytes.ToString("#,0", numberFormatInfo);
    }
}