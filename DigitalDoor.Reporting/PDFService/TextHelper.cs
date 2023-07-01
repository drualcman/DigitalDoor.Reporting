using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.PDF;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalDoor.Reporting.PDFService
{
    internal class TextHelper
    {
        public float MillimeterToPixel(double milimiter)
        {
            return (float)(milimiter*2.83);
        }

        public float MillimeterToPixel(decimal milimiter)
        {
            return (float)((double)milimiter*2.83);
        }

        public List<List<ColumnContent>> Split(List<ColumnContent> original, int sizeList)
        {
            List<List<ColumnContent>> Result = new List<List<ColumnContent>>();
            int Index = 0;
            if(original is not null)
            {
                while (Index < original.Count)
                {
                    List<ColumnContent> List = original.GetRange(Index, Math.Min(sizeList, original.Count - Index));
                    Result.Add(List);
                    Index += sizeList;
                }
            }
            else
            {
                Result.Add(new List<ColumnContent>());
            }
            return Result;
        }


        public List<ColumnContent> GetElements(List<ColumnSetup> setups, List<ColumnData> data)
        {
            List<ColumnContent> Elements = default;
            if (data.Count > 0)
            {
                Elements = GetColumnsContent(setups, data);
            }
            return Elements;
        }

        private List<ColumnContent> GetColumnsContent(List<ColumnSetup> setups, List<ColumnData> data)
        {
            List<ColumnContent> ColumnsContent = new();
            int TotalRows = data.Max(d => d.Row) + 1;
            for (int Counter = 0; Counter < TotalRows; Counter++)
            {
                ColumnContent Content = new ColumnContent();
                Content.Columns = new List<ColumnContent>();
                for (int i = 0; i < setups.Count; i++)
                {
                    List<ColumnData> RowData = data.Where(d => d.Column.Equals(setups[i].DataColumn)).ToList();
                    ColumnData Data = RowData.FirstOrDefault(r => r.Row == Counter);
                    if (Data != null)
                    {
                        if (Data?.Value.GetType() == typeof(byte[]))
                        {
                            Content.Columns.Add(new ColumnContent()
                            {
                                Column = setups[i],
                                Image = (byte[])Data.Value
                            });
                        }
                        else
                        {
                            Content.Columns.Add(new ColumnContent()
                            {
                                Column = setups[i],
                                Value = Data.Value.ToString()
                            });
                        }
                    }
                }
                if (Content.Columns.Count > 0)
                {
                    ColumnsContent.Add(Content);
                }
            }
            return ColumnsContent;
        }
    }
}
