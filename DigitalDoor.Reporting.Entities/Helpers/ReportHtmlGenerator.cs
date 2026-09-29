using DigitalDoor.Reporting.Entities.Helpers.Html;
using DigitalDoor.Reporting.Entities.Layout;

namespace DigitalDoor.Reporting.Entities.Helpers;

public class ReportHtmlGenerator
{
    private readonly ReportViewModel ReportModel;

    public ReportHtmlGenerator(ReportViewModel reportModel)
    {
        ReportModel = reportModel;
    }

    public string GenerateHtml()
    {
        StringBuilder html = new StringBuilder();
        if (ReportModel is not null)
        {
            ReportLayoutContent content = ReportLayoutContentBuilder.Build(ReportModel);
            ReportPageLayout layout = ReportPageLayout.Create(ReportModel);
            List<ReportBodyPage> pages = ReportBodyPaginator.Paginate(ReportModel.Body, content.BodyRows);
            HtmlPageWriter pageWriter = new HtmlPageWriter(ReportModel, layout, content);
            for (int pageIndex = 0; pageIndex < pages.Count; pageIndex++)
            {
                pageWriter.WritePage(html, pages[pageIndex], pageIndex == pages.Count - 1);
            }
        }
        return html.ToString();
    }
}
