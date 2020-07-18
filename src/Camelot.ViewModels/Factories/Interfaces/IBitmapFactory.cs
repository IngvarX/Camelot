using Avalonia.Media.Imaging;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IBitmapFactory
    {
        IBitmap Create(string filePath);
    }
}