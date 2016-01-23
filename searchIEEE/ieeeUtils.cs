using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
namespace searchIEEE
{
    public class ieeeDB
    {
        public DataTable dataTable;
        public delegate void CallbackEventHandler(Boolean? Status);
        private CallbackEventHandler callbackEventHandler;

        public void downloadDBCallback(Boolean? Status)
        {
            try
            {
                switch (Status)
                {
                    case true:
                        callbackEventHandler(true);
                        break;
                    case false:
                        callbackEventHandler(false);
                        break;
                    case null:
                        callbackEventHandler(null);
                        break;
                }
            }
            catch
            {
                if (callbackEventHandler != null)
                    callbackEventHandler(null);
            }
        }

        public ieeeDB(String[] urlArray, String dataTableXML, CallbackEventHandler callbackEventHandler)
        {
            try
            {
                this.dataTable = new DataTable();
                this.callbackEventHandler = callbackEventHandler;
                dataTable.Columns.AddRange(new DataColumn[5] { new DataColumn("Id", typeof(System.UInt32)), new DataColumn("Assignment", typeof(System.UInt64)), new DataColumn("Registry"), new DataColumn("OrgName"), new DataColumn("OrgAddress") });
                dataTable.TableName = "ieeeDatabaseXML";

                downloadDB dDB = new downloadDB(urlArray, dataTableXML, dataTable, downloadDBCallback);
            }
            catch
            {
                if (callbackEventHandler != null)
                    callbackEventHandler(null);
            }
        }

        public DataRow[] searchByID(UInt64 needle)
        {
            try
            {
                UInt64[] maskArray = { 0xFFFFFFFFF000, 0xFFFFFFFF0000, 0xFFFFFFF00000, 0xFFFFFF000000, 0xFFFFF0000000, 0xFFFF00000000, 0xFFF000000000, 0xFF0000000000 };

                foreach (UInt64 mask in maskArray)
                {
                    String request = "Assignment = '" + (needle & mask).ToString() + "'";
                    DataRow[] dataRow = dataTable.Select(request);
                    if (dataRow.Count() > 0)
                    {
                        return (dataRow);
                    }
                }
                return (null);
            }
            catch
            {
                return (null);
            }
        }

        public DataRow[] searchByText(String needle)
        {
            try
            {
                String request = "OrgName LIKE '%" + needle + "%' or OrgAddress  LIKE '%" + needle + "%'";
                DataRow[] dataRow = dataTable.Select(request);
                if (dataRow.Count() > 0)
                {
                    return (dataRow);
                }

                return (null);
            }
            catch
            {
                return (null);
            }
        }

        public DataRow[] search(String needle)
        {
            try
            {
                if (needle != String.Empty)
                {
                    DataRow[] dataRows = searchByID(formatOID.toUInt64(needle));

                    if (dataRows == null)
                    {
                        return (searchByText(needle));
                    }
                    else
                    {
                        return (dataRows);
                    }
                }
                return (null);
            }
            catch
            {
                return (null);
            }
        }
    }

    public class formatOID : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            try
            {
                if (formatType == typeof(ICustomFormatter))
                    return (this);
                else
                    return (null);
            }
            catch
            {
                return (null);
            }
        }


        private static String padOID(String OID)
        {
            try
            {
                for (Int32 i = OID.Count(); i < 12; i++)
                {
                    OID += "0";
                }

                return (OID);
            }
            catch
            {
                return (null);
            }
        }

        public static string toString(UInt64 OID)
        {
            try
            {
                int Count = 0;
                string output = string.Empty;

                foreach (Char c in padOID(OID.ToString("X12")))
                {
                    if (((Count % 2) == 0) && (Count > 0))
                    {
                        output += ":";
                    }
                    output += c;
                    Count++;
                }
                return (output);
            }
            catch
            {
                return (null);
            }
        }

        public static UInt64 toUInt64(String OID)
        {
            try
            {
                return (Convert.ToUInt64("0x" + padOID(cleanOID(OID)), 16));
            }
            catch
            {
                return (UInt64.MaxValue);
            }
        }

        public static String cleanOID(String OID)
        {
            Regex rgx = new Regex("[^a-fA-F0-9]");
            return (padOID(rgx.Replace(OID, "").ToUpper()));
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            try
            {
                if (!formatProvider.Equals(this)) return (null);

                if (!format.StartsWith("MAC")) return (null);

                if (arg is UInt64)
                {
                    return (toString((UInt64)arg));
                }

                else
                {
                    return (null);
                }

            }
            catch
            {
                return (null);
            }
        }
    }

    public class downloadDB
    {
        public delegate void CallbackEventHandler(Boolean? Status);

        public downloadDB(String[] urlArray, String dataTableXML, DataTable dataTable, CallbackEventHandler Callback)
        {
            try
            {
                Thread oThread = new Thread(() => threadWorker(urlArray, dataTableXML, dataTable, Callback));
                oThread.Name = "searchOUI.stringDownloader";
                oThread.IsBackground = true;
                oThread.Start();
            }
            catch
            {
                Callback(null);
            }
        }

        private Boolean? readDataTable(String dataTableXML, DataTable dataTable)
        {
            try
            {
                if (File.Exists(dataTableXML))
                {
                    dataTable.ReadXml(dataTableXML);
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            catch
            {
                return (null);
            }
        }

        private Boolean? downloadDataTable(String[] urlArray, String dataTableXML, DataTable dataTable)
        {
            try
            {
                String payloadData = String.Empty;

                foreach (String Url in urlArray)
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.UseDefaultCredentials = true;
                        payloadData += wc.DownloadString(new Uri(Url));
                    }
                }

                String[] payloadArray = splitLines(payloadData);
                UInt32 total = (UInt32)payloadArray.Count();
                UInt32 current = 0;
                UInt64 OID = UInt64.MaxValue;

                foreach (String item in payloadArray)
                {
                    String[] cleanItem = cleanString(item).Split('>');

                    if (cleanItem[0] != "Registry")
                    {
                        switch (cleanItem.Count())
                        {
                            case 3:
                                OID = convertOID(cleanItem[1]);
                                if (OID != UInt64.MaxValue)
                                    dataTable.Rows.Add(current++, OID, cleanItem[0].Trim(), cleanItem[2].Trim(), String.Empty);
                                break;
                            case 4:
                            case 5:
                                OID = convertOID(cleanItem[1]);
                                if (OID != UInt64.MaxValue)
                                    dataTable.Rows.Add(current++, OID, cleanItem[0].Trim(), cleanItem[2].Trim(), cleanItem[3].Trim());
                                break;
                            default:
                                break;
                        }
                    }
                }
                dataTable.WriteXml(dataTableXML);
                return (true);
            }
            catch
            {
                return (null);
            }
        }

        public void threadWorker(String[] urlArray, String dataTableXML, DataTable dataTable, CallbackEventHandler Callback)
        {
            try
            {
                Boolean? Status = null;
                dataTable.BeginLoadData();

                switch (readDataTable(dataTableXML, dataTable))
                {
                    case true:
                        Status = true;
                        break;
                    case false:
                    case null:
                        Status = downloadDataTable(urlArray, dataTableXML, dataTable);
                        break;
                }
                dataTable.EndLoadData();
                Callback(Status);
            }
            catch
            {
                Callback(null);
            }
        }

        private static String cleanString(String Data)
        {
            String cleanString = String.Empty;
            try
            {

                Boolean inQuote = false;

                foreach (Char c in Data)
                {
                    if (c == '"')
                    {
                        if (inQuote)
                        {
                            inQuote = false;
                            continue;
                        }
                        else
                        {
                            inQuote = true;
                            continue;
                        }
                    }

                    if (!inQuote)
                    {
                        switch (c)
                        {
                            case ',':
                                cleanString += '>';
                                break;
                            case '\r':
                            case '\n':
                                break;
                            default:
                                cleanString += c;
                                break;
                        }
                    }
                    else
                    {
                        cleanString += c;
                    }
                }

                return (cleanString);
            }
            catch (Exception)
            {
                return (cleanString);
            }
        }

        private String[] splitLines(String Data)
        {
            try
            {
                Data = Data.Replace("\r\n", "\n");
                Data = Data.Replace("\n\r", "\n");
                Data = Data.Replace("\r", "\n");
                return (Data.Split(new string[] { "\n" }, StringSplitOptions.None));
            }
            catch
            {
                return (String.Empty.Split());
            }
        }

        private UInt64 convertOID(String OID)
        {
            try
            {
                OID = OID.Trim();

                for (Int32 i = OID.Count(); i < 12; i++)
                {
                    OID += "0";
                }

                return (Convert.ToUInt64("0x" + OID, 16));
            }
            catch
            {
                return (UInt64.MaxValue);
            }
        }
    }
}
