using System.Drawing.Imaging;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using Rectangle = System.Drawing.Rectangle;
using AvaloniaPixelFormat = Avalonia.Platform.PixelFormat;
using AlphaFormat = Avalonia.Platform.AlphaFormat;
using PixelSize = Avalonia.PixelSize;
using Vector = Avalonia.Vector;

namespace Camelot.ViewModels.Windows.ShellIcons;

internal class SystemImageToAvaloniaBitmapConverter
{
    // based on: https://github.com/AvaloniaUI/Avalonia/discussions/5908
    public static AvaloniaBitmap Convert(System.Drawing.Image image)
    {
        if (image is null)
        {
            return null;
        }

        var bitmapTmp = new System.Drawing.Bitmap(image);
        var bitmapData = bitmapTmp.LockBits(
            new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height),
            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        var bitmap = new AvaloniaBitmap(AvaloniaPixelFormat.Bgra8888,
            AlphaFormat.Unpremul,
            bitmapData.Scan0,
            new PixelSize(bitmapData.Width, bitmapData.Height),
            new Vector(96, 96),
            bitmapData.Stride);
        bitmapTmp.UnlockBits(bitmapData);
        bitmapTmp.Dispose();

        return bitmap;
    }
}
