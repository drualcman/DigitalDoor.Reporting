# DigitalDoor.Reporting
Using DigitalDoor Reporting to create ViewModel and use to export in PDF or use in DigitalDoor.Reporting.Blazor to preview in HTML.

## How to use:
Install nuget

```
dotnet add package DigitalDoor.Reporting --version 1.15.53
```

Register the services
``` csharp
    services.AddReportsServices();
```

## Create a report ViewModel
Create object page setup
```
    Setup reportSetUp = new(PageSize.A4, Orientation.Portrait);
```

Setup Header, Body and Footer (header and footer is not required, default all document it's a Body)
``` csharp
    reportSetUp.Header = new Section(new Format(210, 70));
    reportSetUp.Body = new Section()
    {
        Format = new Format()
        {
            Dimension = new Dimension(210, 222),        //full body size
        },
        Row = new Row()             //define size per each element to iterate
        {
            Dimension = new Dimension(89, 18)
        }
    };
    reportSetUp.Footer = new Section(new Format(210, 7.3));
```

Add columns (elements) to each section
``` csharp
    reportSetUp.Header.AddColumn(new ColumnSetup()
    {
        Format = new(165, 6) { FontDetails = new Font(new Shade(13)), Position = new(3, 7) },
        DataColumn = Item.SetItem<Model>(p => p.Name)
    });
    reportSetUp.Body.AddColumn(new ColumnSetup()
    {
        Format = new(165, 6) { FontDetails = new Font(new Shade(13)), Position = new(3, 7) },
        DataColumn = Item.SetItem<Model>(p => p.Name)
    });
    reportSetUp.Footer.AddColumn(new ColumnSetup()
    {
        Format = new(165, 6) { FontDetails = new Font(new Shade(13)), Position = new(3, 7) },
        DataColumn = Item.SetItem<Model>(p => p.Name)
    });
```

Add data to the collection. Data will be loop in a foreach to render the report
``` csharp
    List<Model> content = new List<Model>(await Repository.GetModelData());
    List<ColumnData> data = new List<ColumnData>();
    int row = 1;            //define first row in the section
    if(content.Any())
    {
        //header data
        data.Add(new ColumnData() { Section = SectionType.Header, Column = Item.SetItem<Model>(p => p.Name), Value = content.Name, Row = row });
        // more row data then increment row++
        //body data
        row = 1;            //reset row
        foreach(PackByTrolley item in content)
        {
            data.Add(new ColumnData() { Section = SectionType.Body, Column = Item.SetItem<Model>(p => p.Name), Value = content.Name, Row = row });
            row++;
        }
        //footer data
        row = 1;            //reset row
        data.Add(new ColumnData() { Section = SectionType.Footer, Column = Item.SetItem<Model>(p => p.Name), Value = content.Name, Row = row });      
    }
```

Full class example
``` csharp
    public class CreateReportHandler
    {
        readonly IReportsOutputPort Output;         //from digitalDoor.Reporting.Entities, registered by services.AddReportsServices();
        readonly IGetDataRepository Repository;

        public GetReportHandler(IReportsOutputPort output, IGetDataRepository repository)
        {
            Output = output;
            Repository = repository;
        }

        public async ValueTask Handle()
        {
            Setup reportSetUp = new(PageSize.A4, Orientation.Portrait);
            reportSetUp.Header.AddColumn(new ColumnSetup()
            {
                Format = new(165, 6) { FontDetails = new Font(new Shade(13)), Position = new(3, 7) },
                DataColumn = Item.SetItem<Model>(p => p.Name)
            });
            reportSetUp.Body.AddColumn(new ColumnSetup()
            {
                Format = new(165, 6) { FontDetails = new Font(new Shade(13)), Position = new(3, 7) },
                DataColumn = Item.SetItem<Model>(p => p.Name)
            });
            reportSetUp.Footer.AddColumn(new ColumnSetup()
            {
                Format = new(165, 6) { FontDetails = new Font(new Shade(13)), Position = new(3, 7) },
                DataColumn = Item.SetItem<Model>(p => p.Name)
            });

            List<Model> content = new List<Model>(await Repository.GetModelData());
            List<ColumnData> data = new List<ColumnData>();
            int row = 1;            //define first row in the section
            if(content.Any())
            {
                //header data
                data.Add(new ColumnData() { Section = SectionType.Header, Column = Item.SetItem<Model>(p => p.Name), Value = content.Name, Row = row });
                // more row data then increment row++
                //body data
                row = 1;            //reset row
                foreach(PackByTrolley item in content)
                {
                    data.Add(new ColumnData() { Section = SectionType.Body, Column = Item.SetItem<Model>(p => p.Name), Value = content.Name, Row = row });
                    row++;
                }
                //footer data
                row = 1;            //reset row
                data.Add(new ColumnData() { Section = SectionType.Footer, Column = Item.SetItem<Model>(p => p.Name), Value = content.Name, Row = row });
            }
            else
            {
                reportSetUp = new();
            }
            await Output.Handle(reportSetUp, data);     //create ReportViewModel in a property IReportsOutputPort.Content
        }
    }
```

## Create a PDF
To create a PDF need to manage the report ViewModel using a Handler and then use with the abtractions
``` csharp
    byte[] pdf = await IReportAsBytes.GenerateReport(reportModel);
```
IReportAsBytes it's registered by services.AddReportsServices();

