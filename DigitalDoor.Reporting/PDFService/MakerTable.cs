using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.PDF;
using iText.Layout.Borders;
using iText.Layout.Element;
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
            foreach (var ItemFormat in rows)
            {
                var Item = CreateRowContent(ItemFormat);
                Cell.Add(Item);
                table.AddCell(Cell);
            }
        }

        public Cell CreateCell()
        {
            Cell Cell = new Cell();
            Cell.SetMargins(0, 0, 0, 0);
            Cell.SetPaddings(0, 0, 0, 0);
            Cell.SetBorder(Border.NO_BORDER);
            return Cell;
        }
        private Table CreateRowContent(FormatTable format)
        {
            float[] Columns = new float[format.Columns.Count];
            for (int i = 0; i < format.Columns.Count; i++)
            {
                Columns[i] =  Helper.MillimeterToPixel(format.Columns[i].Column.Format.Dimension.Width);
            }
            Table Item = new Table(Columns);
            Item.SetMargins(0, 0, 0, 0);
            Item.SetPaddings(0, 0, 0, 0);
            CreateColumnContent(Item, format);
            return Item;
        }

        private void CreateColumnContent(Table table, FormatTable format)
        {
            foreach (var item in format.Columns)
            {
                string Content = item.Value;
                Paragraph Text = new Paragraph();
                if (!string.IsNullOrEmpty(Content))
                {
                    Text = Helper.MapperSetParagraph(Content, item);
                }
                Cell Cell = CreateCell();
                Cell.Add(Text);
                table.AddCell(Cell);
            }
        }
    }
}
