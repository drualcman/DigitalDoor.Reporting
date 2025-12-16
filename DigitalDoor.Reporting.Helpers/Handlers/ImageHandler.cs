namespace DigitalDoor.Reporting.Helpers.Handlers;

internal static class ImageHandler
{
    public static byte[] ImageToByteArray(SKImage imageIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
    {
        using var ms = new MemoryStream();
        imageIn.Encode(format, quality).SaveTo(ms);
        return ms.ToArray();
    }

    public static byte[] ImageToByteArray(SKBitmap bitmapIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
    {
        using var ms = new MemoryStream();
        using var image = SKImage.FromBitmap(bitmapIn);
        image.Encode(format, quality).SaveTo(ms);
        return ms.ToArray();
    }

    public static SKImage ByteArrayToImage(byte[] byteArrayIn)
    {
        using var ms = new MemoryStream(byteArrayIn);
        return SKImage.FromEncodedData(ms);
    }

    public static SKImage StreamToImage(Stream stream) => SKImage.FromEncodedData(stream);
}
