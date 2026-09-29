namespace DigitalDoor.Reporting.Helpers.Handlers;

internal static class SKPictureHandler
{
    private const int FallbackPictureSizeInPixels = 512;

    public static byte[] PictureToByteArray(
        SKPicture picture,
        SKEncodedImageFormat format = SKEncodedImageFormat.Png,
        int quality = 100,
        int? overrideWidth = null,
        int? overrideHeight = null,
        SKColor backgroundColor = default)
    {
        ArgumentNullException.ThrowIfNull(picture);
        SKRect bounds = picture.CullRect;
        int width = overrideWidth ?? (int)Math.Ceiling(bounds.Width);
        int height = overrideHeight ?? (int)Math.Ceiling(bounds.Height);
        if (width <= 0 || height <= 0)
        {
            width = overrideWidth ?? FallbackPictureSizeInPixels;
            height = overrideHeight ?? FallbackPictureSizeInPixels;
        }

        using SKBitmap bitmap = new SKBitmap(width, height);
        using (SKCanvas canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(backgroundColor == default ? SKColors.Transparent : backgroundColor);
            if (overrideWidth.HasValue || overrideHeight.HasValue)
            {
                DrawPictureScaledToFit(canvas, picture, bounds, width, height);
            }
            else
            {
                canvas.DrawPicture(picture);
            }
            canvas.Flush();
        }
        return ImageHandler.ImageToByteArray(bitmap, format, quality);
    }

    private static void DrawPictureScaledToFit(SKCanvas canvas, SKPicture picture, SKRect bounds, int width, int height)
    {
        float scale = Math.Min((float)width / bounds.Width, (float)height / bounds.Height);
        canvas.Save();
        canvas.Translate(width / 2, height / 2);
        canvas.Scale(scale);
        canvas.Translate(-bounds.MidX, -bounds.MidY);
        canvas.DrawPicture(picture);
        canvas.Restore();
    }
}
