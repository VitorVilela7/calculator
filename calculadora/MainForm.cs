using Calculadora.Processor;
using Calculadora.Processor.Exceptions;
using System.CodeDom;

namespace Calculadora
{
    public partial class MainForm : Form
    {
        private List<ICalculable> history = new List<ICalculable>();
        private ICalculable? currentLeft;
        private ICalculable? currentRight;
        private Operators? currentOperator;
        private bool clearIfNextInputIsNumber;
        private bool resetIfNextInputIsNumber;

        private const string layoutSheet =
@"log10;log2;mod;C;DEL
x²;7;8;9;÷
x³;4;5;6;x
√x;1;2;3;-
∛x;,;0;=;+";

        public MainForm()
        {
            InitializeComponent();
            LoadCalculatorLayout(layoutSheet);
        }

        private void LoadCalculatorLayout(string layoutSheet)
        {
            LoadCalculatorLayout(layoutSheet.Replace("\r\n", "\n").Split('\n').Select(x => x.Split(';')).ToArray());
        }

        private void LoadCalculatorLayout(string[][] layout)
        {
            var baseSize = new Size(64, 64);
            var baseFont = new Font("Arial", 12);

            for (int line = 0; line < layout.Length; line++)
            {
                for (int column = 0; column < layout[line].Length; column++)
                {
                    var item = layout[line][column];

                    var button = new Button
                    {
                        Size = baseSize,
                        Font = baseFont,
                        Location = GetButtonPosition(line, column),
                        Text = item.ToString()
                    };

                    button.Click += Button_Click;
                    operatorsBox.Controls.Add(button);
                }
            }
        }

        private Point GetButtonPosition(int line, int column)
        {
            return new Point(column * 64 + 8, line * 64 + 16);
        }

        private void Button_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button != null)
            {
                try
                {
                    if (Char.IsNumber(button.Text.First()) || button.Text == ",")
                    {
                        AddNumberDigit(button.Text.First());
                    }
                    else
                    {
                        CalculatorAction(button.Text);
                    }
                }
                catch (Exception ex) when (ex is ProcessException || ex is ParseException)
                {
                    DisplayErrorMessage(ex.Message);
                    ResetNumber();
                }
            }
        }

        private void CalculatorAction(string operation)
        {
            switch (operation)
            {
                case "log10":
                    CalculateSingleOperation(Operators.Log, Escalar.Of(10));
                    break;

                case "log2":
                    CalculateSingleOperation(Operators.Log, Escalar.Of(2));
                    break;

                case "x²":
                    CalculateSingleOperation(Operators.Power, Escalar.Of(2));
                    break;

                case "x³":
                    CalculateSingleOperation(Operators.Power, Escalar.Of(3));
                    break;

                case "√x":
                    CalculateSingleOperation(Operators.Root, Escalar.Of(2));
                    break;

                case "∛x":
                    CalculateSingleOperation(Operators.Root, Escalar.Of(3));
                    break;

                case "÷":
                    CalculateDualOperation(Operators.Divide);
                    break;

                case "mod":
                    CalculateDualOperation(Operators.Mod);
                    break;

                case "x":
                    CalculateDualOperation(Operators.Multiply);
                    break;

                case "-":
                    CalculateDualOperation(Operators.Subtract);
                    break;

                case "+":
                    CalculateDualOperation(Operators.Add);
                    break;

                case "=":
                    CalculateTerminal();
                    break;

                case "DEL":
                    DeleteNumberDigit();
                    break;

                case "C":
                    ResetNumber();
                    break;

                default:
                    DisplayErrorMessage("Método não implementado");
                    break;
            }
        }

        private void CalculateSingleOperation(Operators op, ICalculable baseNumber)
        {
            if (currentLeft == null)
            {
                currentLeft = GetEscalar();
            }

            currentRight = baseNumber;
            currentOperator = op;
            CalculateResult();
        }

        private void CalculateDualOperation(Operators op)
        {
            if (currentLeft == null)
            {
                currentLeft = GetEscalar();
            }
            else if (currentOperator != null)
            {
                CalculateTerminal();
            }

            currentOperator = op;
            clearIfNextInputIsNumber = true;
            resetIfNextInputIsNumber = false;
        }

        private void CalculateTerminal()
        {
            if (currentLeft != null && currentOperator != null)
            {
                currentRight = GetEscalar();
                CalculateResult();
            }
        }

        private Escalar GetEscalar()
        {
            return Escalar.Of(numberPanelBox.Text);
        }

        private void DeleteNumberDigit()
        {
            clearIfNextInputIsNumber = false;
            resetIfNextInputIsNumber = false;

            if (numberPanelBox.Text == null)
            {
                ResetNumber();
            }

            if (numberPanelBox.Text.Length > 0)
            {
                numberPanelBox.Text = numberPanelBox.Text[..^1];
            }
        }

        private void AddNumberDigit(char number)
        {
            if (numberPanelBox.Text == null || resetIfNextInputIsNumber)
            {
                ResetNumber();
            }
            if (clearIfNextInputIsNumber)
            {
                ClearNumber();
            }

            numberPanelBox.Text += number;
        }

        private void ResetNumber()
        {
            ResetContext(null);
            ClearNumber();
        }

        private void ClearNumber()
        {
            numberPanelBox.Text = "";
            operationTextBox.Text = "";
            clearIfNextInputIsNumber = false;
            resetIfNextInputIsNumber = false;
        }

        private void SetNumber(ICalculable calculable)
        {
            SetNumber(calculable.GetResult(), calculable.GetText());
        }

        private void SetNumber(decimal value, String expression)
        {
            numberPanelBox.Text = value.ToString();
            operationTextBox.Text = expression;
            clearIfNextInputIsNumber = true;
            resetIfNextInputIsNumber = true;
        }

        private void ResetContext(ICalculable calculable)
        {
            currentLeft = calculable;
            currentOperator = null;
            currentRight = null;
        }

        private void CalculateResult()
        {
            Calculate(new MonoExpression(currentLeft, currentRight, (Operators)currentOperator));
        }

        private void Calculate(ICalculable calculable)
        {
            SetNumber(calculable);
            ResetContext(calculable);
            AddHistory(calculable);
        }

        private void AddHistory(ICalculable calculable)
        {
            history.Add(calculable);
            listBox1.Items.Add(calculable.GetText());
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var h = history[listBox1.SelectedIndex];
            SetNumber(h);
            ResetContext(h);
        }

        private void DisplayErrorMessage(String message)
        {
            MessageBox.Show(message, "Erro de calculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}