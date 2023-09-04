using DigitalDoor.Reporting.PDF;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Element;
using iText.Layout.Properties;
using Report = DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.PDFService;

internal class TextMapperParagraph : TextMapperBase
{
    public Paragraph SetParagraph(string textValue, ColumnContent item, decimal height, decimal weight)
    {
        Paragraph Text = new Paragraph();
        Text.SetFontSize((float)(item.Column.Format.FontDetails.ColorSize.Width));
        Color Color = GetColor(item.Column.Format.FontDetails.ColorSize.Colour.ToLower());
        if (item.Column.Format.FontDetails.FontStyle.Bold > 599)
        {
            Text.SetBold();
        }
        if (item.Column.Format.FontDetails.FontStyle.Italic)
        {
            Text.SetItalic();
        }
        Text.SetPaddingTop(MillimeterToPixel(item.Column.Format.Padding.Top));
        Text.SetPaddingBottom(MillimeterToPixel(item.Column.Format.Padding.Bottom));
        Text.SetPaddingLeft(MillimeterToPixel(item.Column.Format.Padding.Left));
        Text.SetPaddingRight(MillimeterToPixel(item.Column.Format.Padding.Right));
        Text.SetMarginTop(MillimeterToPixel(item.Column.Format.Margin.Top));
        Text.SetMarginBottom(MillimeterToPixel(item.Column.Format.Margin.Bottom));
        Text.SetMarginLeft(MillimeterToPixel(item.Column.Format.Margin.Left));
        Text.SetMarginRight(MillimeterToPixel(item.Column.Format.Margin.Right));
        Text.SetFontColor(Color);
        Report.BorderStyle Style = item.Column.Format.Borders.Style;
        if (item.Column.Format.Borders.Top.Width > 0)
        {
            Text.SetBorderTop(GetBorder(Style, MillimeterToPixel(item.Column.Format.Borders.Top.Width), item.Column.Format.Borders.Top.Colour));
        }
        if (item.Column.Format.Borders.Bottom.Width > 0)
        {
            Text.SetBorderBottom(GetBorder(Style, MillimeterToPixel(item.Column.Format.Borders.Bottom.Width), item.Column.Format.Borders.Bottom.Colour));
        }
        if (item.Column.Format.Borders.Left.Width > 0)
        {
            Text.SetBorderLeft(GetBorder(Style, MillimeterToPixel(item.Column.Format.Borders.Left.Width), item.Column.Format.Borders.Left.Colour));
        }
        if (item.Column.Format.Borders.Right.Width > 0)
        {
            Text.SetBorderRight(GetBorder(Style, MillimeterToPixel(item.Column.Format.Borders.Right.Width), item.Column.Format.Borders.Right.Colour));
        }
        try
        {
            string FontName = item.Column.Format.FontDetails.FontName;
            if (!PdfFontFactory.IsRegistered(FontName))
            {
                string Path = $@"C:\\Windows\\Fonts\{FontName}.TTF";
                PdfFontFactory.Register(Path, FontName);
            }
            var Font = PdfFontFactory.CreateRegisteredFont(FontName, PdfEncodings.WINANSI,PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED);
            Text.SetFont(Font);
        }
        catch { }
        TextAlignment Aligment = item.Column.Format.TextAlignment switch
        {
            Report.TextAlignment.Right => TextAlignment.RIGHT,
            Report.TextAlignment.Center => TextAlignment.CENTER,
            Report.TextAlignment.Left => TextAlignment.LEFT,
            Report.TextAlignment.Justify => TextAlignment.JUSTIFIED,
        };
        Text.SetTextAlignment(Aligment);
        if (item.Column.Format.Angle != 0)
        {
            Text.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left+weight)+MillimeterToPixel(item.Column.Format.Dimension.Width/1.49),
            MillimeterToPixel(height-(item.Column.Format.Position.Top+10)),
                                  MillimeterToPixel(item.Column.Format.Dimension.Width));
            Text.SetRotationAngle(ConvertAngleToRadian(item.Column.Format.Angle));
        }
        else
        {
            Text.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left+weight),
                                  MillimeterToPixel(height-(item.Column.Format.Position.Top+(decimal)(item.Column.Format.FontDetails.ColorSize.Width*0.53))),
                                  MillimeterToPixel(item.Column.Format.Dimension.Width));
        }
        Text.Add(textValue);
        return Text;
    }
}
