using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tranquility.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Receive : Page
    {
        public Receive()
        {
            this.InitializeComponent();
            try
            {
                this.WalletAddressDisplayBlock.Text = Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address;
            }
            catch(Exception ex)
            {

            }
            ReceiveNavButton.BorderBrush = pageborder.BorderBrush;
            ReceiveNavButton.BorderThickness = new Thickness(1,1,1,1);
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

        private async void Copybutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetText(Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address);
                Clipboard.SetContent(dataPackage);
                MessageBlock.Text = "Wallet Address Copied Successfully!";
                MessageBlock.Visibility = Visibility.Visible;
                await Task.Delay(5000);
                MessageBlock.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBlock.Text = "Could not find address or your internet connection is lost!";
                MessageBlock.Visibility = Visibility.Visible;
                await Task.Delay(10000);
                MessageBlock.Visibility = Visibility.Collapsed;
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
