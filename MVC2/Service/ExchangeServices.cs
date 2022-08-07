using MVC2.Interface;
using MVC2.Models;
using Newtonsoft.Json;
using System.Net;

namespace MVC2.Service
{
    public class ExchangeServices : IExchange
    {
        public async Task<List<Exchange>> callExchange()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //Fetch the JSON string from URL.
            List<Exchange> exchange = new List<Exchange>();
            string apiUrl = "https://www.nrb.org.np/api/forex/v1/app-rate";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(apiUrl).Result;
            if (response.IsSuccessStatusCode)
            {
                exchange = JsonConvert.DeserializeObject<List<Exchange>>(await response.Content.ReadAsStringAsync());
            }
            return exchange;
        }

        public async Task<Decimal> getRate(string fromRate, string toRate, string buySell)
        {
            List<Exchange> exchange = await callExchange();

            decimal fp = 0;
            decimal fi = 0;
            decimal tp = 0;
            decimal ti = 0;

            if(fromRate == "NEP")
            {
                fp = 1;
            }
            if(toRate == "NEP")
            {
                tp = 1;
            }

            foreach (var i in exchange)
            {
                if (i.iso3 == fromRate)
                {
                    if (buySell == "B")
                        fp = Decimal.Parse(i.buy);
                    else
                        fp = Decimal.Parse(i.sell);
                    fi = i.unit;

                    if (fi > 1)
                    {
                        fp = fp / fi;
                    }
                }
                if (i.iso3 == toRate)
                {
                    if (buySell != "B")
                        tp = Decimal.Parse(i.buy);
                    else
                        tp = Decimal.Parse(i.sell);
                    ti = i.unit;

                    if (ti > 1)
                    {
                        tp = tp / ti;
                    }
                }
            }
            return (tp / fp);
        }
    }
}
