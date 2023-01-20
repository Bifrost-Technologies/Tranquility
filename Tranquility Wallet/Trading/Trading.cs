using Solnet.Serum.Models;
using Solnet.Serum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solnet.Rpc;
using Solnet.Wallet;
using Solnet.Programs;
using System.Diagnostics;
using Solnet.Rpc.Models;

namespace Tranquility.Trading
{
    public static class OpenBookAPI
    {
        

        public static async Task<Market> GetMarket(string marketAddress)
        {
            var _serumClient = Solnet.Serum.ClientFactory.GetClient("https://cold-smart-friday.solana-mainnet.discover.quiknode.pro/72b2abc7eb8dd76a205f25f0144e3d992d5b134b/");

            Market market = await _serumClient.GetMarketAsync(marketAddress);

            return market;
        }
        public static async Task<List<MarketInfo>> GetMarkets()
        {
            var _serumClient = Solnet.Serum.ClientFactory.GetClient("https://cold-smart-friday.solana-mainnet.discover.quiknode.pro/72b2abc7eb8dd76a205f25f0144e3d992d5b134b/");

            var markets = await _serumClient.GetMarketsAsync();

            return markets.ToList();
        }
        public static async Task<List<TokenInfo>> GetTokens()
        {
            
            var _serumClient = Solnet.Serum.ClientFactory.GetClient("https://cold-smart-friday.solana-mainnet.discover.quiknode.pro/72b2abc7eb8dd76a205f25f0144e3d992d5b134b/");

            var markets = await _serumClient.GetTokensAsync();

            return markets.ToList();
        }
        public static async Task<MarketManager> PlaceOrder(string marketAddress, Order _order)
        {
            var _serumClient = Solnet.Serum.ClientFactory.GetClient(Cluster.MainNet);
            await _serumClient.ConnectAsync();

            // initialize market manager
            var _marketManager = MarketFactory.GetMarket(new PublicKey(marketAddress), serumClient: _serumClient);
           await _marketManager.InitAsync();

            return (MarketManager)_marketManager;
        }
        public static async Task<KeyValuePair<List<OpenOrder>, List<OpenOrder>>> GetOrderBookData(string marketAddress)
        {
            var _serumClient = Solnet.Serum.ClientFactory.GetClient("https://cold-smart-friday.solana-mainnet.discover.quiknode.pro/72b2abc7eb8dd76a205f25f0144e3d992d5b134b/");

            Market market = await _serumClient.GetMarketAsync(marketAddress);

            Console.WriteLine($"Market:: Own Address: {market.OwnAddress.Key}\n" +
                              $"Base Mint: {market.BaseMint.Key} Quote Mint: {market.QuoteMint.Key}\n" +
                              $"Bids: {market.Bids.Key} Asks: {market.Asks.Key}");


            OrderBookSide bidOrderBookSide = await _serumClient.GetOrderBookSideAsync(market.Bids.Key);
            Console.WriteLine($"BidOrderBook:: SlabNodes: {bidOrderBookSide.Slab.Nodes.Count}");

            OrderBookSide askOrderBookSide = await _serumClient.GetOrderBookSideAsync(market.Asks.Key);
            Console.WriteLine($"AskOrderBook:: SlabNodes: {askOrderBookSide.Slab.Nodes.Count}");
            List<OpenOrder> askOrders = askOrderBookSide.GetOrders();
            askOrders.Sort(Comparer<OpenOrder>.Create((order, order1) => order.RawPrice.CompareTo(order1.RawPrice)));

            List<OpenOrder> bidOrders = bidOrderBookSide.GetOrders();
            bidOrders.Sort(Comparer<OpenOrder>.Create((order, order1) => order1.RawPrice.CompareTo(order.RawPrice)));

            KeyValuePair<List<OpenOrder>, List<OpenOrder>> OrderBook = new KeyValuePair<List<OpenOrder>, List<OpenOrder>>(key: bidOrders, value: askOrders);

            return OrderBook;
        }
    }
}
