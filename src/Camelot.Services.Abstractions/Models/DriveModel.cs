namespace Camelot.Services.Abstractions.Models
{
    public class DriveModel
    {
        public string Name { get; set; }

        public string RootDirectory { get; set; }

        public long FreeSpaceBytes { get; set; }

        public long TotalSpaceBytes { get; set; }
    }
}