using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace searchOUIDB
{
    public sealed partial class MainPage : Page
    {
        private Records.Data recordsData = null;
        private Configuration.Data configurationData = null;
        private Boolean Initialized = false;

        public MainPage()
        {
            this.InitializeComponent();
            loadAll();
            navigateTo(typeof(pageResults), null);
        }

        private String getResourceString(String ResourceID)
        {
            try
            {
                ResourceLoader resourceLoader = new ResourceLoader();
                return (resourceLoader.GetString(ResourceID));
            }
            catch
            {
                return (String.Empty);
            }
        }

        private async Task<IUICommand> messageBox(String Title, String Message)
        {
            List<UICommand> uiCommandList = new List<UICommand>();
            uiCommandList.Add(new UICommand { Label = getResourceString("MainPage_ErrorOK"), Id = 0 });
            return (await messageBox(Title, Message, uiCommandList));
        }

        private async Task<IUICommand> messageBox(String Title, String Message, List<UICommand> uiCommandList)
        {
            MessageDialog msgbox = new MessageDialog(Message, Title);
            msgbox.Commands.Clear();
            foreach (UICommand uiCommand in uiCommandList)
            {
                msgbox.Commands.Add(uiCommand);
            }
            return (await msgbox.ShowAsync());
        }

        private async void loadAll()
        {
            await loadConfigurationAsync();
            await loadRecordsDataAsync(false);
        }

        private async Task loadConfigurationAsync()
        {
            try
            {
                configurationData = await Configuration.Manager.loadConfigurationAsync();
            }
            catch (Exception e)
            {
                IUICommand Result = await messageBox(getResourceString("MainPage_ErrorHeader"), e.ToString());
            }
        }

        private async Task loadRecordsDataAsync(Boolean Refresh)
        {
            try
            {
                recordsData = await Records.Loader.loadAsync(configurationData, Refresh, recordLoaderCallback);
            }
            catch
            {
                IUICommand Result = await messageBox(getResourceString("MainPage_ErrorHeader"), getResourceString("MainPage_ErrorDownload"));
                statusFooter.Text = getResourceString("MainPage_RecordLoaderError");
            }
        }

        private void recordLoaderCallback(Records.Loader.States state, Int32 Count, Int32 Total, UInt64 RecordCount)
        {
            switch (state)
            {
                case Records.Loader.States.start:
                    statusFooter.Text = String.Format(getResourceString("MainPage_RecordLoaderStart"), Count, Total, RecordCount);
                    Initialized = false;
                    break;
                case Records.Loader.States.update:
                    statusFooter.Text = String.Format(getResourceString("MainPage_RecordLoaderUpdate"), Count, Total, RecordCount);
                    break;
                case Records.Loader.States.done:
                    statusFooter.Text = String.Format(getResourceString("MainPage_RecordLoaderStop"), Count, Total, RecordCount);
                    Initialized = true;
                    break;
            }
        }

        private void navigateTo(Type target, Object Parameters)
        {
            Frame frame = this.SplitViewContent as Frame;
            Page page = frame?.Content as Page;
            frame.Navigate(target, Parameters);
        }

        private void configButton_Click(object sender, RoutedEventArgs e)
        {
            navigateTo(typeof(pageConfig), (Object)new ConfigParameters(configReloadCallback, configurationData));
        }

        private async void configReloadCallback(Boolean Reload, Boolean Close, Configuration.Data configurationData)
        {
            this.configurationData = configurationData;

            if (Reload == true)
            {
                await loadRecordsDataAsync(true);
            }

            if (Close == true)
            {
                navigateTo(typeof(pageResults), null);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchRun();
        }

        private void searchBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                searchRun();
                e.Handled = true;
            }
        }

        private async void searchRun()
        {
            if ((Initialized == true) && (!this.searchBox.CueBannerState))
            {
                statusFooter.Text = getResourceString("MainPage_SearchProgress");
                List<Records.Items> results = await this.recordsData.searchAsync(this.searchBox.Text);

                if (results != null)
                {
                    statusFooter.Text = String.Format(getResourceString("MainPage_SearchResult"), results.Count);
                    navigateTo(typeof(pageResults), (Object)results);
                }
                else
                {
                    statusFooter.Text = getResourceString("MainPage_SearchEmpty");
                }
            }
        }

        private async void syncButton_Click(object sender, RoutedEventArgs e)
        {
            await loadRecordsDataAsync(true);
        }
    }
}
