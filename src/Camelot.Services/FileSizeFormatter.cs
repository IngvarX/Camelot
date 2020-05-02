using System;
using System.Globalization;
using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class FileSizeFormatter : IFileSizeFormatter
    {
        public string GetFormattedSize(long bytes)
        {
            if (bytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            string suffix;
            double readable;
            if (bytes >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = bytes >> 30;
            }
            else if (bytes >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = bytes >> 20;
            }
            else if (bytes >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = bytes >> 10;
            }
            else if (bytes >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = bytes;
            }
            else
            {
                return bytes.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable /= 1024;

            // Return formatted number with suffix
            return readable.ToString("0.# ", CultureInfo.InvariantCulture) + suffix;
        }
    }
}