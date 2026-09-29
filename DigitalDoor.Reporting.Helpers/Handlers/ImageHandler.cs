namespace DigitalDoor.Reporting.Helpers.Handlers;

internal static class ImageHandler
{
    public static byte[] ImageToByteArray(SKImage imageIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
    {
        using SKData encodedData = imageIn.Encode(format, quality) ??
            throw new InvalidOperationException($"The image cannot be encoded as {format}.");
        return encodedData.ToArray();
    }

    public static byte[] ImageToByteArray(SKBitmap bitmapIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
    {
        using SKImage image = SKImage.FromBitmap(bitmapIn);
        return ImageToByteArray(image, format, quality);
    }

    public static SKImage ByteArrayToImage(byte[] byteArrayIn)
    {
        using MemoryStream imageStream = new MemoryStream(byteArrayIn);
        return SKImage.FromEncodedData(imageStream);
    }

    public static SKImage StreamToImage(Stream stream) => SKImage.FromEncodedData(stream);
}
