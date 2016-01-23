using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace searchIEEE
{
    public partial class ieeeConfiguration : Form
    {
        public ieeeConfiguration()
        {
            try
            {
                InitializeComponent();
            }
            catch
            {
            }
        }

        private void ieeeConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                ieeeMAL.Text = Properties.Settings.Default.IEEE_MAL;
                ieeeMAM.Text = Properties.Settings.Default.IEEE_MAM;
                ieeeMAS.Text = Properties.Settings.Default.IEEE_MAS;
                ieeeIAB.Text = Properties.Settings.Default.IEEE_IAB;
                ieeeCID.Text = Properties.Settings.Default.IEEE_CID;
                ieeeEth.Text = Properties.Settings.Default.IEEE_Ethertype;
                ieeeMID.Text = Properties.Settings.Default.IEEE_Manufacturer;
                ieeeOID.Text = Properties.Settings.Default.IEEE_Operator;
            }
            catch
            {
            }
        }

        private void configDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Properties.Settings.Default.IEEE_MAL = ieeeMAL.Text;
                Properties.Settings.Default.IEEE_MAM = ieeeMAM.Text;
                Properties.Settings.Default.IEEE_MAS = ieeeMAS.Text;
                Properties.Settings.Default.IEEE_IAB = ieeeIAB.Text;
                Properties.Settings.Default.IEEE_CID = ieeeCID.Text;
                Properties.Settings.Default.IEEE_Ethertype = ieeeEth.Text;
                Properties.Settings.Default.IEEE_Manufacturer = ieeeMID.Text;
                Properties.Settings.Default.IEEE_Operator = ieeeOID.Text;
                Properties.Settings.Default.Save();
            }
            catch
            {
            }
        }
    }
}
