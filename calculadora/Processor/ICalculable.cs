using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculadora.Processor
{
    internal interface ICalculable
    {
        decimal GetResult();

        string GetText();
    }
}
