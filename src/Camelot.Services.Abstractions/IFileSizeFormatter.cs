namespace Camelot.Services.Abstractions;

public interface IFileSizeFormatter
{
    string GetFormattedSize(long bytes);

    string GetSizeAsNumber(long bytes);
}