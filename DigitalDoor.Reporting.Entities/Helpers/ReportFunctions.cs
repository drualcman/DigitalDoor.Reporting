using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ValueObjects;
using System.Reflection;

namespace DigitalDoor.Reporting.Entities.Helpers;

public class ReportFunctions
{
    public object SetValue(Item column, object data)
    {
        PropertyInfo property = Property(data.GetType().GetProperties(), column.PropertyName);
        if(property is not null) return property.GetValue(data);
        else return null;
    }

    PropertyInfo Property(PropertyInfo[] properties, string propertyName)
    {
        int c = properties.Length;
        int p = 0;
        PropertyInfo result = null;
        do
        {
            if(properties[p].Name == propertyName)
            {
                result = properties[p];
                p = c;
            }
            else p++;
        } while(p < c);
        return result;
    }

    public static void AddPagination(List<ColumnData> data, SectionType section)
    {
        data.Add(new ColumnData
        {
            Value = "",
            Section = section,
            Column = new Item
            {
                PropertyName = "TotalPages",
                ObjectName = "FreeText"
            }
        });
        data.Add(new ColumnData
        {
            Value = "",
            Column = new Item
            {
                PropertyName = "CurrentPage",
                ObjectName = "FreeText"
            },
            Section = section
        });
    }

    public string GetPaperSizeName(Dimension size)
    {
        string paperSizeName = string.Empty;

        Type s = typeof(PageSize);
        FieldInfo[] myPropertyInfos = s.GetFields();
        int count = myPropertyInfos.Count();
        int i = 0;
        do
        {
            if(myPropertyInfos[i].FieldType == size.GetType())
            {
                Dimension dimension = myPropertyInfos[i].GetValue(null) as Dimension;
                if(dimension.Width == size.Width && dimension.Height == size.Height) paperSizeName = myPropertyInfos[i].Name;
                else if(dimension.Height == size.Width && dimension.Width == size.Height) paperSizeName = myPropertyInfos[i].Name;
                else paperSizeName = string.Empty;
            }
            i++;
        } while(string.IsNullOrEmpty(paperSizeName) && i < count);
        if(string.IsNullOrEmpty(paperSizeName)) paperSizeName = "A4";
        return paperSizeName;
    }
}