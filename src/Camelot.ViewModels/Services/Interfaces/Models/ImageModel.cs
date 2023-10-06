using Avalonia.Media.Imaging;

namespace Camelot.ViewModels.Services.Interfaces.Models;

public class ImageModel
{
    public Bitmap Bitmap { get; }
    
    public ImageModel(Bitmap bitmap)
    {
        Bitmap = bitmap;
    }
}