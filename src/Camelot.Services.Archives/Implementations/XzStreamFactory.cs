using System.IO;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Compressors.Xz;

namespace Camelot.Services.Archives.Implementations
{
    public class XzStreamFactory : IStreamFactory
    {
        public Stream Create(Stream inputStream) => new XZStream(inputStream);
    }
}