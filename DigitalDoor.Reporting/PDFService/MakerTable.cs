using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.PDF;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;
using iText.Layout.Element;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalDoor.Reporting.PDFService
{
    internal class MakerTable
    {
        readonly HelperPDF Helper = new HelperPDF();

        public async Task<Table> SetCellContent(Section section, List<FormatTable> format)
        {
            Table Table = new Table(1);
            Table.SetMargins(0, 0, 0, 0);
            Table.SetPaddings(0, 0, 0, 0);
            Table.SetWidth(Helper.MillimeterToPixel(section.Format.Dimension.Width));
            Table.SetHeight(Helper.MillimeterToPixel(section.Format.Dimension.Height));
            Table.SetBorder(Border.NO_BORDER);
            CreateRows(Table, format);
            return Table;
        }

        private void CreateRows(Table table, List<FormatTable> rows)
        {
            Cell Cell = CreateCell();
            for (int i = 0; i < rows.Count; i++)
            {
                float Top = (float)rows[i].Position;
                if (Top != 0)
                {
                   Top =  GetPaddingTop(rows, i) - Top;
                }
                var Item = CreateRowContent(rows[i],Top);
                Item.SetPaddingTop(Helper.MillimeterToPixel((float)10));
                Cell.Add(Item);
                table.AddCell(Cell);
            }
        }

        private Table CreateRowContent(FormatTable format,float top)
        {
            float[] Columns = new float[format.Columns.Count];
            for (int i = 0; i < format.Columns.Count; i++)
            {
                Columns[i] =  Helper.MillimeterToPixel(format.Columns[i].Column.Format.Dimension.Width);
            }
            Table Item = new Table(Columns);
            Item.SetMargins(0, 0, 0, 0);
            Item.SetPaddings(0, 0, 0, 0);
            CreateColumnContent(Item, format,top);
            return Item;
        }

        private void CreateColumnContent(Table table, FormatTable format,float top)
        {
            foreach (var item in format.Columns)
            {
                string Content = item.Value;
                Paragraph Text = new Paragraph();
                if (!string.IsNullOrEmpty(Content))
                {
                    Text = Helper.MapperSetParagraph(Content, item);
                }
                var Borders = item.Column.Format.Borders;
                Cell Cell = CreateCell(Borders);
                Cell.SetMinHeight((float)item.Column.Format.Dimension.Height * 2.4443f);
                Cell.SetPaddingTop(Helper.MillimeterToPixel(top));
                Cell.Add(Text);
                table.AddCell(Cell);
            }
        }

        private float GetPaddingTop(List<FormatTable> rows, int index)
        {
            float Result = 0;
            for (int i = 0; i <= index; i++)
            {
                Result += (float)rows[i].Columns[0].Column.Format.Dimension.Height;
            }
            return Result;
        }


        public Cell CreateCell(Entities.ValueObjects.Border border )
        {
            Cell Cell = CreateCell();
            if (border.Top.Width > 0)
            {
                Cell.SetBorderTop(new SolidBorder(Helper.GetColor(border.Top.Colour), (float)border.Top.Width));
            }
            if (border.Bottom.Width > 0)
            {
                Cell.SetBorderBottom(new SolidBorder(Helper.GetColor(border.Bottom.Colour), (float)border.Bottom.Width));
            }
            if (border.Left.Width > 0)
            {
                Cell.SetBorderLeft(new SolidBorder(Helper.GetColor(border.Left.Colour), (float)border.Left.Width));
            }
            if (border.Right.Width > 0)
            {
                Cell.SetBorderRight(new SolidBorder(Helper.GetColor(border.Right.Colour), (float)border.Right.Width));
            }
            return Cell;
        }

        public Cell CreateCell()
        {
            Cell Cell = new Cell();
            Cell.SetMargins(0, 0, 0, 0);
            Cell.SetPaddings(0, 0, 0, 0);
            Cell.SetBorder(new SolidBorder(ColorConstants.WHITE, 0));
            return Cell;
        }
    }
}
