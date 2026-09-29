namespace DigitalDoor.Reporting.Entities.Layout;

public sealed class ReportPageLayout
{
    public double PageWidth { get; }
    public double PageHeight { get; }
    public ReportSectionLayout Header { get; }
    public ReportSectionLayout Body { get; }
    public ReportSectionLayout Footer { get; }

    private ReportPageLayout(double pageWidth, double pageHeight, ReportSectionLayout header, ReportSectionLayout body, ReportSectionLayout footer)
    {
        PageWidth = pageWidth;
        PageHeight = pageHeight;
        Header = header;
        Body = body;
        Footer = footer;
    }

    public static ReportPageLayout Create(ReportViewModel report)
    {
        Dimension pageDimension = report.Page.Dimension ?? new Dimension(PageSize.A4);
        bool isLandscape = report.Page.Orientation == Orientation.Landscape;
        Kernel pageMargin = report.Page.Margin ?? new Kernel();
        Kernel pagePadding = report.Page.Padding ?? new Kernel();
        double contentLeft = (double)(pageMargin.Left + pagePadding.Left);
        double contentTop = (double)(pageMargin.Top + pagePadding.Top);
        ReportSectionLayout header = ReportSectionLayout.Create(report.Header.Format, contentLeft, contentTop);
        ReportSectionLayout body = ReportSectionLayout.Create(report.Body.Format, contentLeft, header.NextSectionTop);
        ReportSectionLayout footer = ReportSectionLayout.Create(report.Footer.Format, contentLeft, body.NextSectionTop);
        return new ReportPageLayout(
            isLandscape ? pageDimension.Height : pageDimension.Width,
            isLandscape ? pageDimension.Width : pageDimension.Height,
            header, body, footer);
    }
}
