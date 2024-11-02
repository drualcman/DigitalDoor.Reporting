namespace DigitalDoor.Reporting.Helpers;

public static class CssColors
{
    public const string BLACK = "#000000";
    public const string TRANSPARENT = "#00000000";
    public const string WHITE = "#FFFFFF";
    public const string RED = "#FF0000";
    public const string GREEN = "#008000";
    public const string BLUE = "#0000FF";
    public const string YELLOW = "#FFFF00";

    private static readonly Dictionary<string, string> ColorNames = new Dictionary<string, string>
    {
        { "transparent", TRANSPARENT },
        { "black", BLACK },
        { "white", WHITE },
        { "red", RED },
        { "green", GREEN },
        { "blue",BLUE },
        { "yellow", YELLOW },
        { "cyan", "#00FFFF" },
        { "magenta", "#FF00FF" },
        { "gray", "#808080" },
        { "silver", "#C0C0C0" },
        { "maroon", "#800000" },
        { "olive", "#808000" },
        { "purple", "#800080" },
        { "teal", "#008080" },
        { "navy", "#000080" },
        { "darkred", "#8B0000" },
        { "darkgreen", "#006400" },
        { "darkblue", "#00008B" },
        { "darkcyan", "#008B8B" },
        { "darkmagenta", "#8B008B" },
        { "darkgray", "#A9A9A9" },
        { "lightgray", "#D3D3D3" },
        { "lightred", "#FFCCCB" },
        { "lightgreen", "#90EE90" },
        { "lightblue", "#ADD8E6" },
        { "lightyellow", "#FFFFE0" },
        { "lightcyan", "#E0FFFF" },
        { "lightmagenta", "#FF77FF" },
        { "lightpink", "#FFB6C1" },
        { "gold", "#FFD700" },
        { "coral", "#FF7F50" },
        { "orange", "#FFA500" },
        { "khaki", "#F0E68C" },
        { "plum", "#DDA0DD" },
        { "salmon", "#FA8072" },
        { "sandybrown", "#F4A460" },
        { "lightseagreen", "#20B2AA" },
        { "lightsalmon", "#FFA07A" },
        { "lavender", "#E6E6FA" },
        { "honeydew", "#F0FFF0" },
        { "mistyrose", "#FFE4E1" },
        { "seashell", "#FFF5EE" },
        { "beige", "#F5F5DC" },
        { "aliceblue", "#F0F8FF" },
        { "antiquewhite", "#FAEBD7" },
        { "aqua", "#00FFFF" },
        { "aquamarine", "#7FFFD4" },
        { "bisque", "#FFE4C4" },
        { "blanchedalmond", "#FFEBCD" },
        { "blueviolet", "#8A2BE2" },
        { "chartreuse", "#7FFF00" },
        { "chocolate", "#D2691E" },
        { "darkorange", "#FF8C00" },
        { "darksalmon", "#E9967A" },
        { "darkviolet", "#9400D3" },
        { "deeppink", "#FF1493" },
        { "deepskyblue", "#00BFFF" },
        { "dodgerblue", "#1E90FF" },
        { "forestgreen", "#228B22" },
        { "fuchsia", "#FF00FF" },
        { "gainsboro", "#DCDCDC" },
        { "ghostwhite", "#F8F8FF" },
        { "indigo", "#4B0082" },
        { "ivory", "#FFFFF0" },
        { "lightcoral", "#F08080" },
        { "lightgoldenrodyellow", "#FAFAD2" },
        { "lightskyblue", "#87CEFA" },
        { "lightslategray", "#778899" },
        { "lightsteelblue", "#B0C4DE" },
        { "lime", "#00FF00" },
        { "limegreen", "#32CD32" },
        { "linen", "#FAF0E6" },
        { "mediumaquamarine", "#66CDAA" },
        { "mediumblue", "#0000CD" },
        { "mediumorchid", "#BA55D3" },
        { "mediumpurple", "#9370DB" },
        { "mediumseagreen", "#3CB371" },
        { "mediumslateblue", "#7B68EE" },
        { "mediumspringgreen", "#00FA9A" },
        { "mediumturquoise", "#48D1CC" },
        { "mediumvioletred", "#C71585" },
        { "midnightblue", "#191970" },
        { "moccasin", "#FFE4B5" },
        { "navajowhite", "#FFDEAD" },
        { "oldlace", "#FDF5E6" },
        { "orchid", "#DA70D6" },
        { "palegoldenrod", "#EEE8AA" },
        { "palegreen", "#98FB98" },
        { "paleturquoise", "#AFEEEE" },
        { "palevioletred", "#DB7093" },
        { "papayawhip", "#FFEFD5" },
        { "peachpuff", "#FFDAB9" },
        { "peru", "#CD853F" },
        { "powderblue", "#B0E0E6" },
        { "rosybrown", "#BC8F8F" },
        { "royalblue", "#4169E1" },
        { "saddlebrown", "#8B4513" },
        { "seagreen", "#2E8B57" },
        { "slateblue", "#6A5ACD" },
        { "slategray", "#708090" },
        { "snow", "#FFFAFA" },
        { "springgreen", "#00FF7F" },
        { "steelblue", "#4682B4" },
        { "tan", "#D2B48C" },
        { "thistle", "#D8BFD8" },
        { "tomato", "#FF6347" },
        { "turquoise", "#40E0D0" },
        { "violet", "#EE82EE" },
        { "wheat", "#F5DEB3" },
        { "yellowgreen", "#9ACD32" }
    };

    public static string ColorToHex(string color)
    {
        string result = BLACK;
        if (color.StartsWith('#'))
        {
            result = color;
        }
        else
        {
            if (ColorNames.TryGetValue(color.ToLower(System.Globalization.CultureInfo.InvariantCulture), out string hexFromName))
            {
                result = hexFromName;
            }
        }
        return result;
    }
}
