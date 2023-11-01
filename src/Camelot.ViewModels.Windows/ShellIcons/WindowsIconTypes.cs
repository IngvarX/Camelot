namespace Camelot.ViewModels.Windows.ShellIcons
{
    public static class WindowsIconTypes
    {
        public static bool IsIconThatRequiresExtract(string fileName, string extension)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            if (string.IsNullOrEmpty(extension)) 
            {
                throw new ArgumentNullException(nameof(extension));
            }
            if (fileName.Contains('?') || fileName.Contains(','))
            {
                return true; // icon in indexed resource
            }
            var extensionsOfIcoFiles = new List<string> { "ico", "exe", "dll" , "cpl", "appref-ms", "msc" };

            return extensionsOfIcoFiles.Contains(extension);
        }
    }
}
