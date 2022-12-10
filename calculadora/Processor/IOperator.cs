using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculadora.Processor
{
    internal interface IOperator
    {
        ICalculable Calculate(ICalculable a, ICalculable b);
    }
}
