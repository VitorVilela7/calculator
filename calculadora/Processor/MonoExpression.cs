using Calculadora.Processor.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculadora.Processor
{
    internal class MonoExpression : ICalculable
    {
        public static readonly Dictionary<Operators, Func<decimal, decimal, decimal>> operators = new()
        {
            { Operators.Add, (a, b) => a + b },
            { Operators.Subtract, (a, b) => a - b },
            { Operators.Multiply, (a, b) => a * b },
            { Operators.Divide, (a, b) => a / b },
            { Operators.Mod, (a, b) => a % b },
            { Operators.Power, (a, b) => (decimal)Math.Pow((double)a, (double)b) },
            { Operators.Log, (a, b) => (decimal)Math.Log((double)a, (double)b) },
            { Operators.Root, (a, b) => (decimal)Math.Pow((double)a, (double)(1 / b)) }
        };

        public static readonly Dictionary<Operators, string> formatIdentifiers = new()
        {
            { Operators.Add, "{0}+{1}" },
            { Operators.Subtract, "{0}-{1}" },
            { Operators.Multiply, "{0}*{1}" },
            { Operators.Divide, "{0}÷{1}" },
            { Operators.Mod, "{0}%{1}" },
            { Operators.Power, "{0}^{1}" },
            { Operators.Log, "log{1}({0})" },
            { Operators.Root, "{1}√{0}" }
        };

        public MonoExpression(ICalculable a, ICalculable b, Operators operation)
        {
            A = a;
            B = b;
            Operation = operation;
        }

        public ICalculable A { get; }
        public ICalculable B { get; }
        public Operators Operation { get; }

        public string GetText()
        {
            return String.Format(formatIdentifiers[Operation], Wrap(A), Wrap(B));
        }

        private String Wrap(ICalculable calculable)
        {
            if (calculable == null)
            {
                return "";
            }

            var text = calculable.GetText();

            if (calculable is MonoExpression expression && expression.Operation != Operation)
            {
                return $"({text})";
            }
            else
            {
                return text;
            }
        }

        public decimal GetResult()
        {
            try
            {
                return operators[Operation](A.GetResult(), B.GetResult());
            }
            catch
            {
                throw new ProcessException($"Erro ao calcular '{GetText()}'");
            }
        }
    }
}