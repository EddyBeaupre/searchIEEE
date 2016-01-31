using System;
using System.Windows.Forms;
using searchIEEE.Configuration;

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
            ieeeMAL.Text = ieee.configSource.getConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_MAL);
            ieeeMAM.Text = ieee.configSource.getConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_MAM);
            ieeeMAS.Text = ieee.configSource.getConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_MAS);
            ieeeIAB.Text = ieee.configSource.getConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_IAB);
            ieeeCID.Text = ieee.configSource.getConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_CID);
            ieeeEth.Text = ieee.configSource.getConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_Ethertype);
            ieeeMID.Text = ieee.configSource.getConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_Manufacturer);
            ieeeOID.Text = ieee.configSource.getConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_Operator);
        }

        private void configDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            ieee.configSource.setConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_MAL,ieeeMAL.Text);
            ieee.configSource.setConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_MAM, ieeeMAM.Text);
            ieee.configSource.setConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_MAS, ieeeMAS.Text);
            ieee.configSource.setConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_IAB, ieeeIAB.Text);
            ieee.configSource.setConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_CID, ieeeCID.Text);
            ieee.configSource.setConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_Ethertype, ieeeEth.Text);
            ieee.configSource.setConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_Manufacturer, ieeeMID.Text);
            ieee.configSource.setConfigValue(Configuration.Configuration.ConfigurationElements.IEEE_Operator, ieeeOID.Text);
            if (callbackHandler != null)
            {
                callbackHandler();
            }
        }
    }
}
