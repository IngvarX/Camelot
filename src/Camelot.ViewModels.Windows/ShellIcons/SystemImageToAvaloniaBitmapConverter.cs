using System.Drawing.Imaging;
using System.Runtime.Versioning;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using Rectangle = System.Drawing.Rectangle;
using AvaloniaPixelFormat = Avalonia.Platform.PixelFormat;
using AlphaFormat = Avalonia.Platform.AlphaFormat;
using PixelSize = Avalonia.PixelSize;
using Vector = Avalonia.Vector;


namespace Camelot.ViewModels.Windows.ShellIcons;

[SupportedOSPlatform("windows")]

internal class SystemImageToAvaloniaBitmapConverter
{
    // based on: https://github.com/AvaloniaUI/Avalonia/discussions/5908
    public static AvaloniaBitmap Convert(System.Drawing.Image image)
    {
        if (image == null)
            return null;
        var bitmapTmp = new System.Drawing.Bitmap(image);
        var bitmapdata = bitmapTmp.LockBits(
            new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height),
            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        AvaloniaBitmap bitmap1 = new AvaloniaBitmap(AvaloniaPixelFormat.Bgra8888,
            AlphaFormat.Unpremul,
            bitmapdata.Scan0,
            new PixelSize(bitmapdata.Width, bitmapdata.Height),
            new Vector(96, 96),
            bitmapdata.Stride);
        bitmapTmp.UnlockBits(bitmapdata);
        bitmapTmp.Dispose();
        return bitmap1;
    }
}
