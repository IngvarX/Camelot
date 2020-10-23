using System.IO;

namespace Camelot.Services.Archives.Interfaces
{
    public interface IStreamFactory
    {
        Stream CreateInputStream(Stream inStream);

        Stream CreateOutputStream(Stream outStream);
    }
}