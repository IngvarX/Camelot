using System.IO;
using Camelot.Services.Archives.Interfaces;
using ICSharpCode.SharpZipLib.BZip2;

namespace Camelot.Services.Archives.Implementations
{
    public class Bzip2StreamFactory : IStreamFactory
    {
        public Stream CreateInputStream(Stream inStream) => new BZip2InputStream(inStream);

        public Stream CreateOutputStream(Stream outStream) => new BZip2OutputStream(outStream);
    }
}