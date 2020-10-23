using System.IO;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Compressors;
using SharpCompress.Compressors.LZMA;

namespace Camelot.Services.Archives.Implementations
{
    public class LzStreamFactory : IStreamFactory
    {
        public Stream CreateInputStream(Stream inStream) => new LZipStream(inStream, CompressionMode.Decompress);

        public Stream CreateOutputStream(Stream outStream) => new LZipStream(outStream, CompressionMode.Decompress);
    }
}