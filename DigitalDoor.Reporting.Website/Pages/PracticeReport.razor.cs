using DigitalDoor.Reporting.Entities.Helpers;
using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;
using DigitalDoor.Reporting.Entities.ViewModels;
using System.Runtime.CompilerServices;

namespace DigitalDoor.Reporting.Website.Pages
{
    public partial class PracticeReport
    {


        Setup ReportSetup;
        ReportViewModel Report;
        List<ColumnData> ReportData;


        protected override async Task OnInitializedAsync()
        {
            await SetupReport();
        }

        async Task SetupReport()
        {
            Report = null;
            ReportSetup = null;
            ReportData = null;
            List<Task> tasks = new()
            {
                SetUpReportSetup(),
                SetupReportData(),
            };
            await Task.WhenAll(tasks);
            Report = new(ReportSetup, ReportData);
            StateHasChanged();
        }
        #region This is the STRUCTURE OF THE REPORT
               
        #endregion
        async Task SetUpReportSetup()
        {
            ReportSetup = new()
            {
                Page = new Format() { Orientation = Orientation.Portrait, Dimension = PageSize.A4},
                Header = new Section() { Format = new Format(PageSize.A4.Width, 22) { Background = "#ff0000"}  },
                Body = new Section() { Format = new Format(PageSize.A4.Width, 80) { Background = "#dddddd"}, Row = new Row(new Dimension(PageSize.C7.Width, 3.3)) },
                Footer = new Section() { Format = new Format(PageSize.A4.Width, 12) {Background = "#0000FF" } }
            };
            await SetupReportHeader();
            await SetupReportBody();
            await SetupReportFooter();
        }
         Task SetupReportHeader()
        {
            ReportSetup.Header.AddColumn(new ColumnSetup()
            {
                Format = new(39, 5) { FontDetails = new Font("Courier", new Shade(8), new FontStyle(700)), TextAlignment = TextAlignment.Right, Position = new(0, 0), Background = "#FFC0CB",  Section = SectionType.Header },
                DataColumn = new Item("TerminalLabel")
            });

            return Task.CompletedTask;
        }
        Task SetupReportBody()
        {
            ReportSetup.Body.AddColumn(new ColumnSetup()
            {
                Format = new(PageSize.C7.Width, 4) { FontDetails = new Font("Courier", new Shade(8), new FontStyle(700)), TextAlignment = TextAlignment.Center, Position = new(0, 0), Section = SectionType.Body },
                DataColumn = new Item("InvoiceTextTopSpace")
            });
            ReportSetup.Body.AddColumn(new ColumnSetup()
            {
                Format = new(PageSize.C7.Width, 4) { FontDetails = new Font("Courier", new Shade(8), new FontStyle(700)), TextAlignment = TextAlignment.Center, Position = new(0, 0), Section = SectionType.Body },
                DataColumn = new Item("InvoiceText")
            });


            return Task.CompletedTask;
        }
        Task SetupReportFooter()
        {
            ReportSetup.Footer.AddColumn(new ColumnSetup()
            {
                Format = new(PageSize.C7.Width, 5) { FontDetails = new Font("Courier", new Shade(8), new FontStyle(700)),  TextAlignment = TextAlignment.Left, Position = new(0, 5)},
                DataColumn = new Item("SalesLabel")
            });

            ReportSetup.Footer.AddColumn(new ColumnSetup()
            {
                Format = new(PageSize.C7.Width - 8, 5) { FontDetails = new Font("Courier", new Shade(8), new FontStyle(700)), TextAlignment = TextAlignment.Right, Position = new(0, 0) },
                DataColumn = new Item("SalesValue")
            });

       
            ReportSetup.Footer.AddTotalPages(new Format(0, 0));
            return Task.CompletedTask;
        }



        #region THE DATA YOU WILL DISPLAY IN THE STRUCTURE YOU SET
        async Task SetupReportData()
        {
            ReportData = new();
            await SetupReportHeaderData();
            await SetupReportBodyData();
            await SetupReportFooterData();
        }

        Task SetupReportHeaderData()
        {
            ReportData.Add(new ColumnData() { Column = new Item("TerminalLabel"), Value = "Terminal: ", Section = SectionType.Header });
            return Task.CompletedTask;
        }
        Task SetupReportBodyData()
        {
            int rowNumber = 0;
            ReportData.Add(new ColumnData() { Column = new Item("InvoiceTextTopSpace"), Value = "zczc",Section = SectionType.Body, Row = rowNumber });
            rowNumber++;
            ReportData.Add(new ColumnData() { Column = new Item("InvoiceText"), Value = "Invoices", Section = SectionType.Body, Row = rowNumber });
          
            return Task.CompletedTask;
        }
        Task SetupReportFooterData()
        {
            ReportData.Add(new ColumnData() { Column = new Item("SalesLabel"), Value = "Sales:", Section = SectionType.Footer });
            ReportData.Add(new ColumnData() { Column = new Item("SalesValue"), Value = "Sales TAHO", Section = SectionType.Footer });
            return Task.CompletedTask;
        }

        #endregion
    }
}
