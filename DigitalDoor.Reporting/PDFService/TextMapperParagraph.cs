using DigitalDoor.Reporting.PDF;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Report = DigitalDoor.Reporting.Entities.ValueObjects;
namespace DigitalDoor.Reporting.PDFService
{
    internal class TextMapperParagraph : TextMapperBase
    {
        public Paragraph SetParagraph(string textValue, ColumnContent item, decimal height, decimal weight)
        {
            Paragraph Text = new Paragraph();
            Text.SetFontSize((float)(item.Column.Format.FontDetails.ColorSize.Width));
            Color Color = GetColor(item.Column.Format.FontDetails.ColorSize.Colour.ToLower());
            if (item.Column.Format.FontDetails.FontStyle.Bold > 400)
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
            if (item.Column.Format.Borders.Top.Width > 0)
            {
                Text.SetBorderTop(new SolidBorder(GetColor(item.Column.Format.Borders.Top.Colour), (float)item.Column.Format.Borders.Top.Width));
            }
            if (item.Column.Format.Borders.Bottom.Width > 0)
            {
                Text.SetBorderBottom(new SolidBorder(GetColor(item.Column.Format.Borders.Bottom.Colour), (float)item.Column.Format.Borders.Bottom.Width));
            }
            if (item.Column.Format.Borders.Left.Width > 0)
            {
                Text.SetBorderLeft(new SolidBorder(GetColor(item.Column.Format.Borders.Left.Colour), (float)item.Column.Format.Borders.Left.Width));
            }
            if (item.Column.Format.Borders.Right.Width > 0)
            {
                Text.SetBorderRight(new SolidBorder(GetColor(item.Column.Format.Borders.Right.Colour), (float)item.Column.Format.Borders.Right.Width));
            }
            try
            {
                Text.SetFont(PdfFontFactory.CreateFont(item.Column.Format.FontDetails.FontName));
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
            int Angle = item.Column.Format.Angle switch
            {
                -90 => 2,
                90 => -2,
                _ => 0
            };
            if (Angle != 0)
            {
                Text.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left+weight)+MillimeterToPixel(item.Column.Format.Dimension.Width/1.7),
                MillimeterToPixel(height-(item.Column.Format.Position.Top+8)),
                                      MillimeterToPixel(item.Column.Format.Dimension.Width));
                Text.SetRotationAngle(Math.PI/Angle);
            }
            else
            {
                Text.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left+weight),
                                      MillimeterToPixel(height-(item.Column.Format.Position.Top+(decimal)(item.Column.Format.FontDetails.ColorSize.Width*0.33))),
                                      MillimeterToPixel(item.Column.Format.Dimension.Width));
            }
            Text.Add(textValue);
            return Text;
        }
    }
}
