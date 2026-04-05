namespace DigitalDoor.Reporting.Entities.Helpers;

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
        if (ReportModel == null)
            return string.Empty;

        Totalpages = ReportModel.Pages;
        RenderReport(ReportModel);
        return HtmlBuilder.ToString();
    }

    private void RenderReport(ReportViewModel reportModel)
    {
        StartPage(reportModel);
        CurrentPage = reportModel.CurrentPage;

        // Seguridad en LINQ: Verificamos que Data no sea nulo
        var data = reportModel.Data ?? Enumerable.Empty<ColumnData>();

        // HEADER
        if (reportModel.Header != null)
        {
            var headerGrouped = data.Where(d => d.Section == SectionType.Header).GroupBy(r => r.Row);
            CreateHeader(reportModel, headerGrouped);
        }

        // BODY
        if (reportModel.Body != null)
        {
            var bodyGrouped = data.Where(d => d.Section == SectionType.Body).GroupBy(r => r.Row);
            StartBody(reportModel);
            StartColumnSection();
            CreateRow(bodyGrouped, reportModel, reportModel.Body.Items ?? new List<ColumnSetup>());
            EndSection(); // end column
            EndSection(); // end body
        }

        // FOOTER
        if (reportModel.Footer != null)
        {
            var footerGrouped = data.Where(d => d.Section == SectionType.Footer).GroupBy(r => r.Row);
            CreateFooter(reportModel, footerGrouped);
        }

        EndSection();
    }

    private void StartPage(ReportViewModel data)
    {
        if (data?.Page == null)
            return;

        Format format = new Format(data.Page);
        if (data.Page.Orientation == Orientation.Landscape && data.Page.Dimension != null)
        {
            format.Dimension = new Dimension(data.Page.Dimension.Height, data.Page.Dimension.Width);
        }

        string styleContainer = GetStyle(format);
        HtmlBuilder.Append($"<div style='{styleContainer}'>");
    }

    private void CreateHeader(ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        if (data?.Header?.Format == null)
            return;

        string styleHeader = $"{GetStyle(data.Header.Format)}position: relative;";
        HtmlBuilder.Append($"<div style='{styleHeader}'>");

        SectionColumns = data.Header.ColumnsNumber;
        SectionDimension = data.Header.Format.Dimension;
        RowDimension = data.Header.Row?.Dimension;
        if (data.Header.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Header.Row.Dimension.Height, data.Header.Row.Dimension.Width);
        }
        RowBorders = data.Header.Row?.Borders;

        StartColumnSection();
        CreateRow(grouped, data, data.Header.Items ?? new List<ColumnSetup>());
        EndSection();
        HtmlBuilder.Append("</div>");
    }

    private void StartBody(ReportViewModel data)
    {
        if (data?.Body?.Format == null)
            return;

        SectionColumns = data.Body.ColumnsNumber;
        string styleBody;

        if (SectionColumns > 1)
        {
            StringBuilder frString = new StringBuilder();
            for (int i = 0; i < SectionColumns; i++)
                frString.Append("1fr ");

            string baseStyle = GetStyle(data.Body.Format).Replace("display: block;", "");
            styleBody = $"{baseStyle}display:grid; grid-template-columns:{frString}; position:relative; grid-auto-rows: {data.Body.Row?.Dimension?.Height ?? 0}mm ";
        }
        else
        {
            styleBody = $"{GetStyle(data.Body.Format)}position:relative;";
        }

        HtmlBuilder.Append($"<div style='{styleBody}'>");

        SectionDimension = data.Body.Format.Dimension;
        RowDimension = data.Body.Row?.Dimension;
        if (data.Body.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Body.Row.Dimension.Height, data.Body.Row.Dimension.Width);
        }
        RowBorders = data.Body.Row?.Borders;
    }

    private void StartColumnSection()
    {
        if (SectionDimension == null || RowDimension == null)
            return;
        string styleColumn = $"position:relative;overflow: hidden;height:{SectionDimension.Height}mm; width:{RowDimension.Width}mm;";
        HtmlBuilder.Append($"<div style='{styleColumn}'>");
    }

    private void CreateFooter(ReportViewModel data, IEnumerable<IGrouping<int, ColumnData>> grouped)
    {
        if (data?.Footer?.Format == null)
            return;

        string styleFooter = $"{GetStyle(data.Footer.Format)}position:relative;";
        HtmlBuilder.Append($"<div style='{styleFooter}'>");

        SectionColumns = data.Footer.ColumnsNumber;
        SectionDimension = data.Footer.Format.Dimension;
        RowDimension = data.Footer.Row?.Dimension;
        if (data.Footer.Format.Orientation == Orientation.Landscape)
        {
            RowDimension = new Dimension(data.Footer.Row.Dimension.Height, data.Footer.Row.Dimension.Width);
        }
        RowBorders = data.Footer.Row?.Borders;

        StartColumnSection();
        CreateRow(grouped, data, data.Footer.Items ?? new List<ColumnSetup>());
        EndSection();
        HtmlBuilder.Append("</div>");
    }

    private void EndSection() => HtmlBuilder.Append("</div>");

    private void CreateRow(IEnumerable<IGrouping<int, ColumnData>> grouped, ReportViewModel data, List<ColumnSetup> columns)
    {
        if (grouped == null)
            return;

        int heightRow = 1;
        int totalRows = grouped.Count();
        int rowNo = 1;
        int myColumn = 1;

        foreach (var group in grouped)
        {
            lock (HtmlBuilder)
            {
                ColumnData row = group.FirstOrDefault();
                if (row == null)
                    continue;

                ColumnSetup item = columns?.FirstOrDefault(c => c.DataColumn != null && c.DataColumn.Equals(row.Column));

                StringBuilder styleRowBuilder = new StringBuilder();
                styleRowBuilder.Append("position:relative;overflow: hidden;")
                               .Append($"height:{RowDimension?.Height ?? 0}mm; ")
                               .Append($"width:{RowDimension?.Width ?? 0}mm; ")
                               .Append($"border-style: {RowBorders?.Style ?? BorderStyle.none}; ")
                               .Append($"border-top-width: {RowBorders?.Top?.Width ?? 0}mm; ")
                               .Append($"border-top-color: {RowBorders?.Top?.Colour ?? "transparent"}; ")
                               .Append($"border-left-width: {RowBorders?.Left?.Width ?? 0}mm; ")
                               .Append($"border-left-color: {RowBorders?.Left?.Colour ?? "transparent"}; ")
                               .Append($"border-right-width: {RowBorders?.Right?.Width ?? 0}mm; ")
                               .Append($"border-right-color: {RowBorders?.Right?.Colour ?? "transparent"}; ")
                               .Append($"border-bottom-color: {RowBorders?.Bottom?.Colour ?? "transparent"}; ")
                               .Append($"border-bottom-width: {RowBorders?.Bottom?.Width ?? 0}mm;");


                HtmlBuilder.Append($"<div style='{styleRowBuilder}'>");
                if (item != null)
                {
                    CreateColumns(group, columns);
                }
                EndSection();

                heightRow++;

                // Validación de nulos antes de calcular nueva página
                bool canCalculateNewPage = SectionDimension != null && RowDimension != null && RowDimension.Height != 0;
                bool isBody = item?.Format?.Section == SectionType.Body;

                if (canCalculateNewPage && isBody && heightRow > (SectionDimension.Height / RowDimension.Height) && rowNo < totalRows)
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
                }
                rowNo++;
            }
        }
    }

    private void NewPage(ReportViewModel data)
    {
        EndSection(); // end column
        EndSection(); // end body

        var footerData = data?.Data?.Where(d => d.Section == SectionType.Footer).GroupBy(r => r.Row);
        CreateFooter(data, footerData);
        EndSection();

        CurrentPage++;

        StartPage(data);
        var headerData = data?.Data?.Where(d => d.Section == SectionType.Header).GroupBy(r => r.Row);
        CreateHeader(data, headerData);
        StartBody(data);
        StartColumnSection();
    }

    private void CreateColumns(IGrouping<int, ColumnData> group, List<ColumnSetup> columns)
    {
        foreach (ColumnData item in group)
        {
            if (item is not null && HasColumn(columns, item.Column))
            {
                Format columnFormat = GetColumnFormat(columns, item.Column);
                string styleCol = $"{GetStyle(item.Format ?? columnFormat)}position: absolute;";
                string base64 = GetBase64(item);

                if (!string.IsNullOrEmpty(base64))
                {
                    Format itemFormat = GetColumnFormat(columns, item.Column);
                    if (itemFormat?.Position != null && itemFormat.Dimension != null)
                    {
                        if (itemFormat.Angle < 0)
                        {
                            styleCol += $"top: {itemFormat.Position.Top - (decimal)itemFormat.Dimension.Height * 0.6m}mm;" +
                                        $"left: {itemFormat.Position.Left - (decimal)itemFormat.Dimension.Height}mm;";
                        }
                        else if (itemFormat.Angle > 0)
                        {
                            styleCol += $"top: {itemFormat.Position.Top + (decimal)itemFormat.Dimension.Height * 1.9m}mm;" +
                                        $"left: {itemFormat.Position.Left - (decimal)itemFormat.Dimension.Height / 1.6m}mm;";
                        }
                    }
                    HtmlBuilder.Append($"<div style='{styleCol}'>");
                    HtmlBuilder.Append($"<img style='display: block;width:{itemFormat?.Dimension?.Width ?? 0}mm;height:{itemFormat?.Dimension?.Height ?? 0}mm;' src='data:image/png;base64,{base64}'/>");
                    HtmlBuilder.Append("</div>");
                }
                else
                {
                    HtmlBuilder.Append($"<div style='{styleCol}'>");
                    if (item.Column?.PropertyName == "TotalPages")
                        HtmlBuilder.Append(Totalpages);
                    else if (item.Column?.PropertyName == "CurrentPage")
                        HtmlBuilder.Append(CurrentPage);
                    else
                        HtmlBuilder.Append(item.Value ?? string.Empty);
                    HtmlBuilder.Append("</div>");
                }
            }
        }
    }

    private bool HasColumn(List<ColumnSetup> columns, Item item)
    {
        if (columns == null || item == null)
            return false;
        return columns.Any(c => c.DataColumn != null && c.DataColumn.Equals(item));
    }

    private Format GetColumnFormat(List<ColumnSetup> columns, Item item)
    {
        if (columns == null || item == null)
            return null;
        return columns.FirstOrDefault(c => c.DataColumn != null && c.DataColumn.Equals(item))?.Format;
    }

    private string GetBase64(ColumnData item)
    {
        if (item?.Value == null)
            return string.Empty;

        if (item.Value is byte[] bytes)
            return SetBase64Image(bytes);

        string valStr = item.Value.ToString();
        if (ImageValidator.IsLikelyImage(valStr))
        {
            if (item.Value is JsonElement data && data.ValueKind == JsonValueKind.String)
            {
                if (data.TryGetBytesFromBase64(out byte[] b))
                    return SetBase64Image(b);
            }
            return valStr;
        }
        return string.Empty;
    }

    private string SetBase64Image(byte[] bytes) => (bytes?.Length > 10) ? Convert.ToBase64String(bytes) : string.Empty;

    private string GetStyle(Format format)
    {
        if (format == null)
            return string.Empty;

        StringBuilder sb = new();

        // Dimensiones
        sb.Append($"width:{format.Dimension?.Width ?? 0}mm;")
          .Append($"height:{format.Dimension?.Height ?? 0}mm;")
          .Append($"background-color: {format.Background ?? "transparent"};");

        // Padding
        if (format.Padding != null)
        {
            sb.Append($"padding-top: {format.Padding.Top}mm; ")
              .Append($"padding-right: {format.Padding.Right}mm; ")
              .Append($"padding-left: {format.Padding.Left}mm; ")
              .Append($"padding-bottom: {format.Padding.Bottom}mm;");
        }

        // Posicionamiento
        if (format.Position != null)
        {
            if (format.Angle == 0)
            {
                sb.Append($"top: {format.Position.Top}mm;").Append($"right: {format.Position.Right}mm;")
                  .Append($"bottom: {format.Position.Bottom}mm;").Append($"left: {format.Position.Left}mm;");
            }
            else
            {
                decimal h = (decimal)(format.Dimension?.Height ?? 0);
                if (format.Angle > 0)
                    sb.Append($"top: {format.Position.Top + h}mm;").Append($"left: {format.Position.Left - h}mm;");
                else
                    sb.Append($"top: {format.Position.Top - h * 1.5m}mm;").Append($"left: {format.Position.Left - (decimal)(format.Dimension?.Width ?? 0) / 2}mm;");
            }
        }

        // Márgenes y Bordes
        if (format.Margin != null)
            sb.Append($"margin-top: {format.Margin.Top}mm;").Append($"margin-left: {format.Margin.Left}mm;");

        if (format.Borders != null)
        {
            sb.Append($"border-style: {format.Borders?.Style ?? BorderStyle.none};")
              .Append($"border-top-width: {RowBorders?.Top?.Width ?? 0}mm; ")
              .Append($"border-top-color: {RowBorders?.Top?.Colour ?? "transparent"}; ")
              .Append($"border-left-width: {RowBorders?.Left?.Width ?? 0}mm; ")
              .Append($"border-left-color: {RowBorders?.Left?.Colour ?? "transparent"}; ")
              .Append($"border-right-width: {RowBorders?.Right?.Width ?? 0}mm; ")
              .Append($"border-right-color: {RowBorders?.Right?.Colour ?? "transparent"}; ")
              .Append($"border-bottom-color: {RowBorders?.Bottom?.Colour ?? "transparent"}; ")
              .Append($"border-bottom-width: {RowBorders?.Bottom?.Width ?? 0}mm;");
        }

        // Fuente
        if (format.FontDetails != null)
        {
            sb.Append($"color: {format.FontDetails.ColorSize?.Colour ?? "black"};")
              .Append($"font-family: {format.FontDetails.FontName ?? "sans-serif"};")
              .Append($"font-weight: {format.FontDetails.FontStyle?.Bold ?? 500};")
              .Append($"font-size: {format.FontDetails.ColorSize?.Width ?? 10}pt;")
              .Append($"font-style: {(format.FontDetails.FontStyle?.Italic == true ? "italic" : "normal")};");
        }

        sb.Append($"transform:rotate({format.Angle}deg);")
          .Append($"text-align: {format.TextAlignment};")
          .Append($"z-index: {ActiveZindex++};")
          .Append("overflow: hidden; visibility: visible; display: block; box-sizing: unset;");

        return sb.ToString();
    }
}