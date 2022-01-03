namespace Camelot.DataAccess.Models;

public class CreateArchiveSettings
{
    public static CreateArchiveSettings Empty => new();

    public int ArchiveType { get; set; }
}