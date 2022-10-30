using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Tranquility.Utilities
{
    public static class birdeye
    {
        public static async Task<decimal> GetPrice(string mint)
        {
            decimal price = 0;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
                var BirdEyeAPIrequest = await httpClient.GetStringAsync(new Uri("https://public-api.birdeye.so/public/price?address=" + mint));
                
                PriceData price_data = JsonConvert.DeserializeObject<PriceData>(BirdEyeAPIrequest);
                if(price_data._data != null && price_data.success == true)
                {
                    price = price_data._data.price;
                }
            }
            return price;
        }
    }

    public class PriceData
    {
        [JsonProperty("data")]
        public data _data { get; set; }

        [JsonProperty("success")]
        public bool success { get; set; }
    }
    public class data
    {
        [JsonProperty("value")]
        public decimal price { get; set; }

        [JsonProperty("updateHumanTime")]
        public DateTime timestamp { get; set; }

    }
}
