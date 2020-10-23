using System.IO;
using Camelot.Services.Archives.Interfaces;
using ICSharpCode.SharpZipLib.GZip;

namespace Camelot.Services.Archives.Implementations
{
    public class GzipStreamFactory : IStreamFactory
    {
        public Stream CreateInputStream(Stream inStream) => new GZipInputStream(inStream);

        public Stream CreateOutputStream(Stream outStream) => new GZipOutputStream(outStream);
    }
}