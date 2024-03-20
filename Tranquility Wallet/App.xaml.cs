using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LinkStream.Server;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc;
using Solnet.Wallet;
using Solnet.Rpc.Models;
using System.Diagnostics;

namespace Tranquility 
{ 
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    
                }
                Window.Current.Content = rootFrame;
            }
            
           Storage.SolanaVault _solanaVault = new Storage.SolanaVault();
           Core.Runtime.LoadRPCProvider();
           if (Core.Runtime.WalletRPCprovider == null | Core.Runtime.WalletRPCprovider == String.Empty)
           {
               Core.Runtime.WalletRPCprovider = "https://cold-smart-friday.solana-mainnet.discover.quiknode.pro/72b2abc7eb8dd76a205f25f0144e3d992d5b134b/";
           }
           Core.Runtime.SolanaVault = _solanaVault;
            Core.Runtime.openbookMarkets = await Trading.OpenBookAPI.GetMarkets();
            await Task.Delay(50);
            Core.Runtime.tokenMintDatabase = await Solnet.Extensions.TokenMintResolver.LoadAsync();
          

           if (Core.Runtime.SolanaVault.AccountStorage == null)
           {
               Core.Runtime.SolanaVault.AccountStorage = new List<Windows.Storage.Streams.IBuffer>();
            }
            if (Core.Runtime.SolanaVault.WalletIndexChart == null)
            {
                Core.Runtime.SolanaVault.WalletIndexChart = new List<int>();
                Core.Runtime.SolanaVault.WalletIndexChart.Add(0);
            }
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(@"checknetisolation loopbackexempt -c");
            cmd.StandardInput.WriteLine(@"checknetisolation loopbackexempt -a -n=BifrostInc.TranquilityWallet_fvkdpj831z6pj");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Debug.WriteLine(cmd.StandardOutput.ReadToEnd());
            Core.Runtime.linkNetwork = new LinkNetwork(50505, "127.0.0.1", "Tranquility");
            Core.Runtime.linkNetwork.signatureRequestEvent += HandleRequestEvent;
            Core.Runtime.linkNetwork.isOnline = true;
           
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(Views.MainPage), e.Arguments);
                }
                Window.Current.Activate();
            }
             //await Core.Runtime.linkNetwork.LinkStream();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        async void HandleRequestEvent(object? sender, SignRequestEventArgs e)
        {
            Debug.WriteLine("Reached Successfully!");
            //Our pseudo wallet app decodes the instructions and displays it. In a real application the user would click a button and sign the transaction after reading the instructions. In this example we auto sign and send the transaction.
            string _decodedInstructions = PacketProcessor.DecodeTransactionMessage(Convert.FromBase64String(e.Message));
            Console.WriteLine(_decodedInstructions);

            IRpcClient rpcClient = ClientFactory.GetClient(Cluster.MainNet);

            Wallet wallet = new Wallet("", passphrase: "");
            Account signer = wallet.GetAccount(0);

            byte[] transactionMessage = Convert.FromBase64String(e.Message);
            byte[] signedTransaction = signer.Sign(transactionMessage);
            List<byte[]> signatures = new() { signedTransaction };
            Transaction tx = Transaction.Populate(Message.Deserialize(transactionMessage), signatures);

            RequestResult<string> _tx = await rpcClient.SendTransactionAsync(tx.Serialize());
        }
    }
}
