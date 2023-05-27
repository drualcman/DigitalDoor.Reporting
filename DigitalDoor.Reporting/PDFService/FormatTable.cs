using DigitalDoor.Reporting.Entities.Models;


namespace DigitalDoor.Reporting.PDF
{
    internal class ColumnFormat
    {
        public string Value { get; set; }
        public ColumnSetup Column { get; set; }
    }

    internal class FormatTable
    {
        public int Row { get; set; }
        public decimal Position { get; set; }
        public List<ColumnFormat> Columns { get; set; }

        public async Task<List<FormatTable>> GetTableFormat(List<ColumnSetup> setups, List<ColumnData> data)
        {
            List<FormatTable> Formats = new();
            bool HasRows = true;
            int TotalRows = data.Max(d => d.Row) + 1;
            List<Task> Tasks = new List<Task>();
            for (int Counter = 1; Counter < TotalRows; Counter++)
            {

                for (int i = 0; i < setups.Count; i++)
                {
                    var RowData = data.Where(d => d.Column.Equals(setups[i].DataColumn)).ToList();
                    var Data = RowData.Where(r => r.Row == Counter).FirstOrDefault();
                    if (Data == null)
                    {
                        HasRows = false;
                    }
                    else
                    {
                        FormatTable Format = Formats.Where(d => d.Position == setups[i].Format.Position.Top && d.Row == Counter).FirstOrDefault();
                        if (Format != null && Format.Row == Counter)
                        {
                            if (setups[i].Format.Position.Left != 0)
                            {
                                var NewSetup = new ColumnSetup() { Format = new Format() }; ;
                                NewSetup.Format.Dimension.Width = (double)setups[i].Format.Position.Left - (setups[i - 1].Format.Dimension.Width + (double)setups[i - 1].Format.Position.Left);
                                NewSetup.Format.Dimension.Height = (double)setups[i].Format.Dimension.Height;
                                Format.Columns.Add(new ColumnFormat()
                                {
                                    Column = NewSetup,
                                    Value = string.Empty
                                });
                                Format.Columns.Add(new ColumnFormat()
                                {
                                    Column = setups[i],
                                    Value = Data.Value.ToString()
                                });
                            }
                            else
                            {
                                Format.Columns.Add(new ColumnFormat()
                                {
                                    Column = setups[i],
                                    Value = Data.Value.ToString()
                                });
                            }

                        }
                        else
                        {
                            if (setups[i].Format.Position.Left != 0)
                            {
                                var NewSetup = new ColumnSetup() { Format = new Format() };
                                NewSetup.Format.Dimension.Width = (double)setups[i].Format.Position.Left;
                                NewSetup.Format.Dimension.Height = (double)setups[i].Format.Dimension.Height;
                                Format = new FormatTable()
                                {
                                    Row = Counter,
                                    Position = setups[i].Format.Position.Top,
                                    Columns = new List<ColumnFormat>()
                                {
                                        new ColumnFormat()
                                     {
                                        Column = NewSetup,
                                        Value = string.Empty,
                                    }
                                }
                                };
                                Formats.Add(Format);
                                Format.Columns.Add(new ColumnFormat()
                                {
                                    Column = setups[i],
                                    Value = Data.Value.ToString()
                                });
                            }
                            else
                            {
                                Format = new FormatTable()
                                {
                                    Row = Counter,
                                    Position = setups[i].Format.Position.Top,
                                    Columns = new List<ColumnFormat>()
                                {
                                        new ColumnFormat()
                                    {
                                        Column = setups[i],
                                        Value = Data.Value.ToString()
                                    }
                                }
                                };
                                Formats.Add(Format);
                            }
                        }
                    }
                }

            }
            return Formats;
        }
    }
}
