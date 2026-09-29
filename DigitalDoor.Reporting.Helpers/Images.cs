namespace DigitalDoor.Reporting.Helpers;

public class Images
{
    public bool TryGetImageBytes(object img, out byte[] bytes)
    {
        bytes = img switch
        {
            SKImage skImage => ImageToByteArray(skImage),
            SKBitmap skBitmap => ImageToByteArray(skBitmap),
            SKPicture skPicture => ImageToByteArray(skPicture),
            byte[] bytesArray => GetImageBytesOrNull(bytesArray),
            string svgText when SvgValidator.IsSvg(svgText) => SvgRasterHandler.SvgToPngOrNull(svgText),
            _ => null
        };
        return bytes is not null;
    }

    public bool TryGetSvg(object img, out string svg)
    {
        svg = null;
        if (img is SKPicture skPicture)
        {
            svg = SKPictureSvgHandler.PictureToSvg(skPicture);
        }
        else if (SvgValidator.TryGetSvg(img, out string svgText))
        {
            svg = svgText;
        }
        return svg is not null;
    }

    private static byte[] GetImageBytesOrNull(byte[] imageBytes)
    {
        byte[] result = null;
        if (ImageValidator.IsLikelyImage(imageBytes))
        {
            result = NormalizeImageBytes(imageBytes);
        }
        return result;
    }

    private static byte[] NormalizeImageBytes(byte[] imageBytes)
    {
        byte[] normalizedBytes = imageBytes;
        if (!ImageValidator.HasImageHeader(imageBytes) &&
            ImageValidator.TryDecodeBase64Image(Encoding.UTF8.GetString(imageBytes), out byte[] decodedBytes))
        {
            normalizedBytes = decodedBytes;
        }
        return normalizedBytes;
    }

    public byte[] ImageToByteArray(SKImage imageIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100) =>
        ImageHandler.ImageToByteArray(imageIn, format, quality);

    public byte[] ImageToByteArray(SKBitmap bitmapIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100) =>
        ImageHandler.ImageToByteArray(bitmapIn, format, quality);

    public byte[] ImageToByteArray(SKPicture bitmapIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100, int? overrideWidth = null, int? overrideHeight = null, SKColor backgroundColor = default) =>
        SKPictureHandler.PictureToByteArray(bitmapIn, format, quality, overrideWidth, overrideHeight, backgroundColor);

    public SKImage ByteArrayToImage(byte[] byteArrayIn) =>
        ImageHandler.ByteArrayToImage(byteArrayIn);

    public SKImage StreamToImage(Stream stream) =>
        ImageHandler.StreamToImage(stream);
}
