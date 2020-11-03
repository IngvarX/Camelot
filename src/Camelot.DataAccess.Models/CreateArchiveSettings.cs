namespace Camelot.DataAccess.Models
{
    public class CreateArchiveSettings
    {
        public static CreateArchiveSettings Empty => new CreateArchiveSettings();

        public int ArchiveType { get; set; }
    }
}