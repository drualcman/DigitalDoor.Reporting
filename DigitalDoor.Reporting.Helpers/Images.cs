namespace DigitalDoor.Reporting.Helpers;

public class Images
{
    public bool TryGetImageBytes(object img, out byte[] bytes)
    {
        bool result;
        try
        {
            if(img != null && img is SKImage skImage)
            {
                bytes = ImageToByteArray(skImage);
                result = true;
            }
            else if(img != null && img is SKBitmap skBitmap)
            {
                bytes = ImageToByteArray(skBitmap);
                result = true;
            }
            else if(img != null && img is byte[] bytesArray)
            {
                result = ImageValidator.IsLikelyImage(bytesArray);
                bytes = result ? bytesArray : null;
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

    public byte[] ImageToByteArray(SKImage imageIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
    {
        using var ms = new MemoryStream();
        imageIn.Encode(format, quality).SaveTo(ms);
        return ms.ToArray();
    }

    public byte[] ImageToByteArray(SKBitmap bitmapIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
    {
        using var ms = new MemoryStream();
        using var image = SKImage.FromBitmap(bitmapIn);
        image.Encode(format, quality).SaveTo(ms);
        return ms.ToArray();
    }

    public SKImage ByteArrayToImage(byte[] byteArrayIn)
    {
        using var ms = new MemoryStream(byteArrayIn);
        return SKImage.FromEncodedData(ms);
    }

    public SKImage StreamToImage(Stream stream)
    {
        return SKImage.FromEncodedData(stream);
    }
}