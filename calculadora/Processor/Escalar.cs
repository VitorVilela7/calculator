using Calculadora.Processor.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculadora.Processor
{
    internal class Escalar : ICalculable
    {
        private readonly decimal value;

        private Escalar(decimal value)
        {
            this.value = value;
        }

        public decimal GetResult()
        {
            return value;
        }

        public string GetText()
        {
            return value.ToString();
        }

        public static Escalar Of(decimal value)
        {
            return new Escalar(value);
        }

        public static Escalar Of(String value)
        {
            try
            {
                return Of(Decimal.Parse(value));
            }
            catch
            {
                throw new ParseException($"'{value}' não é um número válido.");
            }
        }
    }
}
