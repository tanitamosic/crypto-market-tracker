using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class MarketCapInfo
    {
        public DateTime Vreme { get; set; }

        public double OpenFirstCurrency { get; set; }

        public double? OpenUSDCurrency { get; set; }         // nullable

        public double HighFirstCurrency { get; set; }

        public double? HighUSDCurrency { get; set; }         // nullable

        public double LowFirstCurrency { get; set; }

        public double? LowUSDCurrency { get; set; }          // nullable

        public double CloseFirstCurrency { get; set; }

        public double? CloseUSDCurrency { get; set; }        // nullable

        public double Volume { get; set; }

        public double? MarketCap { get; set; }               // nullable

        public override string ToString()
        {
            return $"Vreme: {Vreme}\nOpen: {OpenFirstCurrency}\n" +
                $"High: {HighFirstCurrency}\nLow: {LowFirstCurrency}\n" +
                $"Close: {CloseFirstCurrency}\nVolume: {Volume}\nMarketCap: {MarketCap}";
        }
    }
}
