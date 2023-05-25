using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ViewModels;
using DigitalDoor.Reporting.PDFService;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Data;
using Report = DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.PDF
{
    internal class ITextPDF
    {
        ReportViewModel ReportViewModel { get; set; }
        readonly FormatTable FormatTable;
        readonly HelperPDF Helper;
        readonly MakerTable MakerTable;
        
        public ITextPDF(ReportViewModel reportViewModel)
        {
            ReportViewModel = reportViewModel;
            Helper = new HelperPDF();
            FormatTable = new FormatTable();
            MakerTable = new MakerTable();  
        }

        public async Task<byte[]> CreatePDFReport()
        {
            using MemoryStream OutputStream = new MemoryStream();
            Document Document = CreateDocument(ReportViewModel.Page, OutputStream);
            await SetContentDocument(Document);
            Document.Close();
            return OutputStream.ToArray();
        }

        private Document CreateDocument(Format format, MemoryStream OutputStream)
        {
            PdfWriter PdfWriter = new PdfWriter(OutputStream);
            PdfDocument PdfDocument = new PdfDocument(PdfWriter);
            PageSize Size = new PageSize(Helper.MillimeterToPixel(format.Dimension.Width),Helper.MillimeterToPixel(format.Dimension.Height));
            if (format.Orientation == Report.Orientation.Landscape)
            {
                Size = Size.Rotate();
            }
            PdfDocument.SetDefaultPageSize(Size);
            Document Document = new Document(PdfDocument);
            Document.SetMargins(Helper.MillimeterToPixel(format.Margin.Top), Helper.MillimeterToPixel(format.Margin.Right),
                Helper.MillimeterToPixel(format.Margin.Bottom), Helper.MillimeterToPixel(format.Margin.Left));
            return Document;
        }

        private async Task SetContentDocument(Document page)
        {
            Table Table = new Table(1);
            Table.SetMargins(0, 0, 0, 0);
            Table.SetPaddings(0, 0, 0, 0);
            Table.SetBorder(Border.NO_BORDER);
            List<Task> tasks = new List<Task>()
            {
               Task.Run( async () => { await CreateHeader(Table);}),
               Task.Run( async () => { await CreateBody(Table);}),
               Task.Run( async () => { await CreateFooter(Table);})
            };
            await Task.WhenAll(tasks);
            page.Add(Table);
        }

        private async Task CreateHeader(Table table)
        {
            List<ColumnData> Data = ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Header).ToList();
            table.AddHeaderCell(await CreateTableContent(ReportViewModel.Header, Data));
        }

        private async Task CreateBody(Table table)
        {
            Cell Cell = MakerTable.CreateCell();
            List<ColumnData> Data = ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Body).ToList();
            List<FormatTable> Format = await FormatTable.GetTableFormat(ReportViewModel.Body.Items, Data);
            int Rows = (int)(ReportViewModel.Body.Format.Dimension.Height / ReportViewModel.Body.Row.Dimension.Height);
            var Pages = Helper.Split(Format, Rows);
            List<Task> Tasks = new List<Task>();
            foreach (var Page in Pages)
            {
                Table Table = await MakerTable.SetCellContent(ReportViewModel.Body,Page);
                Cell.Add(Table);
                table.AddCell(Cell);
            }
        }

        private async Task CreateFooter(Table table)
        {
            List<ColumnData> Data = ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Footer).ToList();
            table.AddFooterCell(await CreateTableContent(ReportViewModel.Footer, Data));
        }

        private async Task<Cell> CreateTableContent(Section section, List<ColumnData> sectionData)
        {
            Cell Cell = MakerTable.CreateCell();
            List<FormatTable> Format = await FormatTable.GetTableFormat(section.Items, sectionData);
            Table Table = await MakerTable.SetCellContent(section,Format);
            Cell.Add(Table);
            return Cell;
        }
    }

}
