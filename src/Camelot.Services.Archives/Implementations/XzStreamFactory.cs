using System.IO;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Compressors.Xz;

namespace Camelot.Services.Archives.Implementations
{
    public class XzStreamFactory : IStreamFactory
    {
        public Stream CreateInputStream(Stream inStream) => new XZStream(inStream);

        public Stream CreateOutputStream(Stream outStream) => new XZStream(outStream);
    }
}