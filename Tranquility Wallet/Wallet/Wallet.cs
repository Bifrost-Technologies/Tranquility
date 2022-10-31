using Solnet.Extensions;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Bip39;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
namespace Tranquility.Wallets
{
    public static class SolanaWallet
    {
        public static async void CreateNewWallet(string mnemomicPassword)
        {
            try
            {
                Wallet newWallet = new Wallet(WordCount.TwentyFour, wordList: WordList.English, mnemomicPassword);
                IBuffer _encryptedKey = await Security.DataProtection.ProtectAsync(newWallet.Mnemonic.ToString());
                string pwHASH = Security.SHA512.Hash(mnemomicPassword);
                IBuffer _encryptedHashedPW = await Security.DataProtection.ProtectAsync(pwHASH);
                IBuffer _encryptedPW = await Security.DataProtection.ProtectAsync(mnemomicPassword);
                Core.Runtime.SolanaVault = new Storage.SolanaVault();
                Core.Runtime.SolanaVault.HashedProtectedKey = _encryptedHashedPW;
                Core.Runtime.SolanaVault.ProtectedSessionKey = _encryptedPW;
                Core.Runtime.SolanaVault.Wallet = _encryptedKey;
                Core.Runtime.SelectedAccount = 0;
                Core.Runtime.SolanaVault.SaveWalletStorage();
                Core.Runtime.SolanaVault.SaveVaultKey();
                Debug.WriteLine("Wallet Created Successfully!");
            }
            catch
            {

            }
        }

        public static async void ImportWallet(string mnemonic, string mnemonicPassword)
        {
            try
            {
                IBuffer _encryptedKey = await Security.DataProtection.ProtectAsync(mnemonic);
                string pwHASH = Security.SHA512.Hash(mnemonicPassword);
                IBuffer _encryptedHashedPW = await Security.DataProtection.ProtectAsync(pwHASH);
                IBuffer _encryptedPW = await Security.DataProtection.ProtectAsync(mnemonicPassword);
                Core.Runtime.SolanaVault = new Storage.SolanaVault();
                Core.Runtime.SolanaVault.HashedProtectedKey = _encryptedHashedPW;
                Core.Runtime.SolanaVault.ProtectedSessionKey = _encryptedPW;
                Core.Runtime.SolanaVault.Wallet = _encryptedKey;
                Core.Runtime.SelectedAccount = 0;
                Core.Runtime.SolanaVault.SaveWalletStorage();
                Core.Runtime.SolanaVault.SaveVaultKey();
            }
            catch
            {
              
            }
        }

        public static async void SendSOL(string recipientAddress, decimal amount)
        {
            try
            {
                var m = await Security.DataProtection.UnprotectData(Core.Runtime.SolanaVault.Wallet);
                var k = await Security.DataProtection.UnprotectData(Core.Runtime.SolanaVault.ProtectedSessionKey);
                m = null; k = null;
                Wallet wallet = new Wallet(mnemonic: new Mnemonic(m), passphrase: k);
                Account ActiveAcc = wallet.GetAccount(Core.Runtime.SolanaVault.WalletIndexChart[Core.Runtime.SelectedAccount]);
                wallet = null;
                var rpcClient = ClientFactory.GetClient(Core.Runtime.WalletRPCprovider);
                var recipient = new PublicKey(recipientAddress);
                Account walletOwner = ActiveAcc;
                RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

                byte[] transferTx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(walletOwner).
                AddInstruction(TokenProgram.Transfer(walletOwner, recipient, SolHelper.ConvertToLamports(amount), walletOwner)).
                Build(new List<Account> { walletOwner });
                var tx = await rpcClient.SendTransactionAsync(transferTx);
            }
            catch
            {

            }
        }

        public static async void SendTokens(string recipientAddress, string mintAddress, decimal amount)
        {
            try
            {
                var m = await Security.DataProtection.UnprotectData(Core.Runtime.SolanaVault.Wallet);
                var k = await Security.DataProtection.UnprotectData(Core.Runtime.SolanaVault.ProtectedSessionKey);
                Wallet wallet = new Wallet(mnemonic: new Mnemonic(m), passphrase: k);
                m = null; k = null;
                Account ActiveAcc = wallet.GetAccount(Core.Runtime.SolanaVault.WalletIndexChart[Core.Runtime.SelectedAccount]);
                wallet = null;

                var rpcClient = ClientFactory.GetClient(Core.Runtime.WalletRPCprovider);

                PublicKey mint = new PublicKey(mintAddress);
                PublicKey recipient = new PublicKey(recipientAddress);
                Account walletOwner = ActiveAcc;

                RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

                byte[] transferTx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(walletOwner).
                AddInstruction(AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(walletOwner, recipient, mint)).
                AddInstruction(TokenProgram.Transfer(AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(walletOwner, mint), AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(recipient, mint), SolHelper.ConvertToLamports(amount), walletOwner)).
                Build(new List<Account> { walletOwner });

                var tx = await rpcClient.SendTransactionAsync(transferTx);
            }
            catch
            {

            }
        }

        public static async Task RetrieveActiveAccountData()
        {
            try
            {
                var m = await Security.DataProtection.UnprotectData(Core.Runtime.SolanaVault.Wallet);
                var k = await Security.DataProtection.UnprotectData(Core.Runtime.SolanaVault.ProtectedSessionKey);
                Wallet _wallet = new Wallet(mnemonic: new Mnemonic(m), passphrase: k);
                Account ActiveAcc = _wallet.GetAccount(Core.Runtime.SolanaVault.WalletIndexChart[Core.Runtime.SelectedAccount]);
                string _walletaddress = ActiveAcc.PublicKey;
                m = null; k = null; _wallet = null; ActiveAcc = null;
                var RPC_client = ClientFactory.GetClient(url: Core.Runtime.WalletRPCprovider);
                Core.Runtime.tokenWallet = await TokenWallet.LoadAsync(RPC_client, Core.Runtime.tokenMintDatabase, new PublicKey(key: _walletaddress));
                Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Balance = Core.Runtime.tokenWallet.Sol.ToString();
            }
            catch
            {

            }
        }
        public static async Task GenerateActiveAccounts()
        {
            try
            {
                var m = await Security.DataProtection.UnprotectData(Core.Runtime.SolanaVault.Wallet);
                var k = await Security.DataProtection.UnprotectData(Core.Runtime.SolanaVault.ProtectedSessionKey);
                Wallet _wallet = new Wallet(mnemonic: new Mnemonic(m), passphrase: k);
                m = null; k = null;
                Account ActiveAcc = _wallet.GetAccount(0);
                string _walletaddress = ActiveAcc.PublicKey;
                ActiveAcc = null;
                var RPC_client = ClientFactory.GetClient(url: Core.Runtime.WalletRPCprovider);
                Core.Runtime.tokenWallet = await TokenWallet.LoadAsync(RPC_client, Core.Runtime.tokenMintDatabase, new PublicKey(key: _walletaddress));
                var _mainAccount = new ActiveAccount
                {
                    Address = _walletaddress,
                    WalletIndex = 0,
                    Balance = Core.Runtime.tokenWallet.Sol.ToString()
                };

                Core.Runtime.SolanaVault.ActiveAccounts = new List<ActiveAccount>();
                Core.Runtime.SolanaVault.ActiveAccounts.Add(_mainAccount);
                int count = 0;
                if (Core.Runtime.SolanaVault.WalletIndexChart != null)
                {
                    foreach (var index in Core.Runtime.SolanaVault.WalletIndexChart)
                    {
                        if (index != 0)
                        {
                            ActiveAcc = _wallet.GetAccount(index);
                            _walletaddress = ActiveAcc.PublicKey;
                            var _subAccount = new ActiveAccount
                            {
                                Address = _walletaddress,
                                WalletIndex = count,
                                Balance = "0"
                            };
                            Core.Runtime.SolanaVault.ActiveAccounts.Add(_subAccount);
                        }
                        count++;
                    }
                }
                else
                {
                    Core.Runtime.SolanaVault.WalletIndexChart = new List<int>();
                    Core.Runtime.SolanaVault.WalletIndexChart.Add(0);
                    Core.Runtime.SolanaVault.SaveWalletIndex();

                }

                _wallet = null; ActiveAcc = null;

            }
            catch
            {

            }
        }
    }
    public class ActiveAccount
    {
        public int WalletIndex { get; set; }
        public string Address { get; set; }
        public string Balance { get; set; }

        public bool isImported { get; set; }
    }
}
