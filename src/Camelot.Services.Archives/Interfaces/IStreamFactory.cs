using System.IO;

namespace Camelot.Services.Archives.Interfaces
{
    public interface IStreamFactory
    {
        Stream Create(Stream inputStream);
    }
}