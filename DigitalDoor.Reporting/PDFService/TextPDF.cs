using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ViewModels;
using DigitalDoor.Reporting.PDFService;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Report = DigitalDoor.Reporting.Entities.ValueObjects;

namespace DigitalDoor.Reporting.PDF;

internal class TextPDF
{
    readonly ReportViewModel ReportViewModel;
    readonly TextMapperParagraph MapperParagraph;
    readonly TextMapperImage MapperImage;
    readonly TextMapperBorder MapperBorder;
    readonly TextMapperBase MapperBase;
    readonly TextHelper Helper;
    readonly decimal HeightHeader;
    readonly decimal HeightFooter;
    readonly decimal HeightBody;

    public TextPDF(ReportViewModel reportViewModel)
    {
        ReportViewModel = reportViewModel;
        MapperParagraph = new TextMapperParagraph();
        MapperImage = new TextMapperImage();
        MapperBorder = new TextMapperBorder();
        MapperBase = new TextMapperBase();
        Helper = new TextHelper();
        HeightHeader = (decimal)(ReportViewModel.Header.Format.Dimension.Height + ReportViewModel.Body.Format.Dimension.Height + ReportViewModel.Footer.Format.Dimension.Height);
        HeightBody = (decimal)(ReportViewModel.Body.Format.Dimension.Height + ReportViewModel.Footer.Format.Dimension.Height);
        HeightFooter = (decimal)ReportViewModel.Footer.Format.Dimension.Height;
    }

    public async Task<byte[]> CreatePDFReport()
     {
        using MemoryStream OutputStream = new MemoryStream();
        PdfWriter PdfWriter = new PdfWriter(OutputStream);
        PdfDocument PdfDocument = new PdfDocument(PdfWriter);
        PageSize Size = new PageSize(Helper.MillimeterToPixel(ReportViewModel.Page.Dimension.Width),
            Helper.MillimeterToPixel(ReportViewModel.Page.Dimension.Height));
        if(ReportViewModel.Page.Orientation == Report.Orientation.Landscape)
        {
            Size = Size.Rotate();
        }
        PdfDocument.SetDefaultPageSize(Size);
        await CreateDocument(PdfDocument);
        return OutputStream.ToArray();
    }

    private async Task CreateDocument(PdfDocument pdfDocument)
    {
        Document Document = new Document(pdfDocument);
        Document.SetMargins(Helper.MillimeterToPixel(ReportViewModel.Page.Margin.Top), Helper.MillimeterToPixel(ReportViewModel.Page.Margin.Right),
            Helper.MillimeterToPixel(ReportViewModel.Page.Margin.Bottom), Helper.MillimeterToPixel(ReportViewModel.Page.Margin.Left));
        await SetContentDocument(Document);
        Document.Close();
    }

    private async Task SetContentDocument(Document page)
    {
        List<ColumnContent> HeaderElements = Helper.GetElements(ReportViewModel.Header.Items, ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Header).ToList());
        List<ColumnContent> FooterElements = Helper.GetElements(ReportViewModel.Footer.Items, ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Footer).ToList());
        List<ColumnContent> BodyRows = Helper.GetElements(ReportViewModel.Body.Items, ReportViewModel.Data.Where(d => d.Section == Report.SectionType.Body).ToList());
        int RowsByPages = (int)(ReportViewModel.Body.Format.Dimension.Height / ReportViewModel.Body.Row.Dimension.Height);
        decimal HeightBodyElement = HeightBody;
        int ColumnsNumber = ReportViewModel.Body.ColumnsNumber;
        List<List<ColumnContent>> Pages = Helper.Split(BodyRows, RowsByPages);
        int PageNumber = 1;
        decimal ColumnWeight = 0;
        for(int i = 0; i < Pages.Count; i++)
        {
            int CounterColumn = 0;
            if(i > 0)
            {
                PageNumber += 1;
                ColumnWeight = 0;
                HeightBodyElement = HeightBody;
            }
            //if (Element.Column.DataColumn?.PropertyName == "TotalPages")
            //{
            //    Element.Value = PageElements.Count.ToString();
            //}
            //if (Element.Column.DataColumn.PropertyName == "CurrentPage")
            //{
            //    Element.Value = numberPage.ToString();
            //}
            //var TotalPages =  FooterElements[0]?.Columns.Where(d => d.Column.DataColumn.PropertyName == "TotalPages").FirstOrDefault();
            //TotalPages.Value = Pages.Count.ToString();
            MapperBase.DrawBackground(page, ReportViewModel.Header.Format.Background, PageNumber, ReportViewModel.Header.Format.Dimension.Height, HeightBody);
            MapperBase.DrawBackground(page, ReportViewModel.Body.Format.Background, PageNumber, ReportViewModel.Body.Format.Dimension.Height, HeightFooter);
            MapperBase.DrawBackground(page, ReportViewModel.Footer.Format.Background, PageNumber, ReportViewModel.Footer.Format.Dimension.Height, 0);
            HeaderElements?.ForEach(async Element => await DrawContent(page, Element, HeightHeader, PageNumber, 0, HeightBody));
            FooterElements?.ForEach(async Element => await DrawContent(page, Element, HeightFooter, PageNumber, 0, 0));
            await CreateBodyElements(page, Pages[i], HeightBodyElement, PageNumber, ColumnWeight, HeightFooter);
            while(ColumnsNumber > CounterColumn + 1 && i + 1 < Pages.Count)
            {
                CounterColumn += 1;
                i += 1;
                HeightBodyElement = HeightBody;
                ColumnWeight += (decimal)(ReportViewModel.Body.Row.Dimension.Width + ReportViewModel.Body.ColumnsSpace);
                await CreateBodyElements(page, Pages[i], HeightBodyElement, PageNumber, ColumnWeight, HeightFooter);
            }
        }
    }

    private async Task CreateBodyElements(Document page, List<ColumnContent> pagesElements, decimal heightBodyElement, int numberPage, decimal columnWeight, decimal heightBackground)
    {
        List<ColumnContent> PageElements = pagesElements;
        for(int r = 0; r < PageElements.Count; r++)
        {
            ColumnContent Element = PageElements[r];
            if(ReportViewModel.Body.Row.Borders != null)
            {
                ColumnSetup setup = new ColumnSetup();
                setup.Format.Borders = ReportViewModel.Body.Row.Borders;
                setup.Format.Dimension.Width = ReportViewModel.Body.Row.Dimension.Width;
                setup.Format.Dimension.Height = ReportViewModel.Body.Row.Dimension.Height;
                setup.Format.Position = ReportViewModel.Body.Format.Position;
                Div BorderBody = MapperBorder.SetBorder(setup, (heightBodyElement - ReportViewModel.Body.Format.Position.Top), columnWeight);
                BorderBody.SetPageNumber(numberPage);
                page.Add(BorderBody);
            }
            await DrawContent(page, Element, heightBodyElement, numberPage, columnWeight + (decimal)ReportViewModel.Body.Format.Position.Left, heightBackground);
            heightBodyElement -= (decimal)ReportViewModel.Body.Row.Dimension.Height;
        }
    }

    private Task DrawContent(Document page, ColumnContent format, decimal height, int PositionPage, decimal weight, decimal heightBackground)
    {
        format.Columns = format.Columns.OrderBy(d => d.Column.Format.Foreground).ToList();
        foreach(var item in format.Columns)
        {
            string Content = item.Value;
            if(Content != null)
            {
                if(Content == " ")
                {
                    Div BorderSpaceWhite = MapperBorder.SetBorder(item.Column, height, weight);
                    BorderSpaceWhite.SetPageNumber(PositionPage);
                    page.Add(BorderSpaceWhite);
                }
                else if(Content == "")
                {
                    MapperBase.DrawBackground(page, item.Column.Format.Background, PositionPage, item.Column.Format.Dimension.Height, heightBackground);
                }
                else
                {
                    Paragraph Text = MapperParagraph.SetParagraph(Content, item, height, weight);
                    Text.SetPageNumber(PositionPage);
                    page.Add(Text);
                }
            }
            if(item.Image != null && item.Image.Length > 0)
            {
                Image Image = MapperImage.SetImage(item.Image, item, height, weight);
                Image.SetPageNumber(PositionPage);
                page.Add(Image);
            }
        }
        return Task.CompletedTask;
    }
}
