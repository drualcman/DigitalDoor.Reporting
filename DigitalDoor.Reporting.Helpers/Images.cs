using System.Drawing;
using System.Drawing.Imaging;

namespace DigitalDoor.Reporting.Helpers
{
    public class Images
    {
        public bool TryGetImageBytes(object img, out byte[] bytes)
        {
            bool result;
            try
            {
                if(img != null && img.GetType() == typeof(Image))
                {
                    bytes = ImageToByteArray((Image)img);
                    result = true;
                }
                else if(img != null && img.GetType() == typeof(Bitmap))
                {
                    bytes = ImageToByteArray((Bitmap)img);
                    result = true;
                }
                else
                {                       
                    bytes = default!;
                    result = false;
                }
            }
            catch
            {
                bytes = default!;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Convert image to bytes[]
        /// </summary>
        /// <param name="imageIn"></param>
        /// <param name="format">formato de la imagen</param>
        /// <returns></returns>
        public byte[] ImageToByteArray(Image imageIn, ImageFormat format)
        {
            using MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ms.ToArray();
        }

        /// <summary>
        /// Convert image to bytes[]
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public byte[] ImageToByteArray(Image imageIn)
        {
            return ImageToByteArray(imageIn, imageIn.RawFormat);
        }

        /// <summary>
        /// Convert image to bytes[]
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public byte[] ImageToByteArray(Bitmap imageIn)
        {
            using(var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

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
            return ms.ToArray();
        }

        /// <summary>
        /// Convert bytes[] to image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
