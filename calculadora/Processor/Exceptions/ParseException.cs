using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculadora.Processor.Exceptions
{
    internal class ParseException : Exception
    {
        public ParseException(String message) : base(message) { 
        }
    }
}
