namespace Camelot.Services.Operations.Settings
{
    public class UnaryFileOperationSettings
    {
        public string FilePath { get; }

        public UnaryFileOperationSettings(string filePath)
        {
            FilePath = filePath;
        }
    }
}