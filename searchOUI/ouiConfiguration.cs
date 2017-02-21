using System;
using System.Windows.Forms;
using searchOUIDB.Configuration;

namespace searchOUIDB
{
    public partial class ConfigurationDialog : Form
    {
        public delegate void Callback();
        private Callback callbackHandler = null;
        private Data configuration = null;

        public ConfigurationDialog(Callback callbackHandler, ref Data configuration)
        {
            InitializeComponent();
            this.callbackHandler = callbackHandler;
            this.configuration = configuration;
        }

        private void ConfigurationDialog_Load(object sender, EventArgs e)
        {
            ieeeMAL.Text = Manager.getConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_MAL);
            ieeeMAM.Text = Manager.getConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_MAM);
            ieeeMAS.Text = Manager.getConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_MAS);
            ieeeIAB.Text = Manager.getConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_IAB);
            ieeeCID.Text = Manager.getConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_CID);
            ieeeEth.Text = Manager.getConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_Ethertype);
            ieeeMID.Text = Manager.getConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_Manufacturer);
            ieeeOID.Text = Manager.getConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_Operator);
        }

        private void ConfigurationDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Manager.setConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_MAL,ieeeMAL.Text);
            Manager.setConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_MAM, ieeeMAM.Text);
            Manager.setConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_MAS, ieeeMAS.Text);
            Manager.setConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_IAB, ieeeIAB.Text);
            Manager.setConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_CID, ieeeCID.Text);
            Manager.setConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_Ethertype, ieeeEth.Text);
            Manager.setConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_Manufacturer, ieeeMID.Text);
            Manager.setConfigurationElements(ref configuration, Manager.ConfigurationElements.IEEE_Operator, ieeeOID.Text);
            if (callbackHandler != null)
            {
                callbackHandler();
            }
        }
    }
}
