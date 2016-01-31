using CsvHelper;
using searchIEEE.CustomExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace searchIEEE.Records
{
    public class IeeeRecordData : IDisposable
    {
        private bool disposedValue = false;
        public List<List<IeeeRecordDataItem>> DataBases = null;

        public void Add(String payLoad)
        {
            if (DataBases == null)
            {
                DataBases = new List<List<IeeeRecordDataItem>>();
            }

            try
            {
                using (TextReader textReader = new StringReader(payLoad))
                {
                    using (CsvReader csvReader = new CsvReader(textReader))
                    {
                        csvReader.Configuration.IgnoreHeaderWhiteSpace = true;
                        csvReader.Configuration.WillThrowOnMissingField = false;
                        List<IeeeRecordDataItem> DataBase = csvReader.GetRecords<IeeeRecordDataItem>().ToList();
                        DataBase.TrimExcess();
                        DataBases.Add(DataBase);
                        DataBases.TrimExcess();
                    }
                }
            }
            catch
            {
                // TODO Do i need to do something if the DB is not created?
            }
        }

        public UInt64 Count()
        {
            UInt64 Count = 0;
            if (DataBases != null)
            {
                foreach (List<IeeeRecordDataItem> database in DataBases)
                {
                    if (database != null)
                        Count += (UInt64)database.Count();
                }
            }
            return (Count);
        }

        public List<IeeeRecordDataItem> search(String Needle)
        {
            try
            {
                List<IeeeRecordDataItem> searchResults = new List<IeeeRecordDataItem>();

                if (Needle != String.Empty)
                {

                    UInt64? Needle64 = Needle.GetOid64();
                    UInt64[] maskArray = null;
                    if (Needle64 != UInt64.MaxValue)
                    {
                        UInt64[] array = { (UInt64)0xFFFFFFFFF000 & (UInt64)Needle64, (UInt64)0xFFFFFFFF0000 & (UInt64)Needle64, (UInt64)0xFFFFFFF00000 & (UInt64)Needle64, (UInt64)0xFFFFFF000000 & (UInt64)Needle64, (UInt64)0xFFFFF0000000 & (UInt64)Needle64, (UInt64)0xFFFF00000000 & (UInt64)Needle64, (UInt64)0xFFF000000000 & (UInt64)Needle64, (UInt64)0xFF0000000000 & (UInt64)Needle64 };
                        maskArray = array;
                    }


                    foreach (List<IeeeRecordDataItem> database in DataBases)
                    {
                        foreach (IeeeRecordDataItem row in database)
                        {
                            if (maskArray != null)
                            {
                                foreach (UInt64 mask in maskArray)
                                {
                                    if (mask == row.Assignment.GetOid64())
                                    {
                                        searchResults.Add(row);
                                        break;
                                    }
                                }
                            }

                            if (row.OrganizationName.IndexOf(Needle, StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                searchResults.Add(row);
                            }
                            else if (row.OrganizationAddress.IndexOf(Needle, StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                searchResults.Add(row);
                            }
                            else if (row.Protocol != null)
                            {
                                if (row.Protocol.IndexOf(Needle, StringComparison.OrdinalIgnoreCase) > -1)
                                {
                                    searchResults.Add(row);
                                }
                            }
                        }
                    }

                    if (searchResults.Count > 0)
                    {
                        searchResults.Sort((x, y) => y.Assignment.GetOid64().CompareTo(x.Assignment.GetOid64()));

                        return (searchResults);
                    }
                    else {
                        return (null);
                    }
                }
                else
                {
                    foreach (List<IeeeRecordDataItem> database in DataBases)
                    {
                        foreach (IeeeRecordDataItem row in database)
                        {
                            searchResults.Add(row);
                        }
                    }
                    searchResults.Sort((x, y) => x.Assignment.GetOid64().CompareTo(y.Assignment.GetOid64()));
                    return (searchResults);
                }
            }
            catch
            {
                return (null);
            }
        }

        public void clearDataBases()
        {
            foreach (List<IeeeRecordDataItem> database in DataBases)
            {
                if (database != null)
                {
                    database.Clear();
                    database.TrimExcess();
                }
            }
            if (DataBases != null)
            {
                DataBases.Clear();
                DataBases.TrimExcess();
            }
            GC.Collect();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    clearDataBases();
                }
                disposedValue = true;
            }
        }

        ~IeeeRecordData()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
