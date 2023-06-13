using DigitalDoor.Reporting.PDF;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Text;
using Report = DigitalDoor.Reporting.Entities.ValueObjects;
namespace DigitalDoor.Reporting.PDFService
{
    internal class HelperPDF
    {
        public Color GetColor(string color)
        {
            return color switch
            {
                "green" => ColorConstants.GREEN,
                _ => ColorConstants.BLACK
            };
        }

        public Paragraph MapperSetParagraph(string textValue, ColumnFormat item)
        {
            Paragraph Text = new Paragraph();
            Text.Add(textValue);
            Text.SetFontSize((float)item.Column.Format.FontDetails.ColorSize.Width);
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
            try
            {
                Text.SetFont(PdfFontFactory.CreateFont(item.Column.Format.FontDetails.FontName));
                int Angle = item.Column.Format.Angle switch
                {
                    90 => 2,
                    -90 => -2,
                    _ => 0
                };
                if (Angle != 0)
                {
                    Text.SetRotationAngle(Math.PI/Angle);
                }
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
            return Text;
        }

        public Image MapperSetImage(string bytes, ColumnFormat item)
        {
            byte[] ImageBytes = Encoding.ASCII.GetBytes(bytes);
            Image Image = null;
            try
            {
                ImageData imageData = ImageDataFactory.Create(ImageBytes);
                Image = new Image(imageData);
                Image.SetPaddingTop(MillimeterToPixel(item.Column.Format.Padding.Top));
                Image.SetPaddingBottom(MillimeterToPixel(item.Column.Format.Padding.Bottom));
                Image.SetPaddingLeft(MillimeterToPixel(item.Column.Format.Padding.Left));
                Image.SetPaddingRight(MillimeterToPixel(item.Column.Format.Padding.Right));
                Image.SetMarginTop(MillimeterToPixel(item.Column.Format.Margin.Top));
                Image.SetMarginBottom(MillimeterToPixel(item.Column.Format.Margin.Bottom));
                Image.SetMarginLeft(MillimeterToPixel(item.Column.Format.Margin.Left));
                Image.SetMarginRight(MillimeterToPixel(item.Column.Format.Margin.Right));
                int Angle = item.Column.Format.Angle switch
                {
                    90 => 2,
                    -90 => -2,
                    _ => 0
                };
                if (Angle != 0)
                {
                    Image.SetRotationAngle(Math.PI/Angle);
                }
            }
            catch { }
            return Image;
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
