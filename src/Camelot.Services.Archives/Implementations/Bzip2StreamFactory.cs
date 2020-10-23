using System.IO;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;

namespace Camelot.Services.Archives.Implementations
{
    public class Bzip2StreamFactory : IStreamFactory
    {
        public Stream CreateInputStream(Stream inStream) => new BZip2Stream(inStream, CompressionMode.Decompress, true);

        public Stream CreateOutputStream(Stream outStream) => new BZip2Stream(outStream, CompressionMode.Compress, false);
    }
}