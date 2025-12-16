namespace DigitalDoor.Reporting.Helpers.Handlers;

internal static class SKPictureHandler
{
    public static byte[] PictureToByteArray(
        SKPicture picture,
        SKEncodedImageFormat format = SKEncodedImageFormat.Png,
        int quality = 100,
        int? overrideWidth = null,
        int? overrideHeight = null,
        SKColor backgroundColor = default)
    {
        if (picture == null)
            throw new ArgumentNullException(nameof(picture));

        var bounds = picture.CullRect;

        int width = overrideWidth ?? (int)Math.Ceiling(bounds.Width);
        int height = overrideHeight ?? (int)Math.Ceiling(bounds.Height);

        // Fallback
        if (width <= 0 || height <= 0)
        {
            width = overrideWidth ?? 512;
            height = overrideHeight ?? 512;
        }

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        // Background Color: default transparent
        SKColor clearColor = backgroundColor == default ? SKColors.Transparent : backgroundColor;
        canvas.Clear(clearColor);

        // Optional: scale when overwrite width or height
        if (overrideWidth.HasValue || overrideHeight.HasValue)
        {
            var scaleX = (float)width / bounds.Width;
            var scaleY = (float)height / bounds.Height;
            var scale = Math.Min(scaleX, scaleY);

            canvas.Save();
            canvas.Translate(width / 2, height / 2);
            canvas.Scale(scale);
            canvas.Translate(-bounds.MidX, -bounds.MidY);
            canvas.DrawPicture(picture);
            canvas.Restore();
        }
        else
        {
            canvas.DrawPicture(picture);
        }

        canvas.Flush();

        return ImageHandler.ImageToByteArray(bitmap, format, quality);
    }
}
