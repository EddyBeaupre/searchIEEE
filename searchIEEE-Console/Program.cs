using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using searchIEEE.CustomExtensions;
using System.Net;
using System.Net.Http;
using System.Configuration;

namespace searchIEEE
{
    class Program
    {
        static searchIEEE.Records.IeeeRecordData ieeeCSV = null;

        public struct DatabaseInfo
        {
            public String fileName;
            public Uri Uri;

            public DatabaseInfo(String f, String u)
            {
                fileName = f;
                Uri = new Uri(u);
            }

            public DatabaseInfo(String f, Uri u)
            {
                fileName = f;
                Uri = u;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.Search != null)
                {
                    search(options.Search);
                }
                else if (options.Show == true)
                {
                    search(String.Empty);
                }
                else
                    Console.WriteLine(options.GetUsage());
            }
            Environment.Exit(0);
        }

        private static void search(String Needle)
        {
            Int64 Count = 0;


            ieee_init(false);

            List<searchIEEE.Records.IeeeRecordDataItem> results = Program.ieeeCSV.search(Needle);

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            if (results != null)
            {
                foreach (searchIEEE.Records.IeeeRecordDataItem record in results)
                {
                    Console.WriteLine(Count.ToString() + "," + record.Assignment.Trim() + "," + record.Registry.Trim() + ",\"" + record.OrganizationName.Trim() + "\",\"" + record.OrganizationAddress.Trim() + "\"");
                    Count++;
                }
            }
        }

        public static Configuration.ConfigurationData readConfiguration()
        {
            Configuration.ConfigurationData configuration = new Configuration.ConfigurationData();
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Configuration.ConfigurationData));
                PCLStorage.IFile file = PCLStorage.FileSystem.Current.RoamingStorage.GetFileAsync(@"searchIEEE.XML").Result;
                using (Stream stream = file.OpenAsync(PCLStorage.FileAccess.Read).Result)
                {
                    configuration = (Configuration.ConfigurationData)reader.Deserialize(stream);
                }
                return (configuration);
            }
            catch
            {
                throw new PCLStorage.Exceptions.FileNotFoundException("Not Found");
            }
    }

        public static void saveConfiguration(Configuration.ConfigurationData configuration)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration.ConfigurationData));
            
            PCLStorage.IFile file = PCLStorage.FileSystem.Current.RoamingStorage.CreateFileAsync(@"searchIEEE.XML", PCLStorage.CreationCollisionOption.ReplaceExisting).Result;
            using (Stream stream = file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).Result)
            {
                writer.Serialize(stream, configuration);
            }
        }

        private static void ieee_init(Boolean doReset)
        {
            List<Configuration.Configuration.DatabaseInfo> databasesInfo = new List<Configuration.Configuration.DatabaseInfo>();
            Configuration.Configuration sources = new Configuration.Configuration(readConfiguration, saveConfiguration);

            if (Program.ieeeCSV != null)
            {
                Program.ieeeCSV.Dispose();
                Program.ieeeCSV = null;
            }

            Program.ieeeCSV = new searchIEEE.Records.IeeeRecordData();
            
            databasesInfo.Add(sources.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_MAL));
            databasesInfo.Add(sources.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_MAM));
            databasesInfo.Add(sources.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_MAS));
            databasesInfo.Add(sources.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_IAB));
            databasesInfo.Add(sources.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_CID));
            databasesInfo.Add(sources.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_Ethertype));
            databasesInfo.Add(sources.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_Manufacturer));
            databasesInfo.Add(sources.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_Operator));

            foreach (Configuration.Configuration.DatabaseInfo databaseInfo in databasesInfo)
            {
                PCLStorage.IFile file = null;
                try
                {
                    if (doReset)
                        throw new PCLStorage.Exceptions.FileNotFoundException("Resetting database");

                    file = PCLStorage.FileSystem.Current.RoamingStorage.GetFileAsync(databaseInfo.fileName).Result;

                    using (Stream stream = file.OpenAsync(PCLStorage.FileAccess.Read).Result)
                    {
                        Byte[] b = new Byte[stream.Length];

                        if (stream.Read(b, 0, b.Length) > 0)
                            ieeeCSV.Add(b.GetString());
                    }
                }
                catch
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    using (HttpClient client = new HttpClient())
                    {
                        try { 
                        String payLoad = client.GetStringAsync(databaseInfo.Uri).Result;
                        ieeeCSV.Add(payLoad);

                        file = PCLStorage.FileSystem.Current.RoamingStorage.CreateFileAsync(databaseInfo.fileName, PCLStorage.CreationCollisionOption.ReplaceExisting).Result;
                        using (Stream stream = file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).Result)
                        {
                            Byte[] b = payLoad.GetBytes();
                            stream.Write(b, 0, b.Length);
                        }
                            sources.setTimeStamp();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
            }
        }
    }

    class Options
    {
        [Option('s', "search", HelpText = "Search term.", MutuallyExclusiveSet = "Search")]
        public string Search { get; set; }

        [Option('a', "all", HelpText = "Show all entries.", MutuallyExclusiveSet = "Search")]
        public bool Show { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            String name = fvi.ProductName; ;
            String author = fvi.CompanyName;
            String version = fvi.FileVersion;
            String Copyright = fvi.LegalCopyright;
            String AppName = Path.GetFileName(fvi.FileName);

            var help = new HelpText
            {
                Heading = new HeadingInfo(name, fvi.FileVersion),
                Copyright = Copyright,
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: " + AppName + " [-s term] [-a]");
            help.AddOptions(this);
            return help;
        }
    }
}
