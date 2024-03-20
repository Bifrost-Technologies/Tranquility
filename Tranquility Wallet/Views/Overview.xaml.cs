using Solnet.Wallet;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Tranquility;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Globalization;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Solana.Metaplex;
using Solana.Metaplex.Json;
using Solnet.Rpc;
using Solnet.Programs;
using Windows.Web.Http;
using Newtonsoft.Json;
using System.Linq;

namespace Tranquility.Views
{
    public sealed partial class Overview : Page
    {
        public const string UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
        public Overview()
        {
            InitializeComponent();
            this.Loading += Overview_Loading;
            OverviewNavButton.BorderBrush = tokenlistPreviewborder.BorderBrush;
            OverviewNavButton.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private async void Overview_Loading(FrameworkElement sender, object args)
        {
            try
            {
                if (Core.Runtime.overviewInitialized == false)
                {
                    await Wallets.SolanaWallet.GenerateActiveAccounts();
                    Core.Runtime.overviewInitialized = true;
                }
                else
                {
                    await Wallets.SolanaWallet.RetrieveActiveAccountData();
                }

                ActiveAccountsSelector.Items.Clear();
                foreach (var activeAcc in Core.Runtime.SolanaVault.ActiveAccounts)
                {
                    ActiveAccountsSelector.Items.Add(activeAcc.Address);
                }
                ActiveAccountsSelector.PlaceholderText = ActiveAccountsSelector.Items[Core.Runtime.SelectedAccount].ToString();
            }catch
            {
                WalletBalance.Text = "Something went wrong.. Check your internet connection!";
            }
            LoadWalletData();
        }

        private async void LoadWalletData()
        {
            try
            {
                decimal wallet_worth = 0;
                var solanaTile = (ImageBrush)((Border)(((ListViewItem)TokenAccountList.Items[0]).Content)).Background;
                var Position = new TranslateTransform();
                Position.X = 20;
                solanaTile.AlignmentX = AlignmentX.Left;
                solanaTile.Transform = Position;
                ((Border)(((ListViewItem)TokenAccountList.Items[0]).Content)).Background = solanaTile;
                ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Text = Core.Runtime.tokenWallet.Sol.ToString() + Environment.NewLine + "SOL";
                Core.Runtime.Inventory = new System.Collections.Generic.List<MetadataAccount>();
                int tokenAccCount = 0;
                if (TokenAccountList.Items.Count > 1)
                {
                    foreach (var token in TokenAccountList.Items.ToArray())
                    {
                        if (token != TokenAccountList.Items[0])
                        {
                            TokenAccountList.Items.Remove(token);
                        }
                        tokenAccCount++;
                    }
                }
                InventoryGrid.Items.Clear();
                foreach (var account in Core.Runtime.tokenWallet.TokenAccounts())
                {
                    if (account.QuantityDecimal != 0 & account.Symbol != "USDC" || account.Symbol == "USDC")
                    {
                        if (account.QuantityDecimal != 0)
                        {
                            var price = await Utilities.birdeye.GetPrice(account.TokenMint);
                            var _tokenAccountWorth = account.QuantityDecimal * price;
                            wallet_worth = wallet_worth + _tokenAccountWorth;
                        }
                        ListViewItem balanceTile = new ListViewItem();
                        Border balanceTileBorder = new Border();

                        var tokenData = Core.Runtime.tokenMintDatabase.Resolve(account.TokenMint);
                        if (tokenData != null & !String.IsNullOrEmpty(tokenData.TokenLogoUrl))
                        {
                            BitmapImage tokenLogo = ResizedImage(tokenData.TokenLogoUrl, 40, 40);
                            ImageBrush tokenItembackground = new ImageBrush { ImageSource = tokenLogo, Stretch = Stretch.None };
                            tokenItembackground.AlignmentX = AlignmentX.Left;
                            tokenItembackground.Transform = Position;
                            balanceTileBorder.Background = tokenItembackground;
                        }
                        else
                        {
                            var rpcClient = ClientFactory.GetClient(Core.Runtime.WalletRPCprovider);
                            MetadataAccount metadataAccount = null;
                            try
                            {
                                metadataAccount = await MetadataAccount.GetAccount(rpcClient, AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(new PublicKey(Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address), new PublicKey(account.TokenMint)), 3);
                            }
                            catch
                            {

                            }
                            if (metadataAccount != null)
                            {
                                ListViewItem InventoryItem = new ListViewItem();
                                Canvas IICanvas = new Canvas();
                                TextBlock ItemTitle = new TextBlock();
                                ItemTitle.Margin = new Thickness(40, 170, 0, 0);
                                ItemTitle.HorizontalAlignment = HorizontalAlignment.Center;
                                ItemTitle.HorizontalTextAlignment = TextAlignment.Center;


                                if (metadataAccount.MetadataVersion == 3)
                                    ItemTitle.Text = metadataAccount.metadataV3.name;

                                using (var httpClient = new HttpClient())
                                {
                                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                                    string offsiteTokenRetrieval = String.Empty;

                                    if (metadataAccount.MetadataVersion == 3)
                                        offsiteTokenRetrieval = await httpClient.GetStringAsync(new Uri(metadataAccount.metadataV3.uri));

                                    MetaplexTokenStandard _Metadata = JsonConvert.DeserializeObject<MetaplexTokenStandard>(offsiteTokenRetrieval);
                                    metadataAccount.offchainData = _Metadata;
                                    BitmapImage digitalAsset = new BitmapImage(new Uri(_Metadata.image));
                                    IICanvas.Background = new ImageBrush { ImageSource = digitalAsset, Stretch = Stretch.Fill };
                                }
                                IICanvas.Width = 195;
                                IICanvas.Height = 195;
                                InventoryItem.CornerRadius = new CornerRadius(15, 15, 15, 15);
                                IICanvas.Margin = new Thickness(2, 2, 2, 2);
                                InventoryItem.Padding = new Thickness(1, 1, 1, 1);
                                InventoryItem.MinHeight = 200;
                                InventoryItem.MinWidth = 198;
                                IICanvas.Children.Add(ItemTitle);
                                InventoryItem.Content = IICanvas;
                                InventoryGrid.Items.Add(InventoryItem);
                                Core.Runtime.Inventory.Add(metadataAccount);
                                continue;
                            }
                            else
                            {
                                try
                                {
                                    metadataAccount = await MetadataAccount.GetAccount(rpcClient, AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(new PublicKey(Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address), new PublicKey(account.TokenMint)), 1);
                                }
                                catch
                                {

                                }
                                if (metadataAccount != null)
                                {
                                    ListViewItem InventoryItem = new ListViewItem();
                                    Canvas IICanvas = new Canvas();
                                    TextBlock ItemTitle = new TextBlock();
                                    ItemTitle.Margin = new Thickness(40, 170, 0, 0);
                                    ItemTitle.HorizontalAlignment = HorizontalAlignment.Center;
                                    ItemTitle.HorizontalTextAlignment = TextAlignment.Center;

                                    if (metadataAccount.MetadataVersion == 1)
                                        ItemTitle.Text = metadataAccount.metadataV1.name;

                                    using (var httpClient = new HttpClient())
                                    {
                                        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                                        string offsiteTokenRetrieval = String.Empty;
                                        if (metadataAccount.MetadataVersion == 1)
                                            offsiteTokenRetrieval = await httpClient.GetStringAsync(new Uri(metadataAccount.metadataV1.uri));

                                        MetaplexTokenStandard _Metadata = JsonConvert.DeserializeObject<MetaplexTokenStandard>(offsiteTokenRetrieval);
                                        metadataAccount.offchainData = _Metadata;
                                        BitmapImage digitalAsset = new BitmapImage(new Uri(_Metadata.image));
                                        IICanvas.Background = new ImageBrush { ImageSource = digitalAsset, Stretch = Stretch.Fill };
                                    }
                                    IICanvas.Width = 195;
                                    IICanvas.Height = 195;
                                    InventoryItem.CornerRadius = new CornerRadius(15, 15, 15, 15);
                                    IICanvas.Margin = new Thickness(2, 2, 2, 2);
                                    InventoryItem.Padding = new Thickness(1, 1, 1, 1);
                                    InventoryItem.MinHeight = 200;
                                    InventoryItem.MinWidth = 198;
                                    IICanvas.Children.Add(ItemTitle);
                                    InventoryItem.Content = IICanvas;
                                    InventoryGrid.Items.Add(InventoryItem);
                                    Core.Runtime.Inventory.Add(metadataAccount);
                                    continue;
                                }
                            }
                        }
                        TextBlock currencyBalance = new TextBlock();
                        balanceTile.Margin = ((ListViewItem)TokenAccountList.Items[0]).Margin;
                        balanceTile.BorderBrush = ((ListViewItem)TokenAccountList.Items[0]).BorderBrush;
                        balanceTile.BorderThickness = ((ListViewItem)TokenAccountList.Items[0]).BorderThickness;
                        balanceTile.Padding = ((ListViewItem)TokenAccountList.Items[0]).Padding;
                        balanceTile.Width = ((ListViewItem)TokenAccountList.Items[0]).Width;
                        balanceTile.Height = ((ListViewItem)TokenAccountList.Items[0]).Height;
                        balanceTile.Background = ((ListViewItem)TokenAccountList.Items[0]).Background;

                        currencyBalance.Height = ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Height;
                        currencyBalance.Margin = ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Margin;
                        currencyBalance.Padding = ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Padding;
                        currencyBalance.Foreground = ((TextBlock)((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Child).Foreground;
                        currencyBalance.Text = account.QuantityDecimal.ToString() + Environment.NewLine + account.Symbol;

                        balanceTileBorder.BorderBrush = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).BorderBrush;
                        balanceTileBorder.BorderThickness = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).BorderThickness;
                        balanceTileBorder.CornerRadius = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).CornerRadius;
                        balanceTileBorder.Width = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Width;
                        balanceTileBorder.Height = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).Height;
                        balanceTileBorder.RenderTransform = ((Border)((ListViewItem)TokenAccountList.Items[0]).Content).RenderTransform;
                        balanceTileBorder.Child = currencyBalance;

                        balanceTile.Content = balanceTileBorder;

                        TokenAccountList.Items.Add(balanceTile);
                    }

                }
                TokenAccountList.UpdateLayout();
                WalletBalance.Text = wallet_worth.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            }
            catch
            {
                WalletBalance.Text = "Something went wrong! Restart the app & check your internet connection";
            }
        }
        public static BitmapImage ResizedImage(string URL, int maxWidth, int maxHeight)
        {
            BitmapImage sourceImage = new BitmapImage(new Uri(URL));
            sourceImage.DecodePixelWidth = maxWidth;
            sourceImage.DecodePixelHeight = maxHeight;

            return sourceImage;
        }
        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
        private void App_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            //ApplicationView.GetForCurrentView().TryResizeView(new Size(960, 540));
        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

        private void Send_Navigate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Send));
        }

        private void Receive_Navigate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Receive));
        }

        private void Trade_Navigate_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                Core.Runtime.SelectedAccount = Core.Runtime.SolanaVault.ActiveAccounts.FindIndex(e => e.Address == ActiveAccountsSelector.SelectedValue.ToString());
                LoadWalletData();
            }
            catch
            {
                WalletBalance.Text = "Something went wrong! Restart the app & check your internet connection!";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
