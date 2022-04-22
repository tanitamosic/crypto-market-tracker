using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class APIAnswer
    {

        public MetaData? metaData { get; set; }
        public List<MarketCapInfo> marketCapInfos { get; set; }

        public bool IsIntraday { get; set; }
        public string? Vrsta { get; set; }

        public void SortAnswers()
        {
            var temp = from p in marketCapInfos
                       orderby p.Timestamp
                       select p;
            marketCapInfos = temp.ToList();
        }

        public ChartValues<double> GetOpenFirstValues()
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var v in marketCapInfos)
            {
                values.Add(v.OpenFirstCurrency);
            }
            return values;
        }
        public ChartValues<double> GetClosedFirstValues()
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var v in marketCapInfos)
            {
                values.Add(v.CloseFirstCurrency);
            }
            return values;
        }
        public ChartValues<double> GetHighFirstValues()
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var v in marketCapInfos)
            {
                values.Add(v.HighFirstCurrency);
            }
            return values;
        }
        public ChartValues<double> GetLowFirstValues()
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var v in marketCapInfos)
            {
                values.Add(v.LowFirstCurrency);
            }
            return values;
        }

        //DONT call unless api answer has USD values
        public ChartValues<double> GetOpenUSDValues()
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var v in marketCapInfos)
            {
                values.Add((double)v.OpenUSDCurrency);
            }
            return values;
        }
        public ChartValues<double> GetClosedUSDValues()
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var v in marketCapInfos)
            {
                values.Add((double)v.CloseUSDCurrency);
            }
            return values;
        }
        public ChartValues<double> GetHighUSDValues()
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var v in marketCapInfos)
            {
                values.Add((double)v.HighUSDCurrency);
            }
            return values;
        }
        public ChartValues<double> GetLowUSDValues()
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (var v in marketCapInfos)
            {
                values.Add((double)v.LowUSDCurrency);
            }
            return values;
        }

    }
}
