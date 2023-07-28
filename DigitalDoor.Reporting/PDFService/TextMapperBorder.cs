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
        public Div SetBorder(ColumnContent item, decimal height, decimal weight)
        {
            Div Div = new Div();
            Div.SetWidth(MillimeterToPixel(item.Column.Format.Dimension.Width));
            Div.SetHeight(MillimeterToPixel(item.Column.Format.Dimension.Height));
            Div.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left+weight),
                          MillimeterToPixel(height-item.Column.Format.Position.Top), MillimeterToPixel(item.Column.Format.Dimension.Width));
            if(item.Column.Format.Borders.Bottom.Width > 0 && 
                item.Column.Format.Borders.Top.Width > 0 && 
                item.Column.Format.Borders.Left.Width >0 &&
                item.Column.Format.Borders.Right.Width > 0)
            {
                Div.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left+weight),
                          MillimeterToPixel((height-(decimal)item.Column.Format.Dimension.Height)-item.Column.Format.Position.Top), 
                          MillimeterToPixel(item.Column.Format.Dimension.Width));
            }
            BorderStyle Style = item.Column.Format.Borders.Style;
            if (item.Column.Format.Borders.Bottom.Width > 0)
            {
                Div.SetBorderBottom(GetBorder(Style, item.Column.Format.Borders.Bottom.Width,item.Column.Format.Borders.Bottom.Colour));
            }
            if (item.Column.Format.Borders.Top.Width >  0)
            {
                Div.SetBorderTop(GetBorder(Style, item.Column.Format.Borders.Top.Width,item.Column.Format.Borders.Top.Colour));
            }
            if (item.Column.Format.Borders.Right.Width > 0)
            {
                Div.SetBorderRight(GetBorder(Style, item.Column.Format.Borders.Right.Width,item.Column.Format.Borders.Right.Colour));
            }
            if (item.Column.Format.Borders.Left.Width > 0)
            {
                Div.SetBorderLeft(GetBorder(Style, item.Column.Format.Borders.Left.Width,item.Column.Format.Borders.Left.Colour));
            }
            return Div;
        }
    }
}
