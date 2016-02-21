using System;
using System.IO;
using System.Threading.Tasks;

namespace searchIEEE.Configuration
{
    public class Data
    {
        public String IEEE_MAL;
        public String IEEE_MAM;
        public String IEEE_MAS;
        public String IEEE_IAB;
        public String IEEE_CID;
        public String IEEE_Ethertype;
        public String IEEE_Manufacturer;
        public String IEEE_Operator;
        public String TimeStamp;
    }

    public class Manager
    {
        public enum ConfigurationElements { IEEE_MAL, IEEE_MAM, IEEE_MAS, IEEE_IAB, IEEE_CID, IEEE_Ethertype, IEEE_Manufacturer, IEEE_Operator }

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

        public static DateTime getTimeStamp(ref Data configuration)
        {
            return (DateTime.Parse(configuration.TimeStamp));
        }

        public static void setTimeStamp(ref Data configuration)
        {
            configuration.TimeStamp = DateTime.Now.ToString("r");
            saveConfiguration(ref configuration);
        }

        public static Task<Data> loadConfigurationAsync()
        {
            return (Task.Run<Data>(() => readConfiguration()));
        }

        /*
        public static void loadConfiguration(ref Data configuration)
        {
            try
            {
                configuration = readConfiguration(ref configuration);
            }
            catch
            {
                configuration = defaultConfiguration(ref configuration);
            }
        }
        */

        public static Data defaultConfiguration()
        {
            Data configuration = new Data();

            configuration.IEEE_MAL = "http://standards.ieee.org/develop/regauth/oui/oui.csv";
            configuration.IEEE_MAM = "http://standards.ieee.org/develop/regauth/oui28/mam.csv";
            configuration.IEEE_MAS = "http://standards.ieee.org/develop/regauth/oui36/oui36.csv";
            configuration.IEEE_IAB = "http://standards.ieee.org/develop/regauth/iab/iab.csv";
            configuration.IEEE_CID = "http://standards.ieee.org/develop/regauth/cid/cid.csv";
            configuration.IEEE_Ethertype = "http://standards.ieee.org/develop/regauth/ethertype/eth.csv";
            configuration.IEEE_Manufacturer = "http://standards.ieee.org/develop/regauth/manid/manid.csv";
            configuration.IEEE_Operator = "http://standards.ieee.org/develop/regauth/bopid/opid.csv";
            configuration.TimeStamp = DateTime.Now.ToString("r");
            saveConfiguration(ref configuration);
            return (configuration);
        }

        /*
        public static Data defaultConfiguration(ref Data configuration)
        {
            if (configuration == null)
            {
                configuration = new Data();
            }

            configuration.IEEE_MAL = "http://standards.ieee.org/develop/regauth/oui/oui.csv";
            configuration.IEEE_MAM = "http://standards.ieee.org/develop/regauth/oui28/mam.csv";
            configuration.IEEE_MAS = "http://standards.ieee.org/develop/regauth/oui36/oui36.csv";
            configuration.IEEE_IAB = "http://standards.ieee.org/develop/regauth/iab/iab.csv";
            configuration.IEEE_CID = "http://standards.ieee.org/develop/regauth/cid/cid.csv";
            configuration.IEEE_Ethertype = "http://standards.ieee.org/develop/regauth/ethertype/eth.csv";
            configuration.IEEE_Manufacturer = "http://standards.ieee.org/develop/regauth/manid/manid.csv";
            configuration.IEEE_Operator = "http://standards.ieee.org/develop/regauth/bopid/opid.csv";
            configuration.TimeStamp = DateTime.Now.ToString("r");
            saveConfiguration(ref configuration);
            return (configuration);
        }
        */

        public static Data readConfiguration()
        {
            try
            {
                Data configuration = new Data();

                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Data));
                PCLStorage.IFile file = PCLStorage.FileSystem.Current.RoamingStorage.GetFileAsync(@"searchIEEE.XML").Result;
                using (Stream stream = file.OpenAsync(PCLStorage.FileAccess.Read).Result)
                {
                    configuration = (Data)reader.Deserialize(stream);
                }
                return (configuration);
            }
            catch
            {
                return(defaultConfiguration());
            }
        }

        /*
        public static Data readConfiguration(ref Data configuration)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Data));
                PCLStorage.IFile file = PCLStorage.FileSystem.Current.RoamingStorage.GetFileAsync(@"searchIEEE.XML").Result;
                using (Stream stream = file.OpenAsync(PCLStorage.FileAccess.Read).Result)
                {
                    configuration = (Data)reader.Deserialize(stream);
                }
                return (configuration);
            }
            catch
            {
                throw new PCLStorage.Exceptions.FileNotFoundException("Not Found");
            }
        }
        */

        public static void saveConfiguration(ref Data configuration)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Data));

            PCLStorage.IFile file = PCLStorage.FileSystem.Current.RoamingStorage.CreateFileAsync(@"searchIEEE.XML", PCLStorage.CreationCollisionOption.ReplaceExisting).Result;
            using (Stream stream = file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).Result)
            {
                writer.Serialize(stream, configuration);
            }
        }

        public static DatabaseInfo getDatabaseInfo(ref Data configuration, ConfigurationElements e)
        {
            switch (e)
            {
                case ConfigurationElements.IEEE_MAL:
                    return (new DatabaseInfo(@"ieeeMAL.csv", configuration.IEEE_MAL));
                case ConfigurationElements.IEEE_MAM:
                    return (new DatabaseInfo(@"ieeeMAM.csv", configuration.IEEE_MAM));
                case ConfigurationElements.IEEE_MAS:
                    return (new DatabaseInfo(@"ieeeMAS.csv", configuration.IEEE_MAS));
                case ConfigurationElements.IEEE_IAB:
                    return (new DatabaseInfo(@"ieeeIAB.csv", configuration.IEEE_IAB));
                case ConfigurationElements.IEEE_CID:
                    return (new DatabaseInfo(@"ieeeCID.csv", configuration.IEEE_CID));
                case ConfigurationElements.IEEE_Ethertype:
                    return (new DatabaseInfo(@"ieeeETH.csv", configuration.IEEE_Ethertype));
                case ConfigurationElements.IEEE_Manufacturer:
                    return (new DatabaseInfo(@"ieeeMID.csv", configuration.IEEE_Manufacturer));
                case ConfigurationElements.IEEE_Operator:
                    return (new DatabaseInfo(@"ieeeCID.csv", configuration.IEEE_Operator));
            }
            return (new DatabaseInfo(String.Empty, String.Empty));
        }

        public static String getConfigurationElements(ref Data configuration, ConfigurationElements e)
        {
            switch (e)
            {
                case ConfigurationElements.IEEE_MAL:
                    return configuration.IEEE_MAL;
                case ConfigurationElements.IEEE_MAM:
                    return configuration.IEEE_MAM;
                case ConfigurationElements.IEEE_MAS:
                    return configuration.IEEE_MAS;
                case ConfigurationElements.IEEE_IAB:
                    return configuration.IEEE_IAB;
                case ConfigurationElements.IEEE_CID:
                    return configuration.IEEE_CID;
                case ConfigurationElements.IEEE_Ethertype:
                    return configuration.IEEE_Ethertype;
                case ConfigurationElements.IEEE_Manufacturer:
                    return configuration.IEEE_Manufacturer;
                case ConfigurationElements.IEEE_Operator:
                    return configuration.IEEE_Operator;
            }
            return (String.Empty);
        }

        public static void setConfigurationElements(ref Data configuration, ConfigurationElements e, String s)
        {
            switch (e)
            {
                case ConfigurationElements.IEEE_MAL:
                    configuration.IEEE_MAL = s;
                    break;
                case ConfigurationElements.IEEE_MAM:
                    configuration.IEEE_MAM = s;
                    break;
                case ConfigurationElements.IEEE_MAS:
                    configuration.IEEE_MAS = s;
                    break;
                case ConfigurationElements.IEEE_IAB:
                    configuration.IEEE_IAB = s;
                    break;
                case ConfigurationElements.IEEE_CID:
                    configuration.IEEE_CID = s;
                    break;
                case ConfigurationElements.IEEE_Ethertype:
                    configuration.IEEE_Ethertype = s;
                    break;
                case ConfigurationElements.IEEE_Manufacturer:
                    configuration.IEEE_Manufacturer = s;
                    break;
                case ConfigurationElements.IEEE_Operator:
                    configuration.IEEE_Operator = s;
                    break;
            }
            saveConfiguration(ref configuration);
        }
    }
}
