using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace searchIEEE
{

    public class IeeeRecord
    {
        public String Registry { get; set; }
        public String Assignment { get; set; }
        public String Oid
        {
            get
            {
                try
                {
                    Regex rgx = new Regex("[^a-fA-F0-9]");
                    return (rgx.Replace(Assignment, "").ToUpper().PadRight(12, '0'));
                }
                catch
                {
                    return ("FFFFFFFFFFFF");
                }
            }
        }
        public UInt64 Oid64
        {
            get
            {
                try
                {
                    Regex rgx = new Regex("[^a-fA-F0-9]");
                    return (Convert.ToUInt64("0x" + rgx.Replace(Assignment, "").ToUpper().PadRight(12, '0'), 16));
                }
                catch
                {
                    return (UInt64.MaxValue);
                }
            }
        }
        public String OrganizationName { get; set; }
        public String OrganizationAddress { get; set; }
        public String Protocol { get; set; }
    }

    public class IeeeRecords : IDisposable
    {
        public IEnumerable<IeeeRecord> data = null;
        private TextReader textReader = null;
        private Boolean disposedValue = false;

        public IeeeRecords(String fileName, String Url)
        {
            try
            {
                if (File.Exists(fileName) == false)
                {
                    using (WebClient wc = new WebClient())
                        File.WriteAllText(fileName, wc.DownloadString(new Uri(Url)));
                }

                textReader = File.OpenText(fileName);
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.IgnoreHeaderWhiteSpace = true;
                csv.Configuration.WillThrowOnMissingField = false;
                data = csv.GetRecords<IeeeRecord>().ToList();
            }
            catch
            {
                data = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(textReader != null)
                        textReader.Close();
                    textReader = null;
                    data = null;
                }

                disposedValue = true;
            }
        }

        ~IeeeRecords()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class ieeeCSV : IDisposable
    {
        private bool disposedValue = false;

        private struct databaseInfo
        {
            public String fileName;
            public String Url;
        }

        private List<IeeeRecords> databases = new List<IeeeRecords>();
        private List<databaseInfo> dbInfo = new List<databaseInfo>();

        public void Add(String fileName, String Url)
        {
            databaseInfo db = new databaseInfo();

            db.fileName = Path.Combine(Path.GetDirectoryName(Application.UserAppDataPath), fileName);
            db.Url = Url;
            dbInfo.Add(db);
        }

        public void loadAll()
        {
            foreach (databaseInfo db in dbInfo)
            {
                load(db.fileName, db.Url);
            }
        }

        public void load(String fileName, String Url)
        {
            IeeeRecords db = new IeeeRecords(fileName, Url);
            if (db != null)
                databases.Add(db);
        }

        public void deleteAll()
        {
            foreach (IeeeRecords db in databases)
            {
                if (db != null)
                    db.Dispose();
            }
            if (databases != null)
                databases.Clear();

            foreach (databaseInfo db in dbInfo)
            {
                if (File.Exists(db.fileName))
                    File.Delete(db.fileName);
            }
        }

        private UInt64? ConvertToOid64(String value)
        {
            try
            {
                Regex rgx = new Regex("[^a-fA-F0-9]");
                return (Convert.ToUInt64("0x" + rgx.Replace(value, "").ToUpper().PadRight(12, '0'), 16));
            }
            catch
            {
                return (null);
            }
        }

        public UInt64 Count()
        {
            UInt64 Count = 0;
            if (databases != null)
            {
                foreach (IeeeRecords database in databases)
                {
                    if(database != null)
                        if(database.data != null)
                        Count += (UInt64)database.data.Count();
                }
            }
            return (Count);
        }

        public List<IeeeRecord> search(String Needle)
        {
            try { 
            List<IeeeRecord> searchResults = new List<IeeeRecord>();

            if (Needle != String.Empty)
            {

                UInt64? Needle64 = ConvertToOid64(Needle);
                UInt64[] maskArray = null;
                if (Needle64 != null)
                {
                    UInt64[] array = { (UInt64)0xFFFFFFFFF000 & (UInt64)Needle64, (UInt64)0xFFFFFFFF0000 & (UInt64)Needle64, (UInt64)0xFFFFFFF00000 & (UInt64)Needle64, (UInt64)0xFFFFFF000000 & (UInt64)Needle64, (UInt64)0xFFFFF0000000 & (UInt64)Needle64, (UInt64)0xFFFF00000000 & (UInt64)Needle64, (UInt64)0xFFF000000000 & (UInt64)Needle64, (UInt64)0xFF0000000000 & (UInt64)Needle64 };
                    maskArray = array;
                }


                foreach (IeeeRecords database in databases)
                {
                    foreach (IeeeRecord row in database.data)
                    {
                        if (maskArray != null)
                        {
                            foreach (UInt64 mask in maskArray)
                            {
                                if (mask == row.Oid64)
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
                    searchResults.Sort((x, y) => y.Oid64.CompareTo(x.Oid64));

                    return (searchResults);
                }
                else {
                    return (null);
                }
            }
            else
            {
                foreach (IeeeRecords database in databases)
                {
                    foreach (IeeeRecord row in database.data)
                    {
                        searchResults.Add(row);
                    }
                }
                searchResults.Sort((x, y) => x.Oid64.CompareTo(y.Oid64));
                return (searchResults);
            }
            }
            catch
            {
                return (null);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (IeeeRecords database in databases)
                    {
                        if(database != null)
                            database.Dispose();
                    }
                    if(databases != null)
                        databases = null;
                }

                disposedValue = true;
            }
        }

        ~ieeeCSV()
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
