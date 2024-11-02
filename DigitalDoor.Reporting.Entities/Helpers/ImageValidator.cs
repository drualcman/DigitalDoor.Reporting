namespace DigitalDoor.Reporting.Entities.Helpers;

public static class ImageValidator
{
    public static bool IsLikelyImage(object data)
    {
        bool result = false;
        if (data is not null && data is byte[] bytes)
        {
            result = IsLikelyImage(bytes);
        };
        return result;
    }

    public static bool IsLikelyImage(byte[] bytes)
    {
        string base64 = Encoding.UTF8.GetString(bytes);
        return IsLikelyImage(base64);
    }

    public static bool IsLikelyImage(string base64String)
    {
        if (!IsBase64String(base64String))
        {
            return false;
        }
        //Remove whitespace and carriage returns from string
        if (!IsLikelyBase64(base64String))
        {
            return false;
        }
        string cleanBase64String = Regex.Replace(base64String, @"\s+", "");
        // Decode the string in bytes
        byte[] decodedBytes;
        try
        {
            decodedBytes = Convert.FromBase64String(cleanBase64String);
        }
        catch (FormatException)
        {
            return false;
        }
        string decodedHeaderHex = BitConverter.ToString(decodedBytes, 0, Math.Min(8, decodedBytes.Length)).Replace("-", "");
        //Image format headers
        string[] imageHeaders = {
        "FFD8FF",          // JPG (JPEG)
        "89504E470D0A1A0A",// PNG
        "474946383761",    // GIF87a
        "474946383961",    // GIF89a
        "424D",            // BMP
        "49492A00",        // TIFF (Intel byte order)
        "4D4D002A",        // TIFF (Motorola byte order)
        "52494646",        // WebP (RIFF)
        "57454250",        // WebP (VP8X)
        "464C4946",        // WebP (VP8L)
        "000000667479704D534654", //HEIF (High Efficiency Image File Format)
        "49492A00",        //JXR (JPEG XR)
        "4D4D002A",        //JXR (JPEG XR)
        "00000100",        //ICO (Icon)      
        "00000200"         //ICO (Icon)
        };

        foreach (string header in imageHeaders)
        {
            if (decodedHeaderHex.StartsWith(header))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsBase64String(string input)
    {
        //Check if the length of the string is a multiple of 4
        try
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            if (input.Length % 4 != 0)
            {
                return false;
            }
            Convert.FromBase64String(input);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool IsLikelyBase64(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsLetterOrDigit(c) && c != '+' && c != '/' && c != '=')
            {
                return false;
            }
        }
        return true;
    }
}
