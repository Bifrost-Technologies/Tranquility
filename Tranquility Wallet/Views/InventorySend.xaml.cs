using Solana.Metaplex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tranquility.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InventorySend : Page
    {
        public InventorySend()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
            InventorySendNavButton.BorderBrush = pageborder.BorderBrush;
            InventorySendNavButton.BorderThickness = new Thickness(1,1,1,1);
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Wallets.SolanaWallet.RetrieveActiveAccountData();

                foreach (var item in Core.Runtime.Inventory)
                {
                    if (item.MetadataVersion == 1)
                        InventorySelector.Items.Add(item.metadataV1.name);
                    if (item.MetadataVersion == 3)
                        InventorySelector.Items.Add(item.metadataV3.name);
                }
            }
            catch (Exception ex)
            {
                
            }

        }
        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal amount = 0;
                try
                {
                    amount = Convert.ToDecimal(SendAmountField.Text);
                }
                catch (Exception ex)
                {
                    amount = 0;
                }

                if (SolanaAddressSendField.Text.Count() == 32)
                {
                    MetadataAccount currentAcc = null;
                    currentAcc = Core.Runtime.Inventory.Find(x => x.metadataV3.name == InventorySelector.SelectedValue.ToString() & x.MetadataVersion == 3);
                    if (currentAcc == null)
                        currentAcc = Core.Runtime.Inventory.Find(x => x.metadataV1.name == InventorySelector.SelectedValue.ToString() & x.MetadataVersion == 1);
                    Wallets.SolanaWallet.SendTokens(SolanaAddressSendField.Text, currentAcc.mint, amount);
                }
            }
            catch (Exception ex)
            {

            }
        }

     
        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Overview));
        }

        private void TokenWalletSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MetadataAccount asset_image = null;
                asset_image = Core.Runtime.Inventory.Find(x => x.metadataV3.name == InventorySelector.SelectedValue.ToString() & x.MetadataVersion == 3);
                if (asset_image == null)
                    asset_image = Core.Runtime.Inventory.Find(x => x.metadataV1.name == InventorySelector.SelectedValue.ToString() & x.MetadataVersion == 1);

                BitmapImage digitalAsset = new BitmapImage(new Uri(asset_image.offchainData.image));
                CollectiblePreview.Source = digitalAsset;
            }
            catch (Exception ex)
            {

            }
        }

        private void ReceiveNavButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 2;
            Frame.Navigate(typeof(Receive));
        }

        private void OverviewNavButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 1;
            Frame.Navigate(typeof(Overview));
        }

        private void SendNavButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 3;
            Frame.Navigate(typeof(Send));

        }

        private void InventorySendNavButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 4;
            Frame.Navigate(typeof(InventorySend));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 5;
            Frame.Navigate(typeof(SettingsPage));
        }
    }
}
