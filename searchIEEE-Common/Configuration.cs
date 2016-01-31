using System;

namespace searchIEEE.Configuration
{
    public class Configuration
    {
        public ConfigurationData configuration = null;

        public enum ConfigurationElements { IEEE_MAL, IEEE_MAM, IEEE_MAS, IEEE_IAB, IEEE_CID, IEEE_Ethertype, IEEE_Manufacturer, IEEE_Operator }

        private Func<ConfigurationData> readConfiguration;
        private Action<ConfigurationData> writeConfiguration;

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

        public DateTime getTimeStamp()
        {
            return (DateTime.Parse(configuration.TimeStamp));
        }

        public void setTimeStamp()
        {
            configuration.TimeStamp = DateTime.Now.ToString("r");
            this.writeConfiguration(configuration);
        }

        public Configuration(Func<ConfigurationData> readConfiguration, Action<ConfigurationData> writeConfiguration)
        {
            this.readConfiguration = readConfiguration;
            this.writeConfiguration = writeConfiguration;

            try
            {
                configuration = this.readConfiguration();
            }
            catch
            {
                configuration = new ConfigurationData();
                configuration.IEEE_MAL = "https://standards.ieee.org/develop/regauth/oui/oui.csv";
                configuration.IEEE_MAM = "https://standards.ieee.org/develop/regauth/oui28/mam.csv";
                configuration.IEEE_MAS = "https://standards.ieee.org/develop/regauth/oui36/oui36.csv";
                configuration.IEEE_IAB = "https://standards.ieee.org/develop/regauth/iab/iab.csv";
                configuration.IEEE_CID = "https://standards.ieee.org/develop/regauth/cid/cid.csv";
                configuration.IEEE_Ethertype = "https://standards.ieee.org/develop/regauth/ethertype/eth.csv";
                configuration.IEEE_Manufacturer = "https://standards.ieee.org/develop/regauth/manid/manid.csv";
                configuration.IEEE_Operator = "https://standards.ieee.org/develop/regauth/bopid/opid.csv";
                configuration.TimeStamp = DateTime.Now.ToString("r");
                this.writeConfiguration(configuration);
            }
        }

        public DatabaseInfo getConfig(ConfigurationElements e)
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

        public String getConfigValue(ConfigurationElements e)
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

        public void setConfigValue(ConfigurationElements e, String s)
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
            this.writeConfiguration(configuration);
        }
    }
}
