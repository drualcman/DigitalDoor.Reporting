# DigitalDoor.Reporting.Entities
Object and abstraction for can create a report using DigitalDoor.Reporting

## How to use:
Install nuget

```
dotnet add package DigitalDoor.Reporting --version 1.15.53
```

## Interfaces
``` csharp
IPDFReportOutputPort;
IPDFReportPresenter;
IReportAsBytes;
IReportDataRepository;
IReportsOutputPort;
IReportsPresenter;
```

## Helper Class
namespace DigitalDoor.Reporting.Entities.Helpers;
``` csharp
public class PageSize
{
    public static readonly Dimension _4A0 = new Dimension(1682, 2378);
    public static readonly Dimension _2A0 = new Dimension(1189, 1682);
    public static readonly Dimension A0 = new Dimension(841, 1189);
    public static readonly Dimension A1 = new Dimension(594, 841);
    public static readonly Dimension A2 = new Dimension(420, 594);
    public static readonly Dimension A3 = new Dimension(297, 420);
    public static readonly Dimension A4 = new Dimension(210, 297);
    public static readonly Dimension A5 = new Dimension(148, 210);
    public static readonly Dimension A6 = new Dimension(105, 148);
    public static readonly Dimension A7 = new Dimension(74, 105);
    public static readonly Dimension A8 = new Dimension(52, 74);
    public static readonly Dimension A9 = new Dimension(37, 52);
    public static readonly Dimension A10 = new Dimension(26, 37);

    public static readonly Dimension B0 = new Dimension(1000, 1000);
    public static readonly Dimension B1 = new Dimension(707, 707);
    public static readonly Dimension B2 = new Dimension(500, 500);
    public static readonly Dimension B3 = new Dimension(353, 353);
    public static readonly Dimension B4 = new Dimension(250, 353);
    public static readonly Dimension B5 = new Dimension(176, 250);
    public static readonly Dimension B6 = new Dimension(125, 176);
    public static readonly Dimension B7 = new Dimension(88, 125);
    public static readonly Dimension B8 = new Dimension(62, 88);
    public static readonly Dimension B9 = new Dimension(44, 62);
    public static readonly Dimension B10 = new Dimension(31, 44);

    public static readonly Dimension C0 = new Dimension(917, 1297);
    public static readonly Dimension C1 = new Dimension(648, 917);
    public static readonly Dimension C2 = new Dimension(458, 648);
    public static readonly Dimension C3 = new Dimension(324, 458);
    public static readonly Dimension C4 = new Dimension(229, 324);
    public static readonly Dimension C5 = new Dimension(162, 229);
    public static readonly Dimension C6 = new Dimension(114, 162);
    public static readonly Dimension C7 = new Dimension(81, 114);
    public static readonly Dimension C8 = new Dimension(57, 81);
    public static readonly Dimension C9 = new Dimension(40, 57);
    public static readonly Dimension C10 = new Dimension(28, 40);
}

public static class ImageValidator
{
    public static bool IsLikelyImage(string base64String);
    public static bool IsBase64String(string input);
    public static bool IsLikelyBase64(string input);
}

```