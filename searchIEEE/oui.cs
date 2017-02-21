using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace searchOUIDB
{
    public partial class ieee : Form
    {
        private Records.Data recordsData = null;
        private Configuration.Data configurationData = null;
        private delegate void SearchCallback();

        public ieee()
        {
            InitializeComponent();
            toolStripProgressBar1.Visible = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        private async void ieee_Load(object sender, EventArgs e)
        {
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(this.searchBox, "Enter search term, then press ENTER to search\nTo see the whole database just press ENTER without entering anything.");
            configurationData = await Configuration.Manager.loadConfigurationAsync();

            this.recordsData = await Records.Loader.loadAsync(configurationData, false, recordLoaderCallback);
        }

        private void recordLoaderCallback(Records.Loader.States state, Int32 Count, Int32 Total, UInt64 RecordCounts)
        {
            if (this.InvokeRequired)
            {
                Records.Loader.Callback d = new Records.Loader.Callback(recordLoaderCallback);
                this.Invoke(d, new object[] { state, Count, Total, RecordCounts });
            }
            else {
                switch (state)
                {
                    case Records.Loader.States.start:
                        toolStripLabel1.Text = String.Format("{0} of {1} databases loaded.", Count, Total, RecordCounts);
                        toolStripProgressBar1.Visible = true;
                        searchBox.Enabled = false;
                        break;
                    case Records.Loader.States.update:
                        toolStripLabel1.Text = String.Format("{0} of {1} databases loaded, {2} records availables.", Count, Total, RecordCounts);
                        break;
                    case Records.Loader.States.done:
                        toolStripLabel1.Text = String.Format("{2} records availables in {1} databases.", Count, Total, RecordCounts);
                        searchBox.Enabled = true;
                        toolStripProgressBar1.Visible = false;
                        break;
                }
            }
        }

        private void configureButtonClick(object sender, EventArgs e)
        {
            ConfigurationDialog configurationDialog = new ConfigurationDialog(ConfigurationDialogCallback, ref configurationData);
            configurationDialog.Show();
        }

        private async void ConfigurationDialogCallback()
        {
            if (this.InvokeRequired)
            {
                ConfigurationDialog.Callback d = new ConfigurationDialog.Callback(ConfigurationDialogCallback);
                this.Invoke(d, new object[] { });
            }
            else {
                recordsData = null;

                this.recordsData = await Records.Loader.loadAsync(configurationData, true, recordLoaderCallback);
            }
        }

        private async void resetButtonClick(object sender, EventArgs e)
        {
            recordsData = null;

            this.recordsData = await Records.Loader.loadAsync(configurationData, true, recordLoaderCallback);
        }

        private void aboutButtonClick(object sender, EventArgs e)
        {
            ieeeAbout aboutDialog = new ieeeAbout();
            aboutDialog.Show();
        }

        private void searchBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                search(this.searchBox.Text);
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void search(String Needle)
        {
            toolStripLabel1.Text = "Searching...";
            toolStripProgressBar1.Visible = true;
            searchBox.Enabled = false;

            Thread thread = new Thread(() => searchWorker(Needle));
            thread.Name = "searchWorker";
            thread.IsBackground = true;
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
        }

        private void searchWorker(String Needle)
        {
            List<Records.Items> results = recordsData.search(Needle);

            if (results != null)
            {
                    ieeeResult ieeeResult = new ieeeResult(results);
                    searchCallback();
                    ieeeResult.ShowDialog();
                    results = null;
            }
        }

        private void searchCallback()
        {
            if (this.InvokeRequired)
            {
                SearchCallback d = new SearchCallback(searchCallback);
                this.Invoke(d, new object[] { });
            }
            else
            {
                toolStripLabel1.Text = recordsData.Count().ToString() + " entries avalables.";
                searchBox.Enabled = true;
                toolStripProgressBar1.Visible = false;
            }
        }
    }
}
