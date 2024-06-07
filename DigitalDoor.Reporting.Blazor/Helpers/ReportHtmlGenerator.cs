namespace DigitalDoor.Reporting.Blazor.Helpers;
public class ReportHtmlGenerator
{
    private readonly ReportViewModel ReportModel;
    private StringBuilder HtmlBuilder;
    private double SectionColumns;
    private Dimension SectionDimension;
    private Dimension RowDimension;
    private Border RowBorders;
    private int Totalpages;
    private int CurrentPage;
    private int ActiveZindex = 10;

    public ReportHtmlGenerator(ReportViewModel reportModel)
    {
        HtmlBuilder = new StringBuilder();
        ReportModel = reportModel;
    }

    public string GenerateHtml()
    {
        Totalpages = ReportModel.Pages;
        RenderReport(ReportModel);
        return HtmlBuilder.ToString();
    }

    private void RenderReport(ReportViewModel reportModel)
    {
        StartPage(reportModel);
        CurrentPage = reportModel.CurrentPage;

        IEnumerable<IGrouping<int, ColumnData>> grouped = reportModel.Data.Where(d => d.Section == SectionType.Header)
                                            .GroupBy(r => r.Row);
        CreateHeader(reportModel, grouped);

        grouped = reportModel.Data.Where(d => d.Section == SectionType.Body)
                                          .GroupBy(r => r.Row);
        StartBody(reportModel);
        StartColumnSection();
        CreateRow(grouped, reportModel, reportModel.Body.Items);
        EndSection();
        EndSection();

        grouped = reportModel.Data.Where(d => d.Section == SectionType.Footer)
                                            .GroupBy(r => r.Row);
        CreateFooter(reportModel, grouped);

        EndSection();
    }

    private void StartPage(ReportViewModel data)
    {
        Format format = new Format(data.Page);
        if (data.Page.Orientation == Orientation.Landscape)
        {
            format.Dimension = new Dimension(data.Page.Dimension.Height, data.Page.Dimension.Width);
        }

        string styleContainer = $"{GetStyle(format)}";
        HtmlBuilder.Append($"<div style='{styleContainer}'>");
    }

    private void CreateHeader(ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        string styleHeader = $"{GetStyle(data.Header.Format)}position: relative;";

        HtmlBuilder.Append($"<div style='{styleHeader}'>");

        SectionColumns = data.Header.ColumnsNumber;
        SectionDimension = data.Header.Format.Dimension;
        RowDimension = data.Header.Row.Dimension;
        if (data.Header.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Header.Row.Dimension.Height, data.Header.Row.Dimension.Width);
        }
        RowBorders = data.Header.Row.Borders;

        StartColumnSection();
        CreateRow(grouped, data, data.Header.Items);

        HtmlBuilder.Append("</div></div>");
    }

    private void StartBody(ReportViewModel data)
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
            styleBody = $"{GetStyle(data.Body.Format).Replace("display: block;", "")}display:grid; grid-template-columns:{frString}; position:relative; grid-auto-rows: {data.Body.Row.Dimension.Height}mm ";
        }
        else
        {
            styleBody = $"{GetStyle(data.Body.Format)}position:relative;";
        }
        HtmlBuilder.Append($"<div style='{styleBody}'>");

        SectionDimension = data.Body.Format.Dimension;
        RowDimension = data.Body.Row.Dimension;
        if (data.Body.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Body.Row.Dimension.Height, data.Body.Row.Dimension.Width);
        }
        RowBorders = data.Body.Row.Borders;
    }

    private void StartColumnSection()
    {
        string styleColumn = $"position:relative;overflow: hidden;height:{SectionDimension.Height}mm; width:{RowDimension.Width}mm;";
        HtmlBuilder.Append($"<div style='{styleColumn}'>");
    }

    private void CreateFooter(ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        string styleFooter = $"{GetStyle(data.Footer.Format)}position:relative;";

        HtmlBuilder.Append($"<div style='{styleFooter}'>");

        SectionColumns = data.Footer.ColumnsNumber;
        SectionDimension = data.Footer.Format.Dimension;
        RowDimension = data.Footer.Row.Dimension;
        if (data.Footer.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Footer.Row.Dimension.Height, data.Footer.Row.Dimension.Width);
        }
        RowBorders = data.Footer.Row.Borders;

        StartColumnSection();
        CreateRow(grouped, data, data.Footer.Items);

        HtmlBuilder.Append("</div>");
    }

    private void EndSection()
    {
        HtmlBuilder.Append("</div>");
    }

    private void CreateRow(IEnumerable<IGrouping<int, ColumnData>> grouped, ReportViewModel data, List<ColumnSetup> columns)
    {
        int heightRow = 1;
        int totalRows = grouped.Count();
        int rowNo = 1;
        int myColumn = 1;
        bool newPage = false;

        foreach (IGrouping<int, ColumnData> group in grouped)
        {
            ColumnData row = group.FirstOrDefault();
            ColumnSetup item = columns.FirstOrDefault(c => c.DataColumn.Equals(row.Column));

            string styleRow = $"position:relative;overflow: hidden;height:{RowDimension.Height}mm; width:{RowDimension.Width}mm;" +
                              $"border-style: {RowBorders.Style};" +
                              $"border-top-width: {RowBorders.Top.Width}mm;" +
                              $"border-top-color: {RowBorders.Top.Colour};" +
                              $"border-left-width: {RowBorders.Left.Width}mm;" +
                              $"border-left-color: {RowBorders.Left.Colour};" +
                              $"border-right-width: {RowBorders.Right.Width}mm;" +
                              $"border-right-color: {RowBorders.Right.Colour};" +
                              $"border-bottom-color: {RowBorders.Bottom.Colour};" +
                              $"border-bottom-width: {RowBorders.Bottom.Width}mm;";

            HtmlBuilder.Append($"<div style='{styleRow}'>");

            if (item != null)
            {
                CreateColumns(group, columns);
            }
            else
            {
                HtmlBuilder.Append("style='display:none'");
            }

            heightRow++;
            EndSection();

            newPage = heightRow > (SectionDimension.Height / RowDimension.Height) && item.Format.Section == SectionType.Body && rowNo < totalRows;

            if (newPage)
            {
                if (SectionColumns > 1)
                {
                    if (myColumn < SectionColumns)
                    {
                        heightRow = 1;
                        myColumn++;
                        EndSection();
                        StartColumnSection();
                    }
                    else
                    {
                        NewPage(data);
                        heightRow = 1;
                        myColumn = 1;
                    }
                }
                else
                {
                    NewPage(data);
                    heightRow = 1;
                    myColumn = 1;
                }
                rowNo++;
            }
        }
    }

    private void NewPage(ReportViewModel data)
    {
        EndSection();            //end column
        EndSection();            //end body

        IEnumerable<IGrouping<int, ColumnData>> newGroupedFooter = data.Data.Where(d => d.Section == SectionType.Footer)
                                         .GroupBy(r => r.Row);
        CreateFooter(data, newGroupedFooter);
        EndSection();

        CurrentPage++;

        StartPage(data);
        IEnumerable<IGrouping<int, ColumnData>> newGroupedHeader = data.Data.Where(d => d.Section == SectionType.Header)
                                         .GroupBy(r => r.Row);
        CreateHeader(data, newGroupedHeader);
        StartBody(data);
        StartColumnSection();       //start first column
    }

    private void CreateColumns(IGrouping<int, ColumnData> group, List<ColumnSetup> columns)
    {
        foreach (ColumnData item in group)
        {
            if (HasColumn(columns, item.Column))
            {
                Format columnFormat = GetColumnFormat(columns, item.Column);
                string styleCol = $"{GetStyle(item.Format ?? columnFormat)}position: absolute;";
                string base64 = GetBase64(item);

                if (!string.IsNullOrEmpty(base64))
                {
                    string result = $"data:image/png;base64,{base64}";
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
                    HtmlBuilder.Append($"<div style='{styleCol}'>");
                    HtmlBuilder.Append($"<img style='display: block;width:{itemFormat.Dimension.Width}mm;height:{itemFormat.Dimension.Height}mm;' src='{result}'/>");
                    HtmlBuilder.Append("</div>");
                }
                else
                {
                    HtmlBuilder.Append($"<div style='{styleCol}'>");
                    if (item.Column.PropertyName == "TotalPages")
                    {
                        HtmlBuilder.Append(Totalpages);
                    }
                    else if (item.Column.PropertyName == "CurrentPage")
                    {
                        HtmlBuilder.Append(CurrentPage);
                    }
                    else
                    {
                        HtmlBuilder.Append(item.Value);
                    }
                    HtmlBuilder.Append("</div>");
                }
            }
        }
    }

    private bool HasColumn(List<ColumnSetup> columns, Item item)
    {
        bool result;
        try
        {
            result = columns.Any(c => c.DataColumn.Equals(item)); 
        }
        catch
        {
            result = false;
        }
        return result;
    }

    private Format GetColumnFormat(List<ColumnSetup> columns, Item item)
    {
        Format format;
        try
        {
            format = columns.FirstOrDefault(c => c.DataColumn.Equals(item))?.Format;
        }
        catch
        {
            format = null;
        }
        return format;
    }

    private string GetBase64(ColumnData item)
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

    private string SetBase64Image(byte[] bytes)
    {
        return bytes.Length > 10 ? Convert.ToBase64String(bytes) : string.Empty;
    }

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
}
