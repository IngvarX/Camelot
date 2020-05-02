namespace Camelot.Services.Models.Operations
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