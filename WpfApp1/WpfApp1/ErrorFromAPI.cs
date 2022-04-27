using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class ErrorFromAPI : Exception
    {
        public override string Message => "API nije vratio odgovor";
    }
}
