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
    }
}
