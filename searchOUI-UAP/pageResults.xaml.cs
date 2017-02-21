using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace searchOUIDB
{
    public sealed partial class pageResults : Page
    {
        public pageResults()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<Records.Items> results = null;

            if (e.Parameter is List<Records.Items>)
            {
                results = (List < Records.Items > )e.Parameter;
            }

            base.OnNavigatedTo(e);

            if (results != null)
            {
                    results.Sort(delegate (Records.Items x, Records.Items y) { return x.Oid.CompareTo(y.Oid); });
                    this.gridView.ItemsSource = results;
            }
        }

        private async void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Grid grid = (Grid)sender;
            Records.Items items = (Records.Items)grid.DataContext;
            String Data = String.Empty;
            ResourceLoader resourceLoader = new ResourceLoader();

            Data += String.Format(resourceLoader.GetString("pageResult_Assignment") + Environment.NewLine, items.Oid);
            Data += String.Format(resourceLoader.GetString("pageResult_Registry") + Environment.NewLine, items.RegistryID);
            Data += String.Format(resourceLoader.GetString("pageResult_OrganizationName") + Environment.NewLine, items.Name);
            Data += String.Format(resourceLoader.GetString("pageResult_OrganizationAddress") + Environment.NewLine, items.Address);

            if ( (items.Protocol != String.Empty) && (items.Protocol != null))
            {
                Data += String.Format(resourceLoader.GetString("pageResult_Protocol") + Environment.NewLine, items.Protocol);
            }

            DataPackage dataPackage = new DataPackage();

            dataPackage.SetText(Data);
            Clipboard.SetContent(dataPackage);

            MessageDialog msgbox = new MessageDialog(Data, resourceLoader.GetString("pageResult_DialogCopied"));

            msgbox.Commands.Clear();
            msgbox.Commands.Add(new UICommand { Label = resourceLoader.GetString("pageResult_DialogClose"), Id = 0 });
 
            var res = await msgbox.ShowAsync();
        }
    }
}
