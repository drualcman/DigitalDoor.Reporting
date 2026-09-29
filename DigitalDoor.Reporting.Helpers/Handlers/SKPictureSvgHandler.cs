namespace DigitalDoor.Reporting.Helpers.Handlers;

internal static class SKPictureSvgHandler
{
    public static string PictureToSvg(SKPicture picture)
    {
        string svg = null;
        SKRect bounds = picture.CullRect;
        if (bounds.Width > 0 && bounds.Height > 0)
        {
            using MemoryStream svgStream = new MemoryStream();
            using (SKCanvas svgCanvas = SKSvgCanvas.Create(bounds, svgStream))
            {
                svgCanvas.DrawPicture(picture);
            }
            svg = Encoding.UTF8.GetString(svgStream.ToArray());
        }
        return svg;
    }
}
