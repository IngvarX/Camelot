using System.IO;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;

namespace Camelot.Services.Archives.Implementations
{
    public class Bz2StreamFactory : IStreamFactory
    {
        public Stream Create(Stream inputStream) =>
            new BZip2Stream(inputStream, CompressionMode.Decompress, false);
    }
}