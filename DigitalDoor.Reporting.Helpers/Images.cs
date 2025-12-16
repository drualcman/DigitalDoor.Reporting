namespace DigitalDoor.Reporting.Helpers;

public class Images
{
    public bool TryGetImageBytes(object img, out byte[] bytes)
    {
        bool result;
        try
        {
            if (img is not null)
            {
                if (img is SKImage skImage)
                {
                    bytes = ImageToByteArray(skImage);
                    result = true;
                }
                else if (img is SKBitmap skBitmap)
                {
                    bytes = ImageToByteArray(skBitmap);
                    result = true;
                }
                else if (img is SKPicture skPicture)
                {
                    bytes = ImageToByteArray(skPicture);
                    result = true;
                }
                else if (img is byte[] bytesArray)
                {
                    result = ImageValidator.IsLikelyImage(bytesArray);
                    bytes = result ? bytesArray : null;
                }
                else if (img is string stringImage)
                {
                    bool isSvg = stringImage.TrimStart().StartsWith("<svg", StringComparison.OrdinalIgnoreCase) ||
                                 stringImage.Contains("<svg", StringComparison.OrdinalIgnoreCase);
                    if (isSvg)
                    {
                        string escapedSvg = stringImage.Replace("&lt;", "&amp;lt;");
                        using var skSvg = new SKSvg();
                        skSvg.FromSvg(escapedSvg);
                        if (skSvg.Picture is not null)
                        {
                            bytes = ImageToByteArray(skSvg.Picture);
                            result = true;
                        }
                        else
                        {
                            result = false;
                            bytes = null;
                        }
                    }
                    else
                    {
                        bytes = null!;
                        result = false;
                    }
                }
                else
                {
                    bytes = null!;
                    result = false;
                }
            }
            else
            {
                bytes = null!;
                result = false;
            }
        }
        catch
        {
            bytes = null!;
            result = false;
        }
        return result;
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