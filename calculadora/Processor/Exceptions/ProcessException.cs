using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculadora.Processor.Exceptions
{
    internal class ProcessException : Exception
    {
        public ProcessException(String message) : base(message)
        {
        }
    }
}
