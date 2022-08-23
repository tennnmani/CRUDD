using Microsoft.Extensions.Caching.Memory;
using MVC2.Interface;
using MVC2.Models;
using Newtonsoft.Json;
using System.Net;

namespace MVC2.Service
{
    public class ExchangeServices : IExchange
    {
        private readonly IMemoryCache _memoryCache;

        public ExchangeServices(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<List<Exchange>> callExchange()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<Exchange> exchange = new List<Exchange>();
            string apiUrl = "https://www.nrb.org.np/api/forex/v1/app-rate";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(apiUrl).Result;


            var x = _memoryCache.TryGetValue(getCacheKey(DateTime.Now.Date.ToString()), out exchange);

            

           // _memoryCache.Set<List<Exchange>>(getCacheKey(DateTime.Now.AddDays(-1).Date.ToString()), exchange, new DateTimeOffset().AddDays(1));

            if (!_memoryCache.TryGetValue(getCacheKey(DateTime.Now.Date.ToString()), out exchange))
            {
                if (response.IsSuccessStatusCode)
                {
                    exchange = JsonConvert.DeserializeObject<List<Exchange>>(await response.Content.ReadAsStringAsync());
                }
                _memoryCache.Set<List<Exchange>>(getCacheKey(DateTime.Now.Date.ToString()), exchange, TimeSpan.FromDays(1));
                _memoryCache.Remove(getCacheKey(DateTime.Now.AddDays(-1).Date.ToString()));
            }

            return exchange;
        }

        public async Task<Decimal> getRate(string fromRate, string toRate, string buySell)
        {
            List<Exchange> exchange = await callExchange();

            decimal fromPrice = 0;
            decimal toPrice = 0;


            if (fromRate == "NEP")
            {
                fromPrice = 1;
            }
            else
            {
                var data = (from e in exchange
                            where e.iso3 == fromRate
                            select new { e.buy, e.sell, e.unit }).FirstOrDefault();

                if (buySell == "B")
                    fromPrice = data.buy;
                else
                    fromPrice = data.sell;

                decimal unit = data.unit;
                if (unit > 1)
                    fromPrice /= unit;

            }

            if (toRate == "NEP")
            {
                toPrice = 1;
            }
            else
            {
                var data = (from e in exchange
                            where e.iso3 == toRate
                            select new { e.buy, e.sell, e.unit }).FirstOrDefault();

                if (buySell == "B")
                    toPrice = data.buy;
                else
                    toPrice = data.sell;

                decimal unit = data.unit;
                if (unit > 1)
                    toPrice /= unit;

            }

            //foreach (var i in exchange)
            //{
            //    if (i.iso3 == fromRate)
            //    {
            //        if (buySell == "B")
            //            fromPrice = Decimal.Parse(i.buy);
            //        else
            //            fromPrice = Decimal.Parse(i.sell);
            //        fromRat = i.unit;

            //        if (fromRat > 1)
            //        {
            //            fromPrice = fromPrice / fromRat;
            //        }
            //    }
            //    if (i.iso3 == toRate)
            //    {
            //        if (buySell != "B")
            //            toPrice = Decimal.Parse(i.buy);
            //        else
            //            toPrice = Decimal.Parse(i.sell);
            //        toRat = i.unit;

            //        if (toRat > 1)
            //        {
            //            toPrice = toPrice / toRat;
            //        }
            //    }
            //}

            return (fromPrice / toPrice);
        }
        private string getCacheKey(string date)
        {
            return $"exchangefor{date}";
        }
    }
}
