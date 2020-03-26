namespace Camelot.Services.Operations.Settings
{
    public class BinaryFileOperationSettings
    {
        public string SourceFilePath { get; }

        public string DestinationFilePath { get; }

        public BinaryFileOperationSettings(string sourceFilePath, string destinationFilePath)
        {
            SourceFilePath = sourceFilePath;
            DestinationFilePath = destinationFilePath;
        }
    }
}