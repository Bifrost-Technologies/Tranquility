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

namespace Tranquility.Core
{
    public static class Runtime
    {
        public static Storage.SolanaVault SolanaVault { get; set; }
        public static int SelectedNavItem { get; set; }
        public static bool overviewInitialized { get; set; }
        public static List<MetadataAccount> Inventory { get; set; }
        public static string WalletRPCprovider { get; set; }
        public static TokenWallet tokenWallet { get; set; }
        public static bool SuccessfullyLoaded { get; set; }
        public static bool SuccessfullyLoadedAV { get; set; }
        public static bool SuccessfullyLoadedWI { get; set; }
        public static TokenMintResolver tokenMintDatabase { get; set; }
        public static int SelectedAccount { get; set; }
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
