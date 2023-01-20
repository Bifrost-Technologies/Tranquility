using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tranquility.Security;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tranquility.Views
{
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        

        public SettingsPage()
        {
            InitializeComponent();
            Settings.BorderBrush = save_rpc.BorderBrush;
            Settings.BorderThickness = new Thickness(1, 1, 1, 1);
           
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (Core.Runtime.WalletRPCprovider != null & Core.Runtime.WalletRPCprovider != "https://falling-light-sailboat.solana-mainnet.discover.quiknode.pro/5236d424fa1fd0f1e0fda142470aea120c0d2e3f/")
                    rpcfield.Text = Core.Runtime.WalletRPCprovider;
            }
            catch
            {

            }
            await Task.CompletedTask;
        }

  

    

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void save_rpc_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SaveRPCProvider(rpcfield.Text);
        }

        private async void unlock_seed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hashPW = SHA512.Hash(seedphrase_passphasefield.Password);
                var hashPWReal = await DataProtection.UnprotectData(Core.Runtime.SolanaVault.HashedProtectedKey);
                if (hashPW == hashPWReal)
                {
                    PhraseDisplay.Text = await DataProtection.UnprotectData(Core.Runtime.SolanaVault.Wallet);
                }
            }
            catch
            {

            }
        }

        private async void createAcc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Core.Runtime.SolanaVault.WalletIndexChart.Add(Convert.ToInt32(indexSelector.Text));
                Core.Runtime.SolanaVault.SaveWalletIndex();
                await Wallets.SolanaWallet.GenerateActiveAccounts();
            }
            catch
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

        private void OpenSerum_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 6;
            Frame.Navigate(typeof(Trade));
        }
    }
}
