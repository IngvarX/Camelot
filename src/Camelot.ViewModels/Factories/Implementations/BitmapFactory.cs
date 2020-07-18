using Avalonia.Media.Imaging;
using Camelot.ViewModels.Factories.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class BitmapFactory : IBitmapFactory
    {
        public IBitmap Create(string filePath) => new Bitmap(filePath);
    }
}