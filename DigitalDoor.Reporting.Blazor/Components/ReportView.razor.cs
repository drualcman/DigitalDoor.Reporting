using DigitalDoor.Reporting.Blazor.Models;
using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.Entities.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace DigitalDoor.Reporting.Blazor.Components;

public partial class ReportView : IDisposable
{
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Parameter][EditorRequired] public ReportViewModel ReportModel { get; set; }
    [Parameter] public bool ShowPreview { get; set; } = true;
    [Parameter] public string WrapperId { get; set; } = $"doc{Guid.NewGuid().ToString().Replace("-", "")}";
    [Parameter] public EventCallback<PdfResponse> OnGetHtml { get; set; }

    RenderFragment Content;

    double SectionColumns;
    Dimension SectionDimension;
    Dimension RowDimension;
    Border RowBorders;
    int totalpages;
    int currentPage;


    IJSObjectReference JSModule;

    protected override void OnParametersSet()
    {
        RenderReport();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            if(ReportModel is not null)
            {
                JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/DigitalDoor.Reporting.Blazor/Printing-Report.js?v={DateTime.Now.Ticks}");
                await JSModule.InvokeVoidAsync("PrintReports.AddCssToPage", ReportModel.Page.Dimension.Width, ReportModel.Page.Dimension.Height);
            }
        }
    }

    #region render page
    void RenderReport()
    {
        totalpages = ReportModel.Pages;

        Content = builder =>
        {
            //create a page
            StartPage(builder, ReportModel);
            currentPage = ReportModel.CurrentPage;
            var grouped = ReportModel.Data.Where(d => d.Section == SectionType.Header)
                .GroupBy(r => r.Row);

            CreateHeader(builder, ReportModel, grouped);

            //body
            grouped = ReportModel.Data.Where(d => d.Section == SectionType.Body)
                .GroupBy(r => r.Row);

            StartBody(builder, ReportModel);

            CreateRow(builder, grouped, ReportModel, ReportModel.Body.Items);

            EndSection(builder);

            //footer
            grouped = ReportModel.Data.Where(d => d.Section == SectionType.Footer)
                .GroupBy(r => r.Row);

            CreateFooter(builder, ReportModel, grouped);

            EndSection(builder);
        };
    }

    void StartPage(RenderTreeBuilder builder, ReportViewModel data)
    {
        string pageSetUp = "main--container";
        builder.OpenElement(0, "div");

        Format format = new Format(data.Page);
        if(ReportModel.Page.Orientation == Orientation.Landscape)
        {
            format.Dimension = new Dimension(ReportModel.Page.Dimension.Height, ReportModel.Page.Dimension.Width);
        }

        string styleContainer = $"{GetStyle(format)}";

        builder.AddAttribute(0, "style", styleContainer);
        builder.AddAttribute(0, "class", pageSetUp);
    }

    void CreateHeader(RenderTreeBuilder builder, ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        builder.OpenElement(1, "div");
        string headerWp = "headerWp";
        string styleHeader = $"{GetStyle(data.Header.Format)}" +
            $"position: relative;";
        //$"border-bottom: {data.Header.PageSetup.Padding.Bottom}mm solid #000;" +

        builder.AddAttribute(1, "style", styleHeader);
        builder.AddAttribute(1, "class", headerWp);

        SectionColumns = data.Header.ColumnsNumber;
        SectionDimension = data.Header.Format.Dimension;
        RowDimension = data.Header.Row.Dimension;
        RowBorders = data.Header.Row.Borders;

        CreateRow(builder, grouped, data, data.Header.Items);

        builder.CloseElement();
    }

    void StartBody(RenderTreeBuilder builder, ReportViewModel data)
    {
        string bodyWp = "body-wp";
        string styleBody = "";
        SectionColumns = data.Body.ColumnsNumber;

        if(SectionColumns > 1)
        {
            StringBuilder frString = new StringBuilder();
            for(int i = 0; i < SectionColumns; i++)
            {
                frString.Append("1fr ");
            }
            styleBody = $"{GetStyle(data.Body.Format)}" +
                $"display:grid; grid-template-columns:{frString}; position:relative; grid-auto-rows: {data.Body.Row.Dimension.Height}mm ";
        }
        else
        {
            styleBody = $"{GetStyle(data.Body.Format)}" + $"position:relative;";
        }
        builder.OpenElement(2, "div");
        builder.AddAttribute(2, "style", styleBody);
        builder.AddAttribute(2, "class", bodyWp);

        SectionDimension = data.Body.Format.Dimension;
        RowDimension = data.Body.Row.Dimension;
        RowBorders = data.Body.Row.Borders;
    }

    void CreateFooter(RenderTreeBuilder builder, ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        string footerWp = "footerWp";
        string styleFooter = $"{GetStyle(data.Footer.Format)}" +
          $"position:relative;";

        builder.OpenElement(5, "div");
        builder.AddAttribute(5, "style", styleFooter);
        builder.AddAttribute(5, "class", footerWp);

        SectionColumns = data.Footer.ColumnsNumber;
        SectionDimension = data.Footer.Format.Dimension;
        RowDimension = data.Footer.Row.Dimension;
        RowBorders = data.Footer.Row.Borders;

        CreateRow(builder, grouped, data, data.Footer.Items);

        builder.CloseElement();
    }

    void EndSection(RenderTreeBuilder builder) =>
        builder.CloseElement();

    Format GetColumnFormat(List<ColumnSetup> columns, Item item)
    {
        Format format;
        try
        {
            ColumnSetup column = columns.First(c => c.DataColumn.Equals(item));
            format = column.Format; 
        }
        catch
        {
            format = null;
        }
        return format;
    }

    bool HasColumn(List<ColumnSetup> columns, Item item)
    {
        bool result;
        try
        {
            ColumnSetup column = columns.First(c => c.DataColumn.Equals(item));
            result = column is not null;
        }
        catch
        {
            result = false;
        }
        return result;
    }

    void CreateRow(RenderTreeBuilder builder,
        IEnumerable<IGrouping<int, ColumnData>> grouped,
        ReportViewModel data,
        List<ColumnSetup> columns)
    {
        int heightRow = 1;
        int totalRows = grouped.Count();
        int rowNo = 1;
        int myColumn = 1;
        foreach(var group in grouped)
        {
            ColumnData row = group.FirstOrDefault();
            builder.OpenElement(30, "div");

            ColumnSetup item = columns.FirstOrDefault(c => c.DataColumn.Equals(row.Column));

            string styleRow = $"position:relative;overflow: hidden;" +
             $"height:{RowDimension.Height}mm; width:{RowDimension.Width}mm;" +
             $"border-style: {RowBorders.Style};" +
             $"border-top-width: {RowBorders.Top.Width}mm;" +
             $"border-top-color: {RowBorders.Top.Colour};" +
             $"border-left-width: {RowBorders.Left.Width}mm;" +
             $"border-left-color: {RowBorders.Left.Colour};" +
             $"border-right-width: {RowBorders.Right.Width}mm;" +
             $"border-right-color: {RowBorders.Right.Colour};" +
             $"border-bottom-color: {RowBorders.Bottom.Colour};" +
             $"border-bottom-width: {RowBorders.Bottom.Width}mm;";

            builder.AddAttribute(30, "style", styleRow);

            if(item != null)
            {
                CreateColumns(builder, group, columns);
                if(SectionColumns > 1)
                {
                    if(myColumn >= SectionColumns)
                    {
                        heightRow++;
                        myColumn = 1;
                    }
                    else
                    {

                        myColumn++;
                    }
                }
                else heightRow++;
            }
            else
            {
                builder.AddAttribute(3, "style", "display:none");
            }

            builder.CloseElement();

            if(heightRow > (SectionDimension.Height / RowDimension.Height) && item.Format.Section == SectionType.Body && rowNo < totalRows)
            {
                rowNo++;
                EndSection(builder);
                //footer
                var newGroupedFooter = data.Data.Where(d => d.Section == SectionType.Footer)//.OrderBy(d => new { d.Position.Top, d.Position.Left, d.Foreground })
                    .GroupBy(r => r.Row);

                CreateFooter(builder, data, newGroupedFooter);

                EndSection(builder);

                currentPage++;

                StartPage(builder, data);
                var newGroupedHeader = data.Data.Where(d => d.Section == SectionType.Header)//.OrderBy(d => new { d.Position.Top, d.Position.Left, d.Foreground })
                     .GroupBy(r => r.Row);
                CreateHeader(builder, data, newGroupedHeader);

                StartBody(builder, data);
                heightRow = 1;
            }
        }
    }

    void CreateColumns(RenderTreeBuilder builder, IGrouping<int, ColumnData> group,
        List<ColumnSetup> columns)
    {
        foreach(ColumnData item in group)
        {
            if(HasColumn(columns, item.Column))
            {
                string styleCol;
                if(item.Format is not null)
                {
                    styleCol = GetStyle(item.Format);
                }
                else
                {
                    styleCol = GetStyle(GetColumnFormat(columns, item.Column));
                }

                styleCol += "position: absolute;";
                string base64 = GetBase64(item);
                if(!string.IsNullOrEmpty(base64))
                {
                    string result = $"data:image/png;base64,{base64}";
                    builder.OpenElement(4, "div");
                    builder.AddAttribute(4, "style", styleCol);
                    builder.OpenElement(5, "img");
                    string style = "display: block; width: inherit;";
                    builder.AddAttribute(5, "style", style);
                    builder.AddAttribute(5, "src", result);
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(4, "div");
                    builder.AddAttribute(4, "style", styleCol);
                    if(item.Column.PropertyName == "TotalPages") builder.AddContent(4, totalpages);
                    else if(item.Column.PropertyName == "CurrentPage") builder.AddContent(4, currentPage);
                    else builder.AddContent(4, item.Value);
                }
                builder.CloseElement();
            }
        }
    }

    string GetBase64(ColumnData item)
    {
        string base64 = string.Empty;
        if(item.Value is not null)
        {
            try
            {
                byte[] bytes = new byte[] { };
                if(item.Value.GetType() == typeof(byte[]))
                {
                    bytes = (byte[])item.Value;
                    base64 = SetBase64Image(bytes);
                }
                else if(item.Value.GetType() == typeof(JsonElement))
                {
                    JsonElement data = (JsonElement)item.Value;
                    if(data.TryGetBytesFromBase64(out bytes))
                    {
                        base64 = SetBase64Image(bytes);
                    }
                }
            }
            catch
            {
                base64 = string.Empty;
            }
        }
        return base64;
    }

    string SetBase64Image(byte[] bytes)
    {
        if(bytes.Length > 10) return Convert.ToBase64String(bytes);
        else return string.Empty;
    }

    int ActiveZindex = 10;

    string GetStyle(Format format)
    {       
        string styleContainer = string.Empty;
        if(format is not null)
        {
            styleContainer =
                $"width:{format.Dimension.Width}mm;" +
                $"height:{format.Dimension.Height}mm;" +
                $"background-color: {format.Background};" +
                $"padding-top: {format.Padding.Top}mm; " +
                $"padding-right: {format.Padding.Right}mm; " +
                $"padding-left: {format.Padding.Left}mm; " +
                $"padding-bottom: {format.Padding.Bottom}mm;" +
                $"top: {format.Position.Top}mm;" +
                $"right: {format.Position.Right}mm;" +
                $"bottom: {format.Position.Bottom}mm;" +
                $"left: {format.Position.Left}mm;" +
                $"margin-top: {format.Margin.Top}mm;" +
                $"margin-right: {format.Margin.Right}mm;" +
                $"margin-bottom: {format.Margin.Bottom}mm;" +
                $"margin-left: {format.Margin.Left}mm;" +
                $"border-style: {format.Borders.Style};" +
                $"border-top-width: {format.Borders.Top.Width}mm;" +
                $"border-top-color: {format.Borders.Top.Colour};" +
                $"border-left-width: {format.Borders.Left.Width}mm;" +
                $"border-left-color: {format.Borders.Left.Colour};" +
                $"border-right-width: {format.Borders.Right.Width}mm;" +
                $"border-right-color: {format.Borders.Right.Colour};" +
                $"border-bottom-color: {format.Borders.Bottom.Colour};" +
                $"border-bottom-width: {format.Borders.Bottom.Width}mm;" +
                $"transform:rotate({format.Angle}deg);" +
                $"color: {format.FontDetails.ColorSize.Colour};" +
                $"font-family: {format.FontDetails.FontName}, Helvetica, sans-serif;" +
                $"font-weight: {format.FontDetails.FontStyle.Bold};" +
                $"font-size: {format.FontDetails.ColorSize.Width}pt;" +
                $"text-align: {format.TextAlignment};" +
                $"text-decoration: {format.TextDecoration};" +
                $"text-transform: none;" +
                $"letter-spacing: 0;" +
                $"z-index: {ActiveZindex};" +
                $"overflow: hidden;visibility: visible; display: block;box-sizing: unset;";
            styleContainer += format.FontDetails.FontStyle.Italic.Equals(true) ? $"font-style: italic;" : "";
        }
        ActiveZindex++;
        return styleContainer;
    }
    #endregion

    async public Task<PdfResponse> GetHtml()
    {
        PdfResponse response;
        try
        {
            response = await JSModule.InvokeAsync<PdfResponse>("PrintReports.GetHtml", WrapperId);
        }
        catch(Exception ex)
        {
            response = new PdfResponse
            {
                Base64String = string.Empty,
                Message = ex.Message,
                Html = string.Empty,
                Result = false
            };
        }
        if(OnGetHtml.HasDelegate)
            await OnGetHtml.InvokeAsync(response);
        return response;
    }

    public async void Dispose()
    {
        if(JSModule != null)
        {
            try
            {
                await JSModule.DisposeAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
