using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace searchIEEE
{
    public partial class ieee : Form
    {
        ieeeDB ieeeDB = null;

        public ieee()
        {
            InitializeComponent();
            toolStripProgressBar1.Visible = false;
        }

        public void ieeeDBCallback(Boolean? Status)
        {
            if (this.InvokeRequired)
            {
                downloadDB.CallbackEventHandler d = new downloadDB.CallbackEventHandler(ieeeDBCallback);
                this.Invoke(d, new object[] { Status });
            }
            else {
                switch(Status)
                {
                    case true:
                        searchBox1.Enabled = true;
                        toolStripLabel1.Text = ieeeDB.dataTable.Rows.Count.ToString() + " entries avalables.";
                        toolStripProgressBar1.Visible = false;
                        break;
                    case false:
                    case null:
                        ieeeDB = null;
                        toolStripLabel1.Text = "Error while downloading databases.";
                        break;
                }
            }
        }

        private void ieee_Load(object sender, EventArgs e)
        {
            toolStripLabel1.Text = "Loading databases...";
            toolStripProgressBar1.Visible = true;
            searchBox1.Enabled = false;
            if (!Directory.Exists(Path.GetDirectoryName(Application.UserAppDataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Application.UserAppDataPath));
            }

            if (Properties.Settings.Default.IEEE_LocalDB == String.Empty)
            {
                Properties.Settings.Default.IEEE_LocalDB = Path.Combine(Path.GetDirectoryName(Application.UserAppDataPath), @"ieeeLocalDB.xml");
                Properties.Settings.Default.Save();
            }

            ieeeDB = new ieeeDB(new String[] { Properties.Settings.Default.IEEE_MAL, Properties.Settings.Default.IEEE_MAM, Properties.Settings.Default.IEEE_MAS, Properties.Settings.Default.IEEE_IAB, Properties.Settings.Default.IEEE_CID, Properties.Settings.Default.IEEE_Ethertype, Properties.Settings.Default.IEEE_Manufacturer, Properties.Settings.Default.IEEE_Operator }, Properties.Settings.Default.IEEE_LocalDB, ieeeDBCallback);

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.searchBox1, "Enter search term, then press ENTER to search\nTo see the whole database just press ENTER without entering anything.");
    }

        private void searchBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if(searchBox1.Text != String.Empty) { 
                DataRow[] dataRow = ieeeDB.search(searchBox1.Text);

                if (dataRow != null)
                {
                        ieeeResult ieeeResult = new ieeeResult(dataRow);
                    ieeeResult.Show();
                }
                } else
                {
                    ieeeResult ieeeResult = new ieeeResult(ieeeDB.dataTable);
                    ieeeResult.Show();
                }

                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void configureButton_Click(object sender, EventArgs e)
        {
            ieeeConfiguration ieeeConfiguration = new ieeeConfiguration();
            ieeeConfiguration.Show();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            File.Delete(Properties.Settings.Default.IEEE_LocalDB);
            ieeeDB = null;
            toolStripLabel1.Text = "Loading databases...";
            toolStripProgressBar1.Visible = true;
            searchBox1.Enabled = false;
            ieeeDB = new ieeeDB(new String[] { Properties.Settings.Default.IEEE_MAL, Properties.Settings.Default.IEEE_MAM, Properties.Settings.Default.IEEE_MAS, Properties.Settings.Default.IEEE_IAB, Properties.Settings.Default.IEEE_CID, Properties.Settings.Default.IEEE_Ethertype, Properties.Settings.Default.IEEE_Manufacturer, Properties.Settings.Default.IEEE_Operator }, Properties.Settings.Default.IEEE_LocalDB, ieeeDBCallback);
        }
    }
}
