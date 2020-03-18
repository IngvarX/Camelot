namespace Camelot.Services.Interfaces
{
    public interface IFileSizeFormatter
    {
        string GetFormattedSize(long bytes);
    }
}