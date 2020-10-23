using System.IO;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace Camelot.Services.Archives.Implementations
{
    public class GzipStreamFactory : IStreamFactory
    {
        public Stream CreateInputStream(Stream inStream) => new GZipStream(inStream, CompressionMode.Decompress);

        public Stream CreateOutputStream(Stream outStream) => new GZipStream(outStream, CompressionMode.Compress);
    }
}