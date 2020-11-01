namespace Camelot.DataAccess.Models
{
    public class CreateArchiveState
    {
        public static CreateArchiveState Empty => new CreateArchiveState();

        public int ArchiveType { get; set; }
    }
}