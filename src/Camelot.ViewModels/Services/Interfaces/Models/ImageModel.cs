using Avalonia.Media.Imaging;

namespace Camelot.ViewModels.Services.Interfaces.Models;

public class ImageModel
{
    public IBitmap Bitmap { get; }

    public ImageModel(IBitmap bitmap)
    {
        Bitmap = bitmap;
    }
}