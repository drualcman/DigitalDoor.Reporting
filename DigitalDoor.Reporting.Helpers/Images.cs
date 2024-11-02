namespace DigitalDoor.Reporting.Helpers
{
    public class Images
    {
        public Images()
        {
            if (!(Environment.OSVersion.Platform is PlatformID.Win32NT or PlatformID.Win32Windows))
                AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
        }

        public bool TryGetImageBytes(object img, out byte[] bytes)
        {
            bool result;
            try
            {
                if (img != null && img is SKImage skImage)
                {
                    bytes = ImageToByteArray(skImage);
                    result = true;
                }
                else if (img != null && img is SKBitmap skBitmap)
                {
                    bytes = ImageToByteArray(skBitmap);
                    result = true;
                }
                else if (img != null && img is Image systemImage)
                {
                    bytes = ImageToByteArray(systemImage);
                    result = true;
                }
                else if (img != null && img is Bitmap systemBitmap)
                {
                    bytes = ImageToByteArray(systemBitmap);
                    result = true;
                }
                else if (img != null && img is byte[] bytesArray)
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

        /// <summary>
        /// Convert SKImage to bytes[]
        /// </summary>
        /// <param name="imageIn"></param>
        /// <param name="format">Formato de la imagen</param>
        /// <returns></returns>
        public byte[] ImageToByteArray(SKImage imageIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
        {
            using var ms = new MemoryStream();
            imageIn.Encode(format, quality).SaveTo(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Convert SKBitmap to bytes[]
        /// </summary>
        /// <param name="bitmapIn"></param>
        /// <param name="format">Formato de la imagen</param>
        /// <returns></returns>
        public byte[] ImageToByteArray(SKBitmap bitmapIn, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
        {
            using var ms = new MemoryStream();
            using var image = SKImage.FromBitmap(bitmapIn);
            image.Encode(format, quality).SaveTo(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Convert bytes[] to SKImage
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public SKImage ByteArrayToImage(byte[] byteArrayIn)
        {
            using var ms = new MemoryStream(byteArrayIn);
            return SKImage.FromEncodedData(ms);
        }


        /// <summary>
        /// Convert image to bytes[]
        /// </summary>
        /// <param name="imageIn"></param>
        /// <param name="format">formato de la imagen</param>
        /// <returns></returns>
        public byte[] ImageToByteArray(Image imageIn, ImageFormat format)
        {
            using var ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ImageToByteArray(SKImage.FromBitmap(SKBitmap.Decode(ms.ToArray())));
        }

        /// <summary>
        /// Convert image to bytes[]
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public byte[] ImageToByteArray(Image imageIn) => ImageToByteArray(imageIn, imageIn.RawFormat);

        /// <summary>
        /// Convert image to bytes[]
        /// </summary>
        /// <param name="imageIn"></param>
        /// <param name="format">formato de la imagen</param>
        /// <returns></returns>
        public byte[] ImageToByteArray(Bitmap imageIn, ImageFormat format)
        {
            using MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ImageToByteArray(SKImage.FromBitmap(SKBitmap.Decode(ms.ToArray())));
        }

        /// <summary>
        /// Convert image to bytes[]
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public byte[] ImageToByteArray(Bitmap imageIn) => ImageToByteArray(imageIn, ImageFormat.Png);

    }
}