using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.PDF;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalDoor.Reporting.PDFService
{
    internal class TextMapperBorder : TextMapperBase
    {
        public Div SetBorder(ColumnSetup item, decimal height, decimal weight)
        {
            Div Div = new Div();
            Div.SetWidth(MillimeterToPixel(item.Format.Dimension.Width));
            Div.SetHeight(MillimeterToPixel(item.Format.Dimension.Height));
            Div.SetFixedPosition(MillimeterToPixel(item.Format.Position.Left+weight),
                          MillimeterToPixel(height-item.Format.Position.Top), MillimeterToPixel(item.Format.Dimension.Width));
            if(item.Format.Borders.Bottom.Width > 0 && 
                item.Format.Borders.Top.Width > 0 && 
                item.Format.Borders.Left.Width >0 &&
                item.Format.Borders.Right.Width > 0)
            {
                Div.SetFixedPosition(MillimeterToPixel(item.Format.Position.Left+weight),
                          MillimeterToPixel((height-(decimal)item.Format.Dimension.Height)-item.Format.Position.Top), 
                          MillimeterToPixel(item.Format.Dimension.Width));
            }
            BorderStyle Style = item.Format.Borders.Style;
            if (item.Format.Borders.Bottom.Width > 0)
            {
                Div.SetBorderBottom(GetBorder(Style, MillimeterToPixel(item.Format.Borders.Bottom.Width),item.Format.Borders.Bottom.Colour));
            }
            if (item.Format.Borders.Top.Width >  0)
            {
                Div.SetBorderTop(GetBorder(Style, MillimeterToPixel(item.Format.Borders.Top.Width),item.Format.Borders.Top.Colour));
            }
            if (item.Format.Borders.Right.Width > 0)
            {
                Div.SetBorderRight(GetBorder(Style, MillimeterToPixel(item.Format.Borders.Right.Width), item.Format.Borders.Right.Colour));
            }
            if (item.Format.Borders.Left.Width > 0)
            {
                Div.SetBorderLeft(GetBorder(Style, MillimeterToPixel(item.Format.Borders.Left.Width), item.Format.Borders.Left.Colour));
            }
            return Div;
        }
    }
}
