using CsvHelper;
using searchIEEE.CustomExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace searchIEEE.Records
{

    public class Items
    {
        public String Registry { get; set; }
        public String Assignment { get; set; }
        public String OrganizationName { get; set; }
        public String OrganizationAddress { get; set; }
        public String Protocol { get; set; }
        public String Oid { get { return (Assignment.GetOid()); } }
        public UInt64 Oid64 { get { return (Assignment.GetOid64()); } }
        public String RegistryID
        {
            get
            {
                if(Registry == String.Empty)
                {
                    return ("N/A");
                }
                else
                {
                    return (Registry);
                }
            }
        }
        public String Name
        {
            get
            {
                if (OrganizationName == String.Empty)
                {
                    return ("N/A");
                }
                else {
                    return (OrganizationName);
                }
            }
        }
        public String Address
        {
            get
            {
                if (OrganizationAddress == String.Empty)
                {
                    return ("N/A");
                }
                else {
                    return (OrganizationAddress);
                }
            }
        }
    }

    public class Data
    {
        public List<List<Items>> DataBases = null;

        public void Add(String payLoad)
        {
            if (DataBases == null)
            {
                DataBases = new List<List<Items>>();
            }

            try
            {
                using (TextReader textReader = new StringReader(payLoad))
                {
                    using (CsvReader csvReader = new CsvReader(textReader))
                    {
                        csvReader.Configuration.IgnoreHeaderWhiteSpace = true;
                        csvReader.Configuration.WillThrowOnMissingField = false;
                        List<Items> DataBase = csvReader.GetRecords<Items>().ToList();
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
                foreach (List<Items> database in DataBases)
                {
                    if (database != null)
                        Count += (UInt64)database.Count();
                }
            }
            return (Count);
        }

        public Task<List<Items>> searchAsync(String Needle)
        {
            return(Task.Run<List<Items>>(() => search(Needle)));   
        }

        public List<Items> search(String Needle)
        {
            try
            {
                List<Items> searchResults = new List<Items>();

                if (Needle != String.Empty)
                {

                    UInt64? Needle64 = Needle.GetOid64();
                    UInt64[] maskArray = null;
                    if (Needle64 != UInt64.MaxValue)
                    {
                        UInt64[] array = { (UInt64)0xFFFFFFFFF000 & (UInt64)Needle64, (UInt64)0xFFFFFFFF0000 & (UInt64)Needle64, (UInt64)0xFFFFFFF00000 & (UInt64)Needle64, (UInt64)0xFFFFFF000000 & (UInt64)Needle64, (UInt64)0xFFFFF0000000 & (UInt64)Needle64, (UInt64)0xFFFF00000000 & (UInt64)Needle64, (UInt64)0xFFF000000000 & (UInt64)Needle64, (UInt64)0xFF0000000000 & (UInt64)Needle64 };
                        maskArray = array;
                    }


                    foreach (List<Items> database in DataBases)
                    {
                        foreach (Items row in database)
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
                    foreach (List<Items> database in DataBases)
                    {
                        foreach (Items row in database)
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
    }

    public class Loader
    {
        public enum States { start, update, done };
        public delegate void Callback(States callBackState, Int32 Count, Int32 Total, UInt64 RecordCount);

        public static async Task<Data> loadAsync(Configuration.Data configuration, Boolean doReset, Callback callback)
        {
            Int32 Count = 0;
            List<Configuration.Manager.DatabaseInfo> databasesInfo = new List<Configuration.Manager.DatabaseInfo>();
            Data ieeeRecordData = new Data();

            databasesInfo.Add(Configuration.Manager.getDatabaseInfo(ref configuration, Configuration.Manager.ConfigurationElements.IEEE_MAL));
            databasesInfo.Add(Configuration.Manager.getDatabaseInfo(ref configuration, Configuration.Manager.ConfigurationElements.IEEE_MAM));
            databasesInfo.Add(Configuration.Manager.getDatabaseInfo(ref configuration, Configuration.Manager.ConfigurationElements.IEEE_MAS));
            databasesInfo.Add(Configuration.Manager.getDatabaseInfo(ref configuration, Configuration.Manager.ConfigurationElements.IEEE_IAB));
            databasesInfo.Add(Configuration.Manager.getDatabaseInfo(ref configuration, Configuration.Manager.ConfigurationElements.IEEE_CID));
            databasesInfo.Add(Configuration.Manager.getDatabaseInfo(ref configuration, Configuration.Manager.ConfigurationElements.IEEE_Ethertype));
            databasesInfo.Add(Configuration.Manager.getDatabaseInfo(ref configuration, Configuration.Manager.ConfigurationElements.IEEE_Manufacturer));
            databasesInfo.Add(Configuration.Manager.getDatabaseInfo(ref configuration, Configuration.Manager.ConfigurationElements.IEEE_Operator));

            if (callback != null)
                callback(States.start, Count, databasesInfo.Count, ieeeRecordData.Count());

            foreach (Configuration.Manager.DatabaseInfo databaseInfo in databasesInfo)
            {
                if (callback != null)
                    callback(States.update, Count++ +1, databasesInfo.Count, ieeeRecordData.Count());
                PCLStorage.IFile file = null;
                try
                {
                    if (doReset)
                        throw new PCLStorage.Exceptions.FileNotFoundException("Resetting database");

                    file = await PCLStorage.FileSystem.Current.RoamingStorage.GetFileAsync(databaseInfo.fileName);

                    using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read))
                    {
                        Byte[] b = new Byte[stream.Length];

                        if (stream.Read(b, 0, b.Length) > 0)
                            ieeeRecordData.Add(b.GetString());
                    }
                }
                catch
                {

                    using (HttpClient client = new HttpClient())
                    {
                        String payLoad = await client.GetStringAsync(databaseInfo.Uri);
                        ieeeRecordData.Add(payLoad);

                        file = await PCLStorage.FileSystem.Current.RoamingStorage.CreateFileAsync(databaseInfo.fileName, PCLStorage.CreationCollisionOption.ReplaceExisting);
                        using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            Byte[] b = payLoad.GetBytes();
                            stream.Write(b, 0, b.Length);
                        }
                        Configuration.Manager.setTimeStamp(ref configuration);
                    }
                }
            }

            if (callback != null)
                callback(States.done, Count, databasesInfo.Count, ieeeRecordData.Count());

            return (ieeeRecordData);
        }

    }
}
