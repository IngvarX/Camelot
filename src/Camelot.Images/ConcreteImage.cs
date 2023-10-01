using Avalonia.Media.Imaging;
using Camelot.Services.Abstractions.Models;


namespace Camelot.Images;

public class ConcreteImage : ImageModel
{
    public Bitmap Bitmap { get; }
    public ConcreteImage(Bitmap bitmap)
    {
        Bitmap = bitmap;
    }
}