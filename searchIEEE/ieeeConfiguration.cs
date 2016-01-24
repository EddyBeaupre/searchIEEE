using System;
using System.Windows.Forms;

namespace searchIEEE
{
    public partial class ieeeConfiguration : Form
    {
        public delegate void CallbackHandler();
        private CallbackHandler callbackHandler = null;


        public ieeeConfiguration()
        {
            InitializeComponent();
        }

        public ieeeConfiguration(CallbackHandler callbackHandler)
        {
            InitializeComponent();
            this.callbackHandler = callbackHandler;
        }

        private void ieeeConfiguration_Load(object sender, EventArgs e)
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

        private void configDialog_FormClosing(object sender, FormClosingEventArgs e)
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
            if (callbackHandler != null)
            {
                callbackHandler();
            }
        }
    }
}
