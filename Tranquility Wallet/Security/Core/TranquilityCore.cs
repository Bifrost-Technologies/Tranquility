using Solnet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Tranquility.Views;
using Solana.Metaplex;
using Windows.Storage;
using Solnet.Serum.Models;
using Solnet.Serum;
using LinkStream.Server;
using System.Diagnostics;
using System.IO;
using Windows.UI.Core;

namespace Tranquility.Core
{
    public static class Runtime
    {
        public static Storage.SolanaVault SolanaVault { get; set; }
        public static int SelectedNavItem { get; set; }
        public static bool overviewInitialized { get; set; }
        public static List<MetadataAccount> Inventory { get; set; }
        public static string WalletRPCprovider { get; set; }
        public static KeyValuePair<List<OpenOrder>, List<OpenOrder>> ActiveOrderBook { get; set; }
        public static TokenWallet tokenWallet { get; set; }
        public static List<MarketInfo> openbookMarkets { get; set; }
        public static bool SuccessfullyLoaded { get; set; }
        public static bool SuccessfullyLoadedAV { get; set; }
        public static bool SuccessfullyLoadedWI { get; set; }
        public static TokenMintResolver tokenMintDatabase { get; set; }
        public static int SelectedAccount { get; set; }
        public static IMarketManager MarketManager { get; set; }
        public static ISerumClient SerumClient { get; set; }

        public static LinkNetwork linkNetwork { get; set; }
        public static bool isLinkEnabled { get; set; }
        public static bool isMonitoring { get; set; }


        public static async Task StartServer()
        {
            try
            {
                var streamSocketListener = new Windows.Networking.Sockets.StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListener.BindServiceNameAsync("50505");
                Debug.WriteLine("server is listening!");
           
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        public static async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }
            Debug.WriteLine("test");
            // Echo the request back as the response.
            using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(request);
                    await streamWriter.FlushAsync();
                }
            }

            

            sender.Dispose();

           
        }
        public static async Task linkNetworkAI()
        {
            isMonitoring= true;
            isLinkEnabled = true;
            while (isMonitoring)
            {
                Debug.WriteLine("Listening");
                if (isLinkEnabled)
                {
                    if(linkNetwork != null)
                    {
                        Debug.WriteLine("Currently Listening");
                        await linkNetwork.LinkStream();
                        Debug.WriteLine("Stopped Listening");
                    }
                }
                await Task.Delay(6000);
            }
        }
        public static void SaveRPCProvider(string value)
        {
            try
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["RPC_provider"] = value;
            }
            catch
            {

            }
        }
        public static void LoadRPCProvider()
        {
            try
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                String rpc = localSettings.Values["RPC_provider"] as string;
                WalletRPCprovider = rpc;
            }
            catch
            {

            }
        }
    }
 
}
