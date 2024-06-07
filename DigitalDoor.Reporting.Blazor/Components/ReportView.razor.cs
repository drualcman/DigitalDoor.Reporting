namespace DigitalDoor.Reporting.Blazor.Components;

public partial class ReportView
{
    [Inject] GenerateReportAsBytes GenerateBytes { get; set; }
    [Parameter][EditorRequired] public ReportViewModel ReportModel { get; set; }
    [Parameter] public bool ShowPreview { get; set; } = true;
    [Parameter] public string WrapperId { get; set; } = $"doc{Guid.NewGuid().ToString().Replace("-", "")}";
    [Parameter] public EventCallback<string> OnGetHtml { get; set; }

    RenderFragment ContentFragment;
    MarkupString Content;

    double SectionColumns;
    Dimension SectionDimension;
    Dimension RowDimension;
    Border RowBorders;
    int Totalpages;
    int CurrentPage;
    int CurrentDivId;

    protected override void OnParametersSet()
    {
        ReportHtmlGenerator generator = new(ReportModel);
        Content = new MarkupString(generator.GenerateHtml());
        //RenderReport();
    }

    #region render page
    void RenderReport()
    {
        Totalpages = ReportModel.Pages;

        ContentFragment = builder =>
        {
            //create a page
            StartPage(builder, ReportModel);
            CurrentPage = ReportModel.CurrentPage;
            IEnumerable<IGrouping<int, ColumnData>> grouped = ReportModel.Data.Where(d => d.Section == SectionType.Header)
                .GroupBy(r => r.Row);

            CreateHeader(builder, ReportModel, grouped);

            //body
            grouped = ReportModel.Data.Where(d => d.Section == SectionType.Body).GroupBy(r => r.Row);

            StartBody(builder, ReportModel);                 //start body
            StartColumnSection(builder);       //start first column
            CreateRow(builder, grouped, ReportModel, ReportModel.Body.Items);
            EndSection(builder);                            //end column
            EndSection(builder);                            //end body

            //footer
            grouped = ReportModel.Data.Where(d => d.Section == SectionType.Footer).GroupBy(r => r.Row);
            CreateFooter(builder, ReportModel, grouped);

            EndSection(builder);
        };
    }

    void StartPage(RenderTreeBuilder builder, ReportViewModel data)
    {
        CurrentDivId = 0;
        builder.OpenElement(CurrentDivId, "div");

        Format format = new Format(data.Page);
        if (ReportModel.Page.Orientation == Orientation.Landscape)
        {
            format.Dimension = new Dimension(ReportModel.Page.Dimension.Height, ReportModel.Page.Dimension.Width);
        }

        string styleContainer = $"{GetStyle(format)}";

        builder.AddAttribute(CurrentDivId, "style", styleContainer);
    }

    void CreateHeader(RenderTreeBuilder builder, ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        CurrentDivId++;
        builder.OpenElement(CurrentDivId, "div");
        string styleHeader = $"{GetStyle(data.Header.Format)}position: relative;";
        //$"border-bottom: {data.Header.PageSetup.Padding.Bottom}mm solid #000;" +

        builder.AddAttribute(CurrentDivId, "style", styleHeader);

        SectionColumns = data.Header.ColumnsNumber;
        SectionDimension = data.Header.Format.Dimension;
        RowDimension = data.Header.Row.Dimension;
        if (data.Header.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Header.Row.Dimension.Height, data.Header.Row.Dimension.Width);
        }
        RowBorders = data.Header.Row.Borders;

        StartColumnSection(builder);       //start first column
        CreateRow(builder, grouped, data, data.Header.Items);
        EndSection(builder);            //end column
        builder.CloseElement();
        CurrentDivId--;
    }

    void StartBody(RenderTreeBuilder builder, ReportViewModel data)
    {
        string styleBody;
        SectionColumns = data.Body.ColumnsNumber;

        if (SectionColumns > 1)
        {
            StringBuilder frString = new StringBuilder();
            for (int i = 0; i < SectionColumns; i++)
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

        SectionDimension = data.Body.Format.Dimension;
        RowDimension = data.Body.Row.Dimension;
        if (data.Body.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Body.Row.Dimension.Height, data.Body.Row.Dimension.Width);
        }
        RowBorders = data.Body.Row.Borders;
    }

    void StartColumnSection(RenderTreeBuilder builder)
    {
        string styleColumn = $"position:relative;overflow: hidden;height:{SectionDimension.Height}mm; width:{RowDimension.Width}mm;";
        CurrentDivId++;
        builder.OpenElement(CurrentDivId, "div");
        builder.AddAttribute(CurrentDivId, "style", styleColumn);
    }

    void CreateFooter(RenderTreeBuilder builder, ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        string styleFooter = $"{GetStyle(data.Footer.Format)}position:relative;";

        CurrentDivId++;
        builder.OpenElement(CurrentDivId, "div");
        builder.AddAttribute(CurrentDivId, "style", styleFooter);

        SectionColumns = data.Footer.ColumnsNumber;
        SectionDimension = data.Footer.Format.Dimension;
        RowDimension = data.Footer.Row.Dimension;
        if (data.Footer.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Footer.Row.Dimension.Height, data.Footer.Row.Dimension.Width);
        }
        RowBorders = data.Footer.Row.Borders;

        StartColumnSection(builder);       //start first column
        CreateRow(builder, grouped, data, data.Footer.Items);
        EndSection(builder);            //end column
        builder.CloseElement();
        CurrentDivId--;
    }

    void EndSection(RenderTreeBuilder builder)
    {
        builder.CloseElement();
        CurrentDivId--;
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
        foreach (IGrouping<int, ColumnData> group in grouped)
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

            if (item != null)
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
            if (newPage)
            {
                if (SectionColumns > 1)
                {
                    if (myColumn < SectionColumns)
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

        IEnumerable<IGrouping<int, ColumnData>> newGroupedFooter = data.Data.Where(d => d.Section == SectionType.Footer).GroupBy(r => r.Row);

        CreateFooter(builder, data, newGroupedFooter);
        EndSection(builder);

        CurrentPage++;

        StartPage(builder, data);
        IEnumerable<IGrouping<int, ColumnData>> newGroupedHeader = data.Data.Where(d => d.Section == SectionType.Header).GroupBy(r => r.Row);
        CreateHeader(builder, data, newGroupedHeader);
        StartBody(builder, data);
        StartColumnSection(builder);       //start first column
    }

    void CreateColumns(RenderTreeBuilder builder, IGrouping<int, ColumnData> group,
        List<ColumnSetup> columns)
    {
        foreach (ColumnData item in group)
        {
            if (HasColumn(columns, item.Column))
            {
                Format columnFormat = GetColumnFormat(columns, item.Column);
                string styleCol = $"{GetStyle(item.Format ?? columnFormat)}position: absolute;";

                styleCol += "position: absolute;";
                string base64 = GetBase64(item);
                if (!string.IsNullOrEmpty(base64))
                {
                    string result = $"data:image/png;base64,{base64}";
                    CurrentDivId++;
                    builder.OpenElement(CurrentDivId, "div");
                    Format itemFormat = GetColumnFormat(columns, item.Column);
                    if (itemFormat.Angle != 0)
                    {
                        if (itemFormat.Angle < 0)
                        {
                            styleCol += $"top: {itemFormat.Position.Top - (decimal)itemFormat.Dimension.Height * 0.6m}mm;" +
                                $"right: {itemFormat.Position.Right}mm;" +
                                $"bottom: {itemFormat.Position.Bottom}mm;" +
                                $"left: {itemFormat.Position.Left - (decimal)itemFormat.Dimension.Height}mm;";
                        }
                        else
                        {
                            styleCol += $"top: {itemFormat.Position.Top + (decimal)itemFormat.Dimension.Height * 1.9m}mm;" +
                                        $"right: {itemFormat.Position.Right}mm;" +
                                        $"bottom: {itemFormat.Position.Bottom}mm;" +
                                        $"left: {itemFormat.Position.Left - (decimal)itemFormat.Dimension.Height / 1.6m}mm;";
                        }

                    }
                    builder.AddAttribute(CurrentDivId, "style", styleCol);
                    CurrentDivId++;
                    builder.OpenElement(CurrentDivId, "img");
                    string style = $"display: block;width:{itemFormat.Dimension.Width}mm;height:{itemFormat.Dimension.Height}mm;";
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
                    if (item.Column.PropertyName == "TotalPages") builder.AddContent(4, Totalpages);
                    else if (item.Column.PropertyName == "CurrentPage") builder.AddContent(4, CurrentPage);
                    else builder.AddContent(CurrentDivId, item.Value);
                }
                builder.CloseElement();
                CurrentDivId--;
            }
        }
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


    string GetBase64(ColumnData item)
    {
        string base64 = string.Empty;
        if (item.Value is not null)
        {
            if (item.Value.GetType() == typeof(byte[]))
            {
                base64 = SetBase64Image((byte[])item.Value);
            }
            else if (ImageValidator.IsLikelyImage(item.Value.ToString()))
            {
                if (item.Value.GetType() == typeof(JsonElement))
                {
                    JsonElement data = (JsonElement)item.Value;
                    if (data.TryGetBytesFromBase64(out byte[] bytes))
                    {
                        base64 = SetBase64Image(bytes);
                    }
                }
                else
                {
                    base64 = item.Value.ToString();
                }
            }
        }
        return base64;
    }

    string SetBase64Image(byte[] bytes)
    {
        return bytes.Length > 10 ? Convert.ToBase64String(bytes) : string.Empty;
    }

    int ActiveZindex = 10;

    string GetStyle(Format format)
    {
        StringBuilder styleBuilder = new();
        if (format is not null)
        {
            styleBuilder.Append($"width:{format.Dimension.Width}mm;")
                .Append($"height:{format.Dimension.Height}mm;")
                .Append($"background-color: {format.Background};")
                .Append($"padding-top: {format.Padding.Top}mm; ")
                .Append($"padding-right: {format.Padding.Right}mm; ")
                .Append($"padding-left: {format.Padding.Left}mm; ")
                .Append($"padding-bottom: {format.Padding.Bottom}mm;");

            if (format.Angle == 0)
            {
                styleBuilder.Append($"top: {format.Position.Top}mm;")
                            .Append($"right: {format.Position.Right}mm;")
                            .Append($"bottom: {format.Position.Bottom}mm;")
                            .Append($"left: {format.Position.Left}mm;");
            }
            else
            {
                if (format.Angle > 0)
                {
                    styleBuilder.Append($"top: {format.Position.Top + ((decimal)format.Dimension.Height)}mm;")
                                .Append($"right: {format.Position.Right}mm;")
                                .Append($"bottom: {format.Position.Bottom}mm;")
                                .Append($"left: {format.Position.Left - (decimal)format.Dimension.Height}mm;");
                }
                else
                {
                    styleBuilder.Append($"top: {format.Position.Top - ((decimal)format.Dimension.Height * 1.5m)}mm;")
                                .Append($"right: {format.Position.Right}mm;")
                                .Append($"bottom: {format.Position.Bottom}mm;")
                                .Append($"left: {format.Position.Left - (decimal)format.Dimension.Width / 2}mm;");
                }
            }

            styleBuilder.Append($"margin-top: {format.Margin.Top}mm;")
                        .Append($"margin-right: {format.Margin.Right}mm;")
                        .Append($"margin-bottom: {format.Margin.Bottom}mm;")
                        .Append($"margin-left: {format.Margin.Left}mm;")
                        .Append($"border-style: {format.Borders.Style};")
                        .Append($"border-top-width: {format.Borders.Top.Width}mm;")
                        .Append($"border-top-color: {format.Borders.Top.Colour};")
                        .Append($"border-left-width: {format.Borders.Left.Width}mm;")
                        .Append($"border-left-color: {format.Borders.Left.Colour};")
                        .Append($"border-right-width: {format.Borders.Right.Width}mm;")
                        .Append($"border-right-color: {format.Borders.Right.Colour};")
                        .Append($"border-bottom-color: {format.Borders.Bottom.Colour};")
                        .Append($"border-bottom-width: {format.Borders.Bottom.Width}mm;")
                        .Append($"border-top-left-radius: {format.Borders.Top.Radius.Left}mm;")
                        .Append($"border-top-right-radius: {format.Borders.Top.Radius.Right}mm;")
                        .Append($"border-bottom-right-radius: {format.Borders.Bottom.Radius.Right}mm;")
                        .Append($"border-bottom-left-radius: {format.Borders.Bottom.Radius.Left}mm;")
                        .Append($"transform:rotate({format.Angle}deg);")
                        .Append($"color: {format.FontDetails.ColorSize.Colour};")
                        .Append($"font-family: {format.FontDetails.FontName}, Helvetica, sans-serif;")
                        .Append($"font-weight: {format.FontDetails.FontStyle.Bold};")
                        .Append($"font-size: {format.FontDetails.ColorSize.Width}pt;")
                        .Append($"text-align: {format.TextAlignment};")
                        .Append($"text-decoration: {format.TextDecoration};")
                        .Append($"text-transform: none;")
                        .Append($"letter-spacing: 0;")
                        .Append($"z-index: {ActiveZindex};")
                        .Append($"overflow: hidden;visibility: visible; display: block;box-sizing: unset;");

            styleBuilder.Append(format.FontDetails.FontStyle.Italic.Equals(true) ? $"font-style: italic;" : "");
        }
        ActiveZindex++;
        return styleBuilder.ToString();
    }
    #endregion

    #region methods

    public async Task<string> GetHtml()
    {          
        if (OnGetHtml.HasDelegate)
            await OnGetHtml.InvokeAsync(Content.Value);
        return Content.Value;
    }
    #endregion
}
