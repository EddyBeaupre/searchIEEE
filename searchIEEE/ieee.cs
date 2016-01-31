using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using searchIEEE.CustomExtensions;

namespace searchIEEE
{
    public partial class ieee : Form
    {
        searchIEEE.Records.IeeeRecordData ieeeCSV = null;
        public static Configuration.Configuration configSource = new Configuration.Configuration(readConfiguration, saveConfiguration);

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

        public ieee()
        {
            InitializeComponent();
            toolStripProgressBar1.Visible = false;
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

        private async void ieee_init(Boolean doReset)
        {
            int Count = 0;
            List<Configuration.Configuration.DatabaseInfo> databasesInfo = new List<Configuration.Configuration.DatabaseInfo>();
            
            if (this.ieeeCSV != null)
            {
                this.ieeeCSV.clearDataBases();
            }
            else
            {
                this.ieeeCSV = new searchIEEE.Records.IeeeRecordData();
            }
            
            databasesInfo.Add(configSource.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_MAL));
            databasesInfo.Add(configSource.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_MAM));
            databasesInfo.Add(configSource.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_MAS));
            databasesInfo.Add(configSource.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_IAB));
            databasesInfo.Add(configSource.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_CID));
            databasesInfo.Add(configSource.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_Ethertype));
            databasesInfo.Add(configSource.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_Manufacturer));
            databasesInfo.Add(configSource.getConfig(Configuration.Configuration.ConfigurationElements.IEEE_Operator));

            toolStripLabel1.Text = "Loading databases...";
            toolStripProgressBar1.Visible = true;
            searchBox1.Enabled = false;

            foreach (Configuration.Configuration.DatabaseInfo databaseInfo in databasesInfo)
            {
                toolStripLabel1.Text = String.Format("Loading databases {0} of {1}...", (Count++ + 1).ToString(), databasesInfo.Count.ToString());
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
                            ieeeCSV.Add(b.GetString());
                    }
                }
                catch
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    using (HttpClient client = new HttpClient())
                    {
                        String payLoad = await client.GetStringAsync(databaseInfo.Uri);
                        ieeeCSV.Add(payLoad);

                        file = await PCLStorage.FileSystem.Current.RoamingStorage.CreateFileAsync(databaseInfo.fileName, PCLStorage.CreationCollisionOption.ReplaceExisting);
                        using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            Byte[] b = payLoad.GetBytes();
                            stream.Write(b, 0, b.Length);
                        }
                        configSource.setTimeStamp();
                    }
                }
            }

            toolStripLabel1.Text = ieeeCSV.Count().ToString() + " entries avalables.";
            searchBox1.Enabled = true;
            toolStripProgressBar1.Visible = false;
        }

        private void ieee_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.searchBox1, "Enter search term, then press ENTER to search\nTo see the whole database just press ENTER without entering anything.");

            ieee_init(false);
        }

        private void searchBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toolStripLabel1.Text = "Searching...";
                toolStripProgressBar1.Visible = true;
                searchBox1.Enabled = false;
                threadLauncher(this.search, searchBox1.Text, loadAllCallback);
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void search(String Needle)
        {
            List<searchIEEE.Records.IeeeRecordDataItem> results = ieeeCSV.search(Needle);

            if (results != null)
            {
                ieeeResult ieeeResult = new ieeeResult(results);
                ieeeResult.ShowDialog();
                results = null;
            }
        }

        private void search(String Needle, ThreadWorkerCallback callBack)
        {
            List<searchIEEE.Records.IeeeRecordDataItem> results = ieeeCSV.search(Needle);

            if (results != null)
            {
                ieeeResult ieeeResult = new ieeeResult(results);
                callBack();
                ieeeResult.ShowDialog();
                results = null;
            }
        }

        private void ieeeConfigurationCallback()
        {
            if (this.InvokeRequired)
            {
                ieeeConfiguration.CallbackHandler d = new ieeeConfiguration.CallbackHandler(ieeeConfigurationCallback);
                this.Invoke(d, new object[] { });
            }
            else {
                ieee_init(true);
            }
        }

        private void configureButton_Click(object sender, EventArgs e)
        {
            ieeeConfiguration ieeeConfiguration = new ieeeConfiguration(ieeeConfigurationCallback);
            ieeeConfiguration.Show();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            ieee_init(true);
        }

        private delegate void ThreadWorkerCallback();


        private void threadLauncher(Action<String, ThreadWorkerCallback> function, String payLoad, ThreadWorkerCallback callback)
        {
            Thread thread = new Thread(() => threadWorker(function, payLoad, callback));
            thread.Name = "searchIEEE.threadWorker";
            thread.IsBackground = true;
            thread.Start();
        }

        private void threadLauncher(Action action, ThreadWorkerCallback callback)
        {
            Thread thread = new Thread(() => threadWorker(action, callback));
            thread.Name = "searchIEEE.threadWorker";
            thread.IsBackground = true;
            thread.Start();
        }

        private void threadWorker(Action action, ThreadWorkerCallback callback)
        {
            action();
            callback();
        }

        private void threadWorker(Action<String, ThreadWorkerCallback> action, String payLoad, ThreadWorkerCallback callback)
        {
            action(payLoad, callback);
        }


        private void searchCallback()
        {
            if (this.InvokeRequired)
            {
                ThreadWorkerCallback d = new ThreadWorkerCallback(loadAllCallback);
                this.Invoke(d, new object[] { });
            }
            else
            {
                toolStripLabel1.Text = ieeeCSV.Count().ToString() + " entries avalables.";
                searchBox1.Enabled = true;
                toolStripProgressBar1.Visible = false;
            }
        }

        private void loadAllCallback()
        {
            if (this.InvokeRequired)
            {
                ThreadWorkerCallback d = new ThreadWorkerCallback(loadAllCallback);
                this.Invoke(d, new object[] { });
            }
            else
            {
                toolStripLabel1.Text = ieeeCSV.Count().ToString() + " entries avalables.";
                searchBox1.Enabled = true;
                toolStripProgressBar1.Visible = false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ieeeAbout aboutDialog = new ieeeAbout();
            aboutDialog.Show();
        }
    }
}
