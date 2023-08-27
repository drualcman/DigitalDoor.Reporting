using DigitalDoor.Reporting.Entities.Interfaces;

namespace DigitalDoor.Reporting.Blazor.Components;

public partial class ReportView : IAsyncDisposable
{
    [Inject] public IJSRuntime JSRuntime { get; set; }   
    [Inject] public IReportAsBytes ReportPdf { get; set; }
    [Parameter][EditorRequired] public ReportViewModel ReportModel { get; set; }
    [Parameter] public bool ShowPreview { get; set; } = true;
    [Parameter] public string WrapperId { get; set; } = $"doc{Guid.NewGuid().ToString().Replace("-", "")}";
    [Parameter] public EventCallback<string> OnGetHtml { get; set; }

    RenderFragment Content;

    double SectionColumns;
    Dimension SectionDimension;
    Dimension RowDimension;
    Border RowBorders;
    int Totalpages;
    int CurrentPage;
    int CurrentDivId;


    readonly Lazy<Task<IJSObjectReference>> ModuleTask;

    protected override void OnParametersSet()
    {
        RenderReport();
    }

    public ReportView()
    {
        base.OnInitialized();
        ModuleTask = new Lazy<Task<IJSObjectReference>>(() => GetJSObjectReference(JSRuntime));
    }

    private Task<IJSObjectReference> GetJSObjectReference(IJSRuntime jsRuntime) =>
        jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"./{ContentHelper.ContentPath}/ReportTools.js?v={DateTime.Today.ToFileTimeUtc()}").AsTask();


    #region render page
    void RenderReport()
    {
        Totalpages = ReportModel.Pages;

        Content = builder =>
        {
            //create a page
            StartPage(builder, ReportModel);
            CurrentPage = ReportModel.CurrentPage;
            var grouped = ReportModel.Data.Where(d => d.Section == SectionType.Header)
                .GroupBy(r => r.Row);

            CreateHeader(builder, ReportModel, grouped);

            //body
            grouped = ReportModel.Data.Where(d => d.Section == SectionType.Body)
                .GroupBy(r => r.Row);

            StartBody(builder, ReportModel);                 //start body

            StartColumnSection(builder);       //start first column

            CreateRow(builder, grouped, ReportModel, ReportModel.Body.Items);

            EndSection(builder);                            //end column
            EndSection(builder);                            //end body

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
        CurrentDivId = 0;
        builder.OpenElement(CurrentDivId, "div");

        Format format = new Format(data.Page);
        if(ReportModel.Page.Orientation == Orientation.Landscape)
        {
            format.Dimension = new Dimension(ReportModel.Page.Dimension.Height, ReportModel.Page.Dimension.Width);
        }

        string styleContainer = $"{GetStyle(format)}";

        builder.AddAttribute(CurrentDivId, "style", styleContainer);
        builder.AddAttribute(CurrentDivId, "class", pageSetUp);
    }

    void CreateHeader(RenderTreeBuilder builder, ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        CurrentDivId++;
        builder.OpenElement(CurrentDivId, "div");
        string headerWp = "headerWp";
        string styleHeader = $"{GetStyle(data.Header.Format)}position: relative;";
        //$"border-bottom: {data.Header.PageSetup.Padding.Bottom}mm solid #000;" +

        builder.AddAttribute(CurrentDivId, "style", styleHeader);
        builder.AddAttribute(CurrentDivId, "class", headerWp);

        SectionColumns = data.Header.ColumnsNumber;
        SectionDimension = data.Header.Format.Dimension;
        RowDimension = data.Header.Row.Dimension;
        if(data.Header.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Header.Row.Dimension.Height, data.Header.Row.Dimension.Width);
        }
        RowBorders = data.Header.Row.Borders;

        StartColumnSection(builder);       //start first column
        CreateRow(builder, grouped, data, data.Header.Items);

        builder.CloseElement();
        builder.CloseElement();
        CurrentDivId--;
    }

    void StartBody(RenderTreeBuilder builder, ReportViewModel data)
    {
        string bodyWp = "body-wp";
        string styleBody;
        SectionColumns = data.Body.ColumnsNumber;

        if(SectionColumns > 1)
        {
            StringBuilder frString = new StringBuilder();
            for(int i = 0; i < SectionColumns; i++)
            {
                frString.Append("1fr ");
            }
            styleBody = $"{GetStyle(data.Body.Format).Replace("display: block;", "")}" +
                $"display:grid; grid-template-columns:{frString}; position:relative; grid-auto-rows: {data.Body.Row.Dimension.Height}mm ";
        }
        else
        {
            styleBody = $"{GetStyle(data.Body.Format)}" + $"position:relative;";
        }
        CurrentDivId++;
        builder.OpenElement(CurrentDivId, "div");
        builder.AddAttribute(CurrentDivId, "style", styleBody);
        builder.AddAttribute(CurrentDivId, "class", bodyWp);

        SectionDimension = data.Body.Format.Dimension;
        RowDimension = data.Body.Row.Dimension;
        if(data.Body.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Body.Row.Dimension.Height, data.Body.Row.Dimension.Width);
        }
        RowBorders = data.Body.Row.Borders;
    }

    void StartColumnSection(RenderTreeBuilder builder)
    {
        string bodyWp = "body-wp";
        string styleColumn = $"position:relative;overflow: hidden;height:{SectionDimension.Height}mm; width:{RowDimension.Width}mm;";
        CurrentDivId++;
        builder.OpenElement(CurrentDivId, "div");
        builder.AddAttribute(CurrentDivId, "style", styleColumn);
        builder.AddAttribute(CurrentDivId, "class", bodyWp);
    }

    void CreateFooter(RenderTreeBuilder builder, ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        string footerWp = "footerWp";
        string styleFooter = $"{GetStyle(data.Footer.Format)}position:relative;";

        CurrentDivId++;
        builder.OpenElement(CurrentDivId, "div");
        builder.AddAttribute(CurrentDivId, "style", styleFooter);
        builder.AddAttribute(CurrentDivId, "class", footerWp);

        SectionColumns = data.Footer.ColumnsNumber;
        SectionDimension = data.Footer.Format.Dimension;
        RowDimension = data.Footer.Row.Dimension;
        if(data.Footer.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Footer.Row.Dimension.Height, data.Footer.Row.Dimension.Width);
        }
        RowBorders = data.Footer.Row.Borders;

        StartColumnSection(builder);       //start first column
        CreateRow(builder, grouped, data, data.Footer.Items);

        builder.CloseElement();
        builder.CloseElement();
        CurrentDivId--;
    }

    void EndSection(RenderTreeBuilder builder)
    {
        builder.CloseElement();
        CurrentDivId--;
    }

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
            Console.WriteLine("error");
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
        bool newPage = false;
        foreach(var group in grouped)
        {
            ColumnData row = group.FirstOrDefault();
            CurrentDivId++;
            builder.OpenElement(CurrentDivId, "div");

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

            builder.AddAttribute(CurrentDivId, "style", styleRow);

            if(item != null)
            {
                CreateColumns(builder, group, columns);
            }
            else
            {
                builder.AddAttribute(CurrentDivId, "style", "display:none");
            }

            heightRow++;
            EndSection(builder);            //end element
            newPage = heightRow > (SectionDimension.Height / RowDimension.Height) && item.Format.Section == SectionType.Body && rowNo < totalRows;

            //if(heightRow > (SectionDimension.Height / RowDimension.Height) && item.Format.Section == SectionType.Body && rowNo < totalRows)
            if(newPage)
            {
                if(SectionColumns > 1)
                {
                    if(myColumn < SectionColumns)
                    {
                        heightRow = 1;
                        myColumn++;
                        EndSection(builder);            //end column
                        StartColumnSection(builder);    //start new column
                    }
                    else
                    {
                        NewPage(builder, data);
                        heightRow = 1;
                        myColumn = 1;
                    }
                }
                else
                {
                    NewPage(builder, data);
                    heightRow = 1;
                    myColumn = 1;
                }
                rowNo++;
            }
        }
    }

    void NewPage(RenderTreeBuilder builder, ReportViewModel data)
    {
        EndSection(builder);            //end column
        EndSection(builder);            //end body
                                        //footer
        var newGroupedFooter = data.Data.Where(d => d.Section == SectionType.Footer)//.OrderBy(d => new { d.Position.Top, d.Position.Left, d.Foreground })
            .GroupBy(r => r.Row);

        CreateFooter(builder, data, newGroupedFooter);

        EndSection(builder);

        CurrentPage++;

        StartPage(builder, data);
        var newGroupedHeader = data.Data.Where(d => d.Section == SectionType.Header)//.OrderBy(d => new { d.Position.Top, d.Position.Left, d.Foreground })
             .GroupBy(r => r.Row);
        CreateHeader(builder, data, newGroupedHeader);

        StartBody(builder, data);
        StartColumnSection(builder);       //start first column
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
                    CurrentDivId++;
                    builder.OpenElement(CurrentDivId, "div");
                    builder.AddAttribute(CurrentDivId, "style", styleCol);
                    CurrentDivId++;
                    builder.OpenElement(CurrentDivId, "img");
                    string style = "display: block; width: inherit;";
                    builder.AddAttribute(CurrentDivId, "style", style);
                    builder.AddAttribute(CurrentDivId, "src", result);
                    builder.CloseElement();
                    CurrentDivId--;
                }
                else
                {
                    CurrentDivId++;
                    builder.OpenElement(CurrentDivId, "div");
                    builder.AddAttribute(CurrentDivId, "style", styleCol);
                    if(item.Column.PropertyName == "TotalPages") builder.AddContent(4, Totalpages);
                    else if(item.Column.PropertyName == "CurrentPage") builder.AddContent(4, CurrentPage);
                    else builder.AddContent(CurrentDivId, item.Value);
                }
                builder.CloseElement();
                CurrentDivId--;
            }
        }
    }

    string GetBase64(ColumnData item)
    {
        string base64 = string.Empty;
        if(item.Value is not null)
        {
            if(ImageValidator.IsLikelyImage(item.Value.ToString()))
            {
                byte[] bytes = new byte[] { };
                if(item.Value.GetType() == typeof(JsonElement))
                {
                    JsonElement data = (JsonElement)item.Value;
                    if(data.TryGetBytesFromBase64(out bytes))
                    {
                        base64 = SetBase64Image(bytes);
                    }
                }
                else if(item.Value.GetType() == typeof(byte[]))
                {
                    bytes = (byte[])item.Value;
                    base64 = SetBase64Image(bytes);
                }
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

    public async Task<PdfResponse> SaveAsFile(string pdfName)
    {
        PdfResponse response = new(); 
        byte[] report = await ReportPdf.GenerateReport(ReportModel);
        if(report.Length > 0)
        {
            response.Result = true;
            response.Base64String = Convert.ToBase64String(report);
            try
            {
                IJSObjectReference module = await ModuleTask.Value;
                await module.InvokeVoidAsync("PrintReports.SaveAsFile", pdfName, response.Base64String);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        response.Html = await GetHtml();
        return response;
    }

    async public Task<string> GetHtml()
    {
        string result;
        try
        {
            IJSObjectReference module = await ModuleTask.Value;
            PdfResponse response = await module.InvokeAsync<PdfResponse>("PrintReports.GetHtml", WrapperId);
            result = response.Html;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            result = string.Empty;
        }
        if(OnGetHtml.HasDelegate)         
            await OnGetHtml.InvokeAsync(result);
        return result;
    }


    public async ValueTask DisposeAsync()
    {
        try
        {
            if(ModuleTask.IsValueCreated)
            {
                IJSObjectReference module = await ModuleTask.Value;
                await module.DisposeAsync();
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
