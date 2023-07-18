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
                          MillimeterToPixel((height+1.5m)-item.Column.Format.Position.Top), MillimeterToPixel(item.Column.Format.Dimension.Width));
            if(item.Column.Format.Borders.Bottom.Width > 0 && 
                item.Column.Format.Borders.Top.Width > 0 && 
                item.Column.Format.Borders.Left.Width >0 &&
                item.Column.Format.Borders.Right.Width > 0)
            {
                Div.SetFixedPosition(MillimeterToPixel(item.Column.Format.Position.Left+weight),
                          MillimeterToPixel((height-(decimal)item.Column.Format.Dimension.Height+1.5m)-item.Column.Format.Position.Top), 
                          MillimeterToPixel(item.Column.Format.Dimension.Width));
            }
            if (item.Column.Format.Borders.Bottom.Width > 0)
            {
                Div.SetBorderBottom(GetBorder(item.Column.Format.Borders.Style, item.Column.Format.Borders.Bottom.Width));
            }
            if (item.Column.Format.Borders.Top.Width >  0)
            {
                Div.SetBorderTop(GetBorder(item.Column.Format.Borders.Style, item.Column.Format.Borders.Top.Width));
            }
            if (item.Column.Format.Borders.Right.Width > 0)
            {
                Div.SetBorderRight(GetBorder(item.Column.Format.Borders.Style, item.Column.Format.Borders.Right.Width));
            }
            if (item.Column.Format.Borders.Left.Width > 0)
            {
                Div.SetBorderLeft(GetBorder(item.Column.Format.Borders.Style, item.Column.Format.Borders.Left.Width));
            }
            return Div;
        }
    }
}
