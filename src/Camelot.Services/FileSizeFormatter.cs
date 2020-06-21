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
            if (bytes >= 0x10000000000)
            {
                suffix = "TB";
                readable = bytes >> 30;
            }
            else if (bytes >= 0x40000000)
            {
                suffix = "GB";
                readable = bytes >> 20;
            }
            else if (bytes >= 0x100000)
            {
                suffix = "MB";
                readable = bytes >> 10;
            }
            else if (bytes >= 0x400)
            {
                suffix = "KB";
                readable = bytes;
            }
            else
            {
                return bytes.ToString("0 B");
            }

            readable /= 1024;

            return readable.ToString("0.# ", CultureInfo.InvariantCulture) + suffix;
        }

        public string GetSizeAsNumber(long bytes)
        {
            if (bytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            var numberFormatInfo = new NumberFormatInfo {NumberGroupSeparator = " "};

            return bytes.ToString("#,0", numberFormatInfo);
        }
    }
}