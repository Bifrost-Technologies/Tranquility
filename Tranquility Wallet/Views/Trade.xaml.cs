using Solana.Metaplex;
using Solnet.Extensions;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Serum;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Tranquility.Wallets;
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
    public sealed partial class Trade : Page
    {
        private string ActiveTokenSymbol { get; set; }
        private string ActiveTokenName { get; set; }

        private string ActiveQuoteTokenSymbol { get; set; }
        private string ActiveQuoteTokenName { get; set; }
        public Trade()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
            TradeNavButton.BorderBrush = pageborder.BorderBrush;
            TradeNavButton.BorderThickness = new Thickness(1, 1, 1, 1);
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Wallets.SolanaWallet.RetrieveActiveAccountData();
                Market market = await Trading.OpenBookAPI.GetMarket("8BnEgHoWFysVcuFFX7QztDmzuH8r5ZFvyP3sYwn1XTh6");

                Core.Runtime.ActiveOrderBook = await Trading.OpenBookAPI.GetOrderBookData("8BnEgHoWFysVcuFFX7QztDmzuH8r5ZFvyP3sYwn1XTh6");

                await LoadOpenBookMarketSelector();
                await LoadOrderBook(market);
                Debug.WriteLine("Successfully retrieved orderbook! " + Core.Runtime.ActiveOrderBook.Key.Count);

            }
            catch (Exception aa)
            {
                Debug.WriteLine(aa);
            }

        }
        private async Task LoadOpenBookMarketSelector()
        {
            foreach (var minfo in Core.Runtime.openbookMarkets)
            {
                if (!minfo.Name.Contains("so"))
                {


                    ComboBoxItem mentry = new ComboBoxItem();
                    mentry.Content = minfo.Name;
                    mentry.Name = minfo.Name;
                    mentry.Tag = minfo;
                    OpenBookMarketSelector.Items.Add(mentry);
                }
            }
            await Task.CompletedTask;
        }
        private async Task LoadOrderBook(Market _market)
        {
            double totalOBbuyVolume = 0;
            double totalOBsellVolume = 0;
            double snowballBuyVolume = 0;
            double snowballSellVolume = 0;
            var basetoken = Core.Runtime.tokenMintDatabase.Resolve(_market.BaseMint.Key);
            var quotetoken = Core.Runtime.tokenMintDatabase.Resolve(_market.QuoteMint.Key);
            if (Core.Runtime.ActiveOrderBook.Key != null && Core.Runtime.ActiveOrderBook.Key.Count > 0 && Core.Runtime.ActiveOrderBook.Value != null && Core.Runtime.ActiveOrderBook.Value.Count > 0)
            {


                foreach (var order in Core.Runtime.ActiveOrderBook.Key)
                {
                    if (order.RawPrice > 1000)
                    {
                        totalOBbuyVolume += (Convert.ToDouble(order.RawQuantity) / 1000);
                    }
                }
               
                Debug.WriteLine("Total OrderBook Buy Volume: " + totalOBbuyVolume);


                ProgressBar volumeBar = new ProgressBar();
                TextBlock quantityLabel = new TextBlock();
                TextBlock priceLabel = new TextBlock();

                volumeBar.Width = ((ProgressBar)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.First()).Width;
                volumeBar.Height = ((ProgressBar)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.First()).Height;
                volumeBar.Margin = ((ProgressBar)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.First()).Margin;
                volumeBar.Foreground = ((ProgressBar)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.First()).Foreground;
                priceLabel.Margin = ((TextBlock)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.Last()).Margin;
                quantityLabel.Margin = ((TextBlock)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children[1]).Margin;
                quantityLabel.Width = ((TextBlock)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children[1]).Width;
                quantityLabel.Height = ((TextBlock)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children[1]).Height;
                BuyOrders.Items.Clear();
                foreach (var _buyOrder in Core.Runtime.ActiveOrderBook.Key)
                {
                    ListViewItem orderView = new ListViewItem();
                    Canvas freshOrder = new Canvas();
                    ProgressBar _volumeBar = new ProgressBar { Width = volumeBar.Width, Height = volumeBar.Height, Margin = volumeBar.Margin, Foreground = volumeBar.Foreground };
                    TextBlock _quantityLabel = new TextBlock { Width = quantityLabel.Width, Height = quantityLabel.Height, Margin = quantityLabel.Margin };
                    TextBlock _priceLabel = new TextBlock { Margin = priceLabel.Margin };
                    freshOrder.Width = 421;
                    freshOrder.Height = 38;
                    _quantityLabel.Text = (Convert.ToDouble(_buyOrder.RawQuantity) / 1000).ToString() + " " + basetoken.Symbol;
                    _volumeBar.Maximum = totalOBbuyVolume;
                    _volumeBar.Value = (snowballBuyVolume += (Convert.ToDouble(_buyOrder.RawQuantity) / 1000));
                    _priceLabel.Text = "$" + (Convert.ToDouble(_buyOrder.RawPrice) / 1000).ToString();
                    if (quotetoken.Symbol == "USDC" | quotetoken.Symbol == "USDT")
                        _priceLabel.Text = "$" + (Convert.ToDouble(_buyOrder.RawPrice) / 1000).ToString();


                    freshOrder.Children.Add(_volumeBar);
                    freshOrder.Children.Add(_priceLabel);
                    freshOrder.Children.Add(_quantityLabel);
                    orderView.Content = freshOrder;

                    this.BuyOrders.Items.Add(orderView);

                }

                Core.Runtime.ActiveOrderBook.Value.ForEach(abc => totalOBsellVolume += (Convert.ToDouble(abc.RawQuantity) / 1000));
                totalOBsellVolume -= Convert.ToDouble(Core.Runtime.ActiveOrderBook.Value.Last().RawQuantity) / 1000;
                ProgressBar volumeBar2 = new ProgressBar();
                TextBlock quantityLabel2 = new TextBlock();
                TextBlock priceLabel2 = new TextBlock();

                volumeBar2.Width = ((ProgressBar)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.First()).Width;
                volumeBar2.Height = ((ProgressBar)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.First()).Height;
                volumeBar2.Margin = ((ProgressBar)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.First()).Margin;
                volumeBar2.Foreground = ((ProgressBar)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.First()).Foreground;
                priceLabel2.Margin = ((TextBlock)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.Last()).Margin;
                quantityLabel2.Margin = ((TextBlock)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children[1]).Margin;
                quantityLabel2.Width = ((TextBlock)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children[1]).Width;
                quantityLabel2.Height = ((TextBlock)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children[1]).Height;


                SellOrders.Items.Clear();
                foreach (var _sellOrder in Core.Runtime.ActiveOrderBook.Value)
                {
                    ListViewItem orderView2 = new ListViewItem();
                    Canvas freshOrder2 = new Canvas();
                    ProgressBar _volumeBar2 = new ProgressBar { Width = volumeBar2.Width, Height = volumeBar2.Height, Margin = volumeBar2.Margin, Foreground = volumeBar2.Foreground };
                    TextBlock _quantityLabel2 = new TextBlock { Width = quantityLabel2.Width, Height = quantityLabel2.Height, Margin = quantityLabel2.Margin };
                    TextBlock _priceLabel2 = new TextBlock { Margin = priceLabel2.Margin };
                    freshOrder2.Width = 421;
                    freshOrder2.Height = 38;
                    _quantityLabel2.Text = (Convert.ToDouble(_sellOrder.RawQuantity) / 1000).ToString() + " " + basetoken.Symbol;
                    _priceLabel2.Text = "$" + (Convert.ToDouble(_sellOrder.RawPrice) / 1000).ToString();
                    _volumeBar2.Maximum = totalOBsellVolume;
                    _volumeBar2.Value = (snowballSellVolume += (Convert.ToDouble(_sellOrder.RawQuantity) / 1000));
                    if (quotetoken.Symbol == "USDC" | quotetoken.Symbol == "USDT")
                        _priceLabel2.Text = "$" + (Convert.ToDouble(_sellOrder.RawPrice) / 1000).ToString();

                    freshOrder2.Children.Add(_volumeBar2);
                    freshOrder2.Children.Add(_priceLabel2);
                    freshOrder2.Children.Add(_quantityLabel2);
                    orderView2.Content = freshOrder2;

                    this.SellOrders.Items.Add(orderView2);


                }
                medianpriceLabel.Text = (Convert.ToDouble(Core.Runtime.ActiveOrderBook.Key.First().RawPrice) / 1000).ToString();
                if (quotetoken.Symbol == "USDC" | quotetoken.Symbol == "USDT")
                    medianpriceLabel.Text = "$" + (Convert.ToDouble(Core.Runtime.ActiveOrderBook.Key.First().RawPrice) / 1000).ToString();
                var reversed = this.SellOrders.Items.Reverse().ToArray();
                this.SellOrders.Items.Clear();
                reversed.ToList().ForEach(so => SellOrders.Items.Add(so));
                SellOrders.ScrollIntoView(SellOrders.Items.Last());
            }
            else
            {
                ProgressBar volumeBar = new ProgressBar();
                TextBlock quantityLabel = new TextBlock();
                TextBlock priceLabel = new TextBlock();

                volumeBar.Width = ((ProgressBar)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.First()).Width;
                volumeBar.Height = ((ProgressBar)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.First()).Height;
                volumeBar.Margin = ((ProgressBar)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.First()).Margin;
                volumeBar.Foreground = ((ProgressBar)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.First()).Foreground;
                priceLabel.Margin = ((TextBlock)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children.Last()).Margin;
                quantityLabel.Margin = ((TextBlock)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children[1]).Margin;
                quantityLabel.Width = ((TextBlock)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children[1]).Width;
                quantityLabel.Height = ((TextBlock)((Canvas)((ListViewItem)BuyOrders.Items[0]).Content).Children[1]).Height;

                BuyOrders.Items.Clear();
                ListViewItem orderView = new ListViewItem();
                Canvas freshOrder = new Canvas();
                ProgressBar _volumeBar = new ProgressBar { Width = volumeBar.Width, Height = volumeBar.Height, Margin = volumeBar.Margin, Foreground = volumeBar.Foreground };
                TextBlock _quantityLabel = new TextBlock { Width = quantityLabel.Width, Height = quantityLabel.Height, Margin = quantityLabel.Margin };
                TextBlock _priceLabel = new TextBlock { Margin = priceLabel.Margin };
                freshOrder.Width = 421;
                freshOrder.Height = 38;
                _quantityLabel.Text = 0 + " " + basetoken.Symbol;
                _volumeBar.Maximum = 100;
                _volumeBar.Value = 0;
                _priceLabel.Text = "";
                if (quotetoken.Symbol == "USDC" | quotetoken.Symbol == "USDT")
                    _priceLabel.Text = "";


                freshOrder.Children.Add(_volumeBar);
                freshOrder.Children.Add(_priceLabel);
                freshOrder.Children.Add(_quantityLabel);
                orderView.Content = freshOrder;

                this.BuyOrders.Items.Add(orderView);



                ProgressBar volumeBar2 = new ProgressBar();
                TextBlock quantityLabel2 = new TextBlock();
                TextBlock priceLabel2 = new TextBlock();

                volumeBar2.Width = ((ProgressBar)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.First()).Width;
                volumeBar2.Height = ((ProgressBar)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.First()).Height;
                volumeBar2.Margin = ((ProgressBar)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.First()).Margin;
                volumeBar2.Foreground = ((ProgressBar)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.First()).Foreground;
                priceLabel2.Margin = ((TextBlock)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children.Last()).Margin;
                quantityLabel2.Margin = ((TextBlock)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children[1]).Margin;
                quantityLabel2.Width = ((TextBlock)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children[1]).Width;
                quantityLabel2.Height = ((TextBlock)((Canvas)((ListViewItem)SellOrders.Items[0]).Content).Children[1]).Height;


                SellOrders.Items.Clear();
                ListViewItem orderView2 = new ListViewItem();
                Canvas freshOrder2 = new Canvas();
                ProgressBar _volumeBar2 = new ProgressBar { Width = volumeBar2.Width, Height = volumeBar2.Height, Margin = volumeBar2.Margin, Foreground = volumeBar2.Foreground };
                TextBlock _quantityLabel2 = new TextBlock { Width = quantityLabel2.Width, Height = quantityLabel2.Height, Margin = quantityLabel2.Margin };
                TextBlock _priceLabel2 = new TextBlock { Margin = priceLabel2.Margin };
                freshOrder2.Width = 421;
                freshOrder2.Height = 38;
                _quantityLabel2.Text = 0 + " " + basetoken.Symbol;
                _priceLabel2.Text = "";
                _volumeBar2.Maximum = 100;
                _volumeBar2.Value = 0;
                if (quotetoken.Symbol == "USDC" | quotetoken.Symbol == "USDT")
                    _priceLabel2.Text = "";

                freshOrder2.Children.Add(_volumeBar2);
                freshOrder2.Children.Add(_priceLabel2);
                freshOrder2.Children.Add(_quantityLabel2);
                orderView2.Content = freshOrder2;

                this.SellOrders.Items.Add(orderView2);


                medianpriceLabel.Text = "No Open Orders for this market! Empty Orderbook..";
            }
            await Task.CompletedTask;
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

        private async void Transfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float requestedSize = (float)Convert.ToDecimal(sizeField.Text);
                float requestedPrice = (float)Convert.ToDecimal(priceField.Text);
                var ordertype = Side.Buy;
                if (OrderTypeSwitch.IsOn == true)
                {
                    ordertype = Side.Sell;
                }

                Order NewOrder = new OrderBuilder()
                    .SetPrice(requestedPrice)
                    .SetQuantity(requestedSize)
                    .SetSide(ordertype)
                    .SetOrderType(OrderType.Limit)
                    .SetSelfTradeBehavior(SelfTradeBehavior.DecrementTake)
                    .Build();


                await Wallets.SolanaWallet.Init();
                await Task.Delay(50);
                var a = await Core.Runtime.MarketManager.NewOrderAsync(NewOrder);
                Debug.WriteLine(a.Result.RawRpcResponse);
                Debug.WriteLine(a.ConfirmationResult);
            }
            catch (Exception aa)
            {
                Debug.WriteLine(aa.Message);
            }
        }

        private void OpenSerum_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 6;
            Frame.Navigate(typeof(Trade));
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var marketAddress = ((MarketInfo)((ComboBoxItem)OpenBookMarketSelector.SelectedItem).Tag).Address;
            Market market = await Trading.OpenBookAPI.GetMarket(marketAddress.Key);

            Core.Runtime.ActiveOrderBook = await Trading.OpenBookAPI.GetOrderBookData(marketAddress.Key);
            await LoadOrderBook(market);
        }
      
    }

}
