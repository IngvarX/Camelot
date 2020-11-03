using System.IO;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Compressors;
using SharpCompress.Compressors.LZMA;

namespace Camelot.Services.Archives.Implementations
{
    public class LzipStreamFactory : IStreamFactory
    {
        public Stream Create(Stream inputStream) => new LZipStream(inputStream, CompressionMode.Decompress);
    }
}