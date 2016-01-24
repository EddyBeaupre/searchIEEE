using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace searchIEEE
{
    public partial class ieee : Form
    {
        ieeeCSV ieeeCSV = null;

        public ieee()
        {
            InitializeComponent();
            toolStripProgressBar1.Visible = false;
        }

        private void ieee_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.GetDirectoryName(Application.UserAppDataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Application.UserAppDataPath));
            }

            if (Properties.Settings.Default.IEEE_LocalDB == String.Empty)
            {
                Properties.Settings.Default.IEEE_LocalDB = Path.Combine(Path.GetDirectoryName(Application.UserAppDataPath), @"ieeeLocalDB.xml");
                Properties.Settings.Default.Save();
            }

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.searchBox1, "Enter search term, then press ENTER to search\nTo see the whole database just press ENTER without entering anything.");

            ieeeCSV = new ieeeCSV();
            ieeeCSV.Add(@"ieeeMAL.csv", Properties.Settings.Default.IEEE_MAL);
            ieeeCSV.Add(@"ieeeMAM.csv", Properties.Settings.Default.IEEE_MAM);
            ieeeCSV.Add(@"ieeeMAS.csv", Properties.Settings.Default.IEEE_MAS);
            ieeeCSV.Add(@"ieeeIAB.csv", Properties.Settings.Default.IEEE_IAB);
            ieeeCSV.Add(@"ieeeCID.csv", Properties.Settings.Default.IEEE_CID);
            ieeeCSV.Add(@"ieeeETH.csv", Properties.Settings.Default.IEEE_Ethertype);
            ieeeCSV.Add(@"ieeeMID.csv", Properties.Settings.Default.IEEE_Manufacturer);
            ieeeCSV.Add(@"ieeeOID.csv", Properties.Settings.Default.IEEE_Operator);
            toolStripLabel1.Text = "Loading databases...";
            toolStripProgressBar1.Visible = true;
            searchBox1.Enabled = false;
            threadLauncher(this.ieeeCSV.loadAll, loadAllCallback);
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
            List<IeeeRecord> results = ieeeCSV.search(Needle);

            if (results != null)
            {
                ieeeResult ieeeResult = new ieeeResult(results);
                ieeeResult.ShowDialog();
                results = null;
            }
        }

        private void search(String Needle, ThreadWorkerCallback callBack)
        {
            List<IeeeRecord> results = ieeeCSV.search(Needle);

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
                this.ieeeCSV.deleteAll();
                this.ieeeCSV.Dispose();
                this.ieeeCSV = new ieeeCSV();
                ieeeCSV.Add(@"ieeeMAL.csv", Properties.Settings.Default.IEEE_MAL);
                ieeeCSV.Add(@"ieeeMAM.csv", Properties.Settings.Default.IEEE_MAM);
                ieeeCSV.Add(@"ieeeMAS.csv", Properties.Settings.Default.IEEE_MAS);
                ieeeCSV.Add(@"ieeeIAB.csv", Properties.Settings.Default.IEEE_IAB);
                ieeeCSV.Add(@"ieeeCID.csv", Properties.Settings.Default.IEEE_CID);
                ieeeCSV.Add(@"ieeeETH.csv", Properties.Settings.Default.IEEE_Ethertype);
                ieeeCSV.Add(@"ieeeMID.csv", Properties.Settings.Default.IEEE_Manufacturer);
                ieeeCSV.Add(@"ieeeOID.csv", Properties.Settings.Default.IEEE_Operator);
                toolStripLabel1.Text = "Loading databases...";
                toolStripProgressBar1.Visible = true;
                searchBox1.Enabled = false;
                threadLauncher(this.ieeeCSV.loadAll, loadAllCallback);
            }
        }

        private void configureButton_Click(object sender, EventArgs e)
        {
            ieeeConfiguration ieeeConfiguration = new ieeeConfiguration(ieeeConfigurationCallback);
            ieeeConfiguration.Show();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            ieeeCSV.deleteAll();
            toolStripLabel1.Text = "Loading databases...";
            toolStripProgressBar1.Visible = true;
            searchBox1.Enabled = false;
            threadLauncher(this.ieeeCSV.loadAll, loadAllCallback);
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
    }
}
