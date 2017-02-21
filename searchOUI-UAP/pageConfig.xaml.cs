using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace searchOUIDB
{
    public class ConfigParameters
    {
        public delegate void ReloadCallback(Boolean Reload, Boolean Close, Configuration.Data ConfigurationData);
        public ReloadCallback reloadCallback;
        public Configuration.Data configurationData;

        public ConfigParameters(ReloadCallback ReloadCallback, Configuration.Data ConfigurationData)
        {
            this.reloadCallback = ReloadCallback;
            this.configurationData = ConfigurationData;
        }
    }

    public sealed partial class pageConfig : Page
    {
        private ConfigParameters configParameters;

        public pageConfig()
        {
            this.InitializeComponent();
        }

        private void updateDialog()
        {
            this.TextBox_IEEE_MAL.Text = Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAL);
            this.TextBox_IEEE_MAM.Text = Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAM);
            this.TextBox_IEEE_MAS.Text = Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAS);
            this.TextBox_IEEE_IAB.Text = Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_IAB);
            this.TextBox_IEEE_CID.Text = Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_CID);
            this.TextBox_IEEE_Ethertype.Text = Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Ethertype);
            this.TextBox_IEEE_Manufacturer.Text = Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Manufacturer);
            this.TextBox_IEEE_Operator.Text = Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Operator);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ConfigParameters)
            {
                configParameters = (ConfigParameters)e.Parameter;
                updateDialog();
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Boolean ConfigUpdated = false;

            if (Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAL) != this.TextBox_IEEE_MAL.Text)
            {
                Configuration.Manager.setConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAL, this.TextBox_IEEE_MAL.Text);
                ConfigUpdated = true;
            }

            if (Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAM) != this.TextBox_IEEE_MAM.Text)
            {
                Configuration.Manager.setConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAM, this.TextBox_IEEE_MAM.Text);
                ConfigUpdated = true;
            }

            if (Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAS) != this.TextBox_IEEE_MAS.Text)
            {
                Configuration.Manager.setConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_MAS, this.TextBox_IEEE_MAS.Text);
                ConfigUpdated = true;
            }

            if (Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_IAB) != this.TextBox_IEEE_IAB.Text)
            {
                Configuration.Manager.setConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_IAB, this.TextBox_IEEE_IAB.Text);
                ConfigUpdated = true;
            }

            if (Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_CID) != this.TextBox_IEEE_CID.Text)
            {
                Configuration.Manager.setConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_CID, this.TextBox_IEEE_CID.Text);
                ConfigUpdated = true;
            }

            if (Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Ethertype) != this.TextBox_IEEE_Ethertype.Text)
            {
                Configuration.Manager.setConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Ethertype, this.TextBox_IEEE_Ethertype.Text);
                ConfigUpdated = true;
            }

            if (Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Manufacturer) != this.TextBox_IEEE_Manufacturer.Text)
            {
                Configuration.Manager.setConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Manufacturer, this.TextBox_IEEE_Manufacturer.Text);
                ConfigUpdated = true;
            }

            if (Configuration.Manager.getConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Operator) != this.TextBox_IEEE_Operator.Text)
            {
                Configuration.Manager.setConfigurationElements(ref configParameters.configurationData, Configuration.Manager.ConfigurationElements.IEEE_Operator, this.TextBox_IEEE_Operator.Text);
                ConfigUpdated = true;
            }

            if (ConfigUpdated == true)
            {
                ResourceLoader resourceLoader = new ResourceLoader();
                MessageDialog msgbox = new MessageDialog(resourceLoader.GetString("pageConfig_DialogConfigUpdate"), resourceLoader.GetString("pageConfig_DialogConfigUpdateTitle"));

                msgbox.Commands.Clear();
                msgbox.Commands.Add(new UICommand { Label = resourceLoader.GetString("pageConfig_DialogYes"), Id = 0 });
                msgbox.Commands.Add(new UICommand { Label = resourceLoader.GetString("pageConfig_DialogNo"), Id = 1 });

                var res = await msgbox.ShowAsync();

                if ((int)res.Id == 0)
                {
                    configParameters.reloadCallback(true, true, configParameters.configurationData);
                }
                else {
                    configParameters.reloadCallback(false, true, configParameters.configurationData);
                }
            }
            else
            {
                configParameters.reloadCallback(false, true, configParameters.configurationData);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            configParameters.reloadCallback(false, true, configParameters.configurationData);
        }

        private async void undoButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceLoader resourceLoader = new ResourceLoader();
            MessageDialog msgbox = new MessageDialog(resourceLoader.GetString("pageConfig_DialogConfigReset"), resourceLoader.GetString("pageConfig_DialogConfigResetTitle"));

            msgbox.Commands.Clear();
            msgbox.Commands.Add(new UICommand { Label = resourceLoader.GetString("pageConfig_DialogYes"), Id = 0 });
            msgbox.Commands.Add(new UICommand { Label = resourceLoader.GetString("pageConfig_DialogNo"), Id = 1 });

            var res = await msgbox.ShowAsync();

            if ((int)res.Id == 0)
            {
                configParameters.configurationData = Configuration.Manager.defaultConfiguration();
                updateDialog();
                configParameters.reloadCallback(true, false, configParameters.configurationData);
            }
        }
    }
}
