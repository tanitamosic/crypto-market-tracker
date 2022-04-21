using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DigitalCurrency
    {
        public string CurrencyCode { get; set; }

        public string CurrencyName { get; set; }

        public override string ToString()
        {
            return CurrencyCode + " , " + CurrencyName;
        }

        /// <summary>
        /// convert from string to object
        /// </summary>
        /// <param name="fromstring"> can only be of format gotten from ToString() method.</param>
        public DigitalCurrency(string fromstring)
        {
            CurrencyCode = fromstring.Split(" , ")[0];
            CurrencyName = fromstring.Split(" , ")[1];
        }
        public DigitalCurrency() { }
    }
}
