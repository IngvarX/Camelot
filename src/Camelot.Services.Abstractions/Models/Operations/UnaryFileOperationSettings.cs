namespace Camelot.Services.Abstractions.Models.Operations
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