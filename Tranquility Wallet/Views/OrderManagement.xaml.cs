using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Models;
using Solnet.Serum.Models;
using Solnet.Serum;
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
using Solnet.Rpc;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tranquility.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderManagement : Page
    {
        public OrderManagement()
        {
            this.InitializeComponent();

            ReceiveNavButton.BorderBrush = pageborder.BorderBrush;
            ReceiveNavButton.BorderThickness = new Thickness(1, 1, 1, 1);
            this.Loaded += Page_Loaded;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
               await LoadOpenOrders();

            }
            catch (Exception aa)
            {
                Debug.WriteLine(aa);
            }

        }
        public async Task LoadOpenOrders()
        {
            var _SerumClient = Solnet.Serum.ClientFactory.GetClient(Core.Runtime.WalletRPCprovider);
            Market market = Core.Runtime.ActiveMarket;

            // Get open orders accounts for a market.
            List<MemCmp> filters = new()
            {
                new MemCmp{ Offset = 13, Bytes = market.OwnAddress },
                new MemCmp{ Offset = 45, Bytes = Core.Runtime.tokenWallet.PublicKey}
            };
            var rpcClient = Solnet.Rpc.ClientFactory.GetClient(Core.Runtime.WalletRPCprovider);
            RequestResult<List<AccountKeyPair>> accounts = await rpcClient.GetProgramAccountsAsync(SerumProgram.MainNetProgramIdKeyV3, dataSize: OpenOrdersAccount.Layout.SpanLength, memCmpList: filters);

            /* Print all of the found open orders accounts */
            foreach (AccountKeyPair account in accounts.Result)
            {
                Debug.WriteLine($"---------------------");
                Debug.WriteLine($"OpenOrdersAccount: {account.PublicKey} - Owner: {account.Account.Owner}");
                OpenOrdersAccount ooa = OpenOrdersAccount.Deserialize(Convert.FromBase64String(account.Account.Data[0]));
                Debug.WriteLine($"OpenOrdersAccount:: Owner: {ooa.Owner.Key} Market: {ooa.Market.Key}\n" +
                                  $"BaseTotal: {ooa.BaseTokenTotal} BaseFree: {ooa.BaseTokenFree}\n" +
                                  $"QuoteTotal: {ooa.QuoteTokenTotal} QuoteFree: {ooa.QuoteTokenFree}");
                Debug.WriteLine($"---------------------");
            }
            string openOrdersAddress = accounts.Result[0].PublicKey;
            OpenOrdersAccount openOrdersAccount = OpenOrdersAccount.Deserialize(Convert.FromBase64String(accounts.Result[0].Account.Data[0]));


            foreach (OpenOrder openOrder in openOrdersAccount.Orders)
            {
                Debug.WriteLine($"OpenOrder:: Bid: {openOrder.IsBid}" +
                                  $"Price: {openOrder.RawPrice}" +
                                  $"Quantity: {openOrder.RawQuantity}");

                ComboBoxItem openOrderitem = new ComboBoxItem();
                openOrderitem.Name = $"OpenOrder:: Bid: {openOrder.IsBid}" +
                                  $"Price: {openOrder.RawPrice}" +
                                  $"Quantity: {openOrder.RawQuantity}";
        
                openOrderitem.Tag = openOrder;

                OrderSelectionBox.Items.Add(openOrderitem);

            }
            await Task.CompletedTask;
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

        private async void Copybutton_Click(object sender, RoutedEventArgs e)
        {
          

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

        private void save_rpc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged_2(object sender, RoutedEventArgs e)
        {

        }

        private void basepairAmount_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
