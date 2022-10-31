

using Windows.Foundation;
using Tranquility.Security;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using System;

namespace Tranquility.Views
{
    public sealed partial class MainPage : Page
    {
        public bool firstRun = false;
        public bool import = false;
        public MainPage()
        {
            this.InitializeComponent();

            if (Core.Runtime.SuccessfullyLoaded == false)
            {
                this.PasswordField.PlaceholderText = "Create a Wallet Password";
                this.AuthButton.Content = "Create Wallet";
                this.importSwitch.Visibility = Visibility.Visible;
                firstRun = true;
            }
            else
            {

                this.PasswordField.PlaceholderText = "Enter Wallet Password";
                this.AuthButton.Content = "Login";

            }
        }
      


        private void App_SizeChanged(object sender, SizeChangedEventArgs e)
        {

           // ApplicationView.GetForCurrentView().TryResizeView(new Size(970, 540));
        }

        private async void Auth_click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (import != true)
                {
                    if (String.IsNullOrEmpty(PasswordField.Password))
                        throw new FormatException("Solana Vault requires a password!");

                    if (firstRun == true)
                    {
                        if (!String.IsNullOrEmpty(PasswordField.Password))
                            Wallets.SolanaWallet.CreateNewWallet(PasswordField.Password);
                        Core.Runtime.SelectedAccount = 0;
                        await System.Threading.Tasks.Task.Delay(1000);
                       
                        Frame.Navigate(typeof(Overview));
                    }
                    else
                    {
                        var hashPW = SHA512.Hash(PasswordField.Password);
                        var hashPWReal = await DataProtection.UnprotectData(Core.Runtime.SolanaVault.HashedProtectedKey);
                        if (hashPW == hashPWReal)
                        {
                            //password matched
                            Core.Runtime.SelectedAccount = 0;
                            Core.Runtime.SolanaVault.ProtectedSessionKey = await DataProtection.ProtectAsync(PasswordField.Password);
                            await System.Threading.Tasks.Task.Delay(1000);
                            
                            Frame.Navigate(typeof(Overview));
                        }
                        else
                        {
                            LoginMessageBlock.Visibility = Visibility.Visible;
                            LoginMessageBlock.Text = "Incorrect Password!";
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(PasswordField.Password))
                        Wallets.SolanaWallet.ImportWallet(seedPhraseField.Password, PasswordField.Password);
                    Core.Runtime.SelectedAccount = 0;
                    await System.Threading.Tasks.Task.Delay(1000);
                  
                    Frame.Navigate(typeof(Overview));
                }
            }catch (FormatException ex)
            {
                LoginMessageBlock.Text = ex.Message;
                LoginMessageBlock.Visibility = Visibility.Visible;
            }
        }


        private void AppTitle_Copy_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if(importSwitch.IsOn)
            {
                seedPhraseField.Visibility = Visibility.Visible;
                AuthButton.Content = "Import Wallet";
                import = true;
            }
            else
            {
                seedPhraseField.Visibility = Visibility.Collapsed;
                AuthButton.Content = "Create Wallet";
                import = false;
            }
        }

        private void PasswordField_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loading(FrameworkElement sender, object args)
        {

           
        }
    }
}
