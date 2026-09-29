namespace DigitalDoor.Reporting.Entities.Helpers;

public static class ImageValidator
{
    private const int HeaderBytesToInspect = 12;

    private static readonly string[] ImageHeadersHex =
    {
        "FFD8FF",
        "89504E470D0A1A0A",
        "474946383761",
        "474946383961",
        "424D",
        "49492A00",
        "4D4D002A",
        "52494646",
        "57454250",
        "464C4946",
        "000000667479704D534654",
        "00000100",
        "00000200"
    };

    public static bool IsLikelyImage(object data)
    {
        bool result = false;
        if (data is byte[] bytes)
        {
            result = IsLikelyImage(bytes);
        }
        return result;
    }

    public static bool IsLikelyImage(byte[] bytes)
    {
        bool result = false;
        if (bytes is not null && bytes.Length > 0)
        {
            result = HasImageHeader(bytes) || IsLikelyImage(Encoding.UTF8.GetString(bytes));
        }
        return result;
    }

    public static bool IsLikelyImage(string base64String)
    {
        return TryDecodeBase64Image(base64String, out byte[] _);
    }

    public static bool HasImageHeader(byte[] bytes)
    {
        bool result = false;
        if (bytes is not null && bytes.Length > 0)
        {
            string headerHex = BitConverter.ToString(bytes, 0, Math.Min(HeaderBytesToInspect, bytes.Length)).Replace("-", "");
            int index = 0;
            while (index < ImageHeadersHex.Length && !result)
            {
                result = headerHex.StartsWith(ImageHeadersHex[index], StringComparison.Ordinal);
                index++;
            }
        }
        return result;
    }

    public static bool TryDecodeBase64Image(string base64String, out byte[] imageBytes)
    {
        imageBytes = null;
        bool result = false;
        if (IsBase64String(base64String) && IsLikelyBase64(base64String))
        {
            byte[] decodedBytes = Convert.FromBase64String(Regex.Replace(base64String, @"\s+", ""));
            result = HasImageHeader(decodedBytes);
            imageBytes = result ? decodedBytes : null;
        }
        return result;
    }

    private static bool IsBase64String(string input)
    {
        bool result = false;
        if (!string.IsNullOrWhiteSpace(input) && input.Length % 4 == 0)
        {
            try
            {
                Convert.FromBase64String(input);
                result = true;
            }
            catch (FormatException)
            {
                result = false;
            }
        }
        return result;
    }

    private static bool IsLikelyBase64(string input)
    {
        bool result = true;
        int index = 0;
        while (index < input.Length && result)
        {
            char character = input[index];
            result = char.IsLetterOrDigit(character) || character == '+' || character == '/' || character == '=';
            index++;
        }
        return result;
    }
}
