using Solana.Metaplex;
using Solnet.Extensions;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tranquility.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Send : Page
    {
        public Send()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
            SendNavButton.BorderBrush = pageborder.BorderBrush;
            SendNavButton.BorderThickness = new Thickness(1, 1, 1, 1);
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Wallets.SolanaWallet.RetrieveActiveAccountData();

                TokenWalletSelector.Items.Add("SOL");
                foreach (var account in Core.Runtime.tokenWallet.TokenAccounts())
                {
                    if (account.QuantityDecimal != 0 & account.Symbol != "USDC" && !String.IsNullOrEmpty(account.Symbol) || account.Symbol == "USDC")
                    {
                        TokenWalletSelector.Items.Add(account.Symbol);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBlock.Text = "Something went wrong! Check your internet connection and restart the app!";
                MessageBlock.Visibility = Visibility.Visible;
            }

        }
        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (TokenWalletSelector.SelectedValue.ToString() == "SOL")
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
                        Wallets.SolanaWallet.SendSOL(SolanaAddressSendField.Text, amount);
                        MessageBlock.Visibility = Visibility.Visible;
                        MessageBlock.Text = "SOL has been sent!";
                    }

                }
                else
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
                        TokenWalletAccount currentAcc = null;
                        currentAcc = Core.Runtime.tokenWallet.TokenAccounts().WithCustomFilter(a => a.Symbol == TokenWalletSelector.SelectedValue.ToString()).ToArray()[0];

                        Wallets.SolanaWallet.SendTokens(SolanaAddressSendField.Text, currentAcc.TokenMint, amount);
                        MessageBlock.Text = "Tokens have been sent!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBlock.Visibility = Visibility.Visible;
                MessageBlock.Text = "Something went wrong! Restart the app and check your internet connection!";
            }
        }

        private void TokenWalletSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var TokenAccounts = Core.Runtime.tokenWallet.TokenAccounts();
                if (TokenWalletSelector.SelectedItem.ToString() == "SOL")
                {
                    BalanceLabel.Text = Core.Runtime.tokenWallet.Sol.ToString();
                }
                else
                {
                    BalanceLabel.Text = TokenAccounts.Where(x => x.Symbol == TokenWalletSelector.SelectedItem.ToString()).First().QuantityDecimal.ToString();
                }
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
