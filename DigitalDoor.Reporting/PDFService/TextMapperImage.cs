using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.PDF;
using iText.IO.Image;
using iText.Layout.Element;

namespace DigitalDoor.Reporting.PDFService;

internal class TextMapperImage : TextMapperBase
{
    public Image SetImage(byte[] bytes, ColumnContent item, decimal height, decimal weight)
    {
        Image Image = null;
        try
        {
            ImageData imageData = ImageDataFactory.Create(bytes);
            Image = new Image(imageData);
            Image.SetPaddingTop(MillimeterToPixel(item.Column.Format.Padding.Top));
            Image.SetPaddingBottom(MillimeterToPixel(item.Column.Format.Padding.Bottom));
            Image.SetPaddingLeft(MillimeterToPixel(item.Column.Format.Padding.Left));
            Image.SetPaddingRight(MillimeterToPixel(item.Column.Format.Padding.Right));
            Image.SetMarginTop(MillimeterToPixel(item.Column.Format.Margin.Top));
            Image.SetMarginBottom(MillimeterToPixel(item.Column.Format.Margin.Bottom));
            Image.SetMarginLeft(MillimeterToPixel(item.Column.Format.Margin.Left));
            Image.SetMarginRight(MillimeterToPixel(item.Column.Format.Margin.Right));
            Image.SetHeight(MillimeterToPixel(item.Column.Format.Dimension.Height));
            Image.SetWidth(MillimeterToPixel(item.Column.Format.Dimension.Width));
            Image.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left + weight),
                    MillimeterToPixel(height - item.Column.Format.Position.Top - (decimal)item.Column.Format.Dimension.Height));
            BorderStyle Style = item.Column.Format.Borders.Style;
            if(item.Column.Format.Borders.Top.Width > 0)
            {
                Image.SetBorderTop(GetBorder(Style, MillimeterToPixel(item.Column.Format.Borders.Top.Width), item.Column.Format.Borders.Top.Colour));
            }
            if(item.Column.Format.Borders.Bottom.Width > 0)
            {
                Image.SetBorderBottom(GetBorder(Style, MillimeterToPixel(item.Column.Format.Borders.Bottom.Width), item.Column.Format.Borders.Bottom.Colour));
            }
            if(item.Column.Format.Borders.Left.Width > 0)
            {
                Image.SetBorderLeft(GetBorder(Style, MillimeterToPixel(item.Column.Format.Borders.Left.Width), item.Column.Format.Borders.Left.Colour));
            }
            if(item.Column.Format.Borders.Right.Width > 0)
            {
                Image.SetBorderRight(GetBorder(Style, MillimeterToPixel(item.Column.Format.Borders.Right.Width), item.Column.Format.Borders.Right.Colour));
            }
            Image.SetRotationAngle(ConvertAngleToRadian(item.Column.Format.Angle));             
        }
        catch { }
        return Image;
    }
}
