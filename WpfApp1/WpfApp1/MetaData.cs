using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class MetaData
    {
        public string Information { get; set; }

        public string DigitalCurrencyCode { get; set; }

        public string DigitalCurrencyName { get; set; }

        public string MarketCode { get; set; }

        public string MarketName { get; set; }

        public DateTime LastRefreshed { get; set; }

        public string TimeZone { get; set; }

        // atributi koji su samo za Intraday
        public string? OutputSize { get; set; }      // ovo je nullable

        public string? Interval { get; set; }        // ovo je nullable

        public override string ToString()
        {
            return $"Information: {Information}\nDigitalCurrencyCode: {DigitalCurrencyCode}\n" +
                $"MarketCode: {MarketCode}\nLastRefreshed: {LastRefreshed}";
        }
    }
}
