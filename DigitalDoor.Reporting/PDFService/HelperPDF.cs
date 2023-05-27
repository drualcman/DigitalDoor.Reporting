using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.PDF;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Org.BouncyCastle.Asn1.X509;
using Report = DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.PDFService
{
    internal class HelperPDF
    {
        public Paragraph MapperSetParagraph(string textValue, ColumnFormat item)
        {
            Paragraph Text = new Paragraph();
            Text.Add(textValue);
            Text.SetFontSize((float)item.Column.Format.FontDetails.ColorSize.Width);
            Color Color = item.Column.Format.FontDetails.ColorSize.Colour switch
            {
                "Green" => ColorConstants.GREEN,
                _ => ColorConstants.BLACK
            };
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
            Text.SetFont(PdfFontFactory.CreateFont(item.Column.Format.FontDetails.FontName));
            TextAlignment Aligment = item.Column.Format.TextAlignment switch
            {
                Report.TextAlignment.Right => TextAlignment.RIGHT,
                Report.TextAlignment.Center => TextAlignment.CENTER,
                Report.TextAlignment.Left => TextAlignment.LEFT,
                Report.TextAlignment.Justify => TextAlignment.JUSTIFIED,
            };
            Text.SetTextAlignment(Aligment);
            return Text;
        }

        public float MillimeterToPixel(double milimiter)
        {
            return (float)(milimiter*2.83);
        }

        public float MillimeterToPixel(decimal milimiter)
        {
            return (float)((double)milimiter*2.83);
        }

        public List<List<FormatTable>> Split(List<FormatTable> orginal, int sizeList)
        {
            List<List<FormatTable>> Result = new List<List<FormatTable>>();
            int Index = 0;
            while (Index < orginal.Count)
            {
                List<FormatTable> List = orginal.GetRange(Index, Math.Min(sizeList, orginal.Count - Index));
                Result.Add(List);
                Index += sizeList;
            }
            return Result;
        }

    }
}
