using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathParserTK
{
    #region License agreement

    // This is light, fast and simple to understand mathematical parser 
    // designed in one class, which receives as input a mathematical 
    // expression (System.String) and returns the output value (System.Double). 
    // For example, if you input string is "√(625)+25*(3/3)" then parser return double value — 50. 
    // Copyright (C) 2012-2013 Yerzhan Kalzhani, kirnbas@gmail.com

    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.

    // This program is distributed in the hope that it will be useful,
    // but WITHOUT ANY WARRANTY; without even the implied warranty of
    // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    // GNU General Public License for more details.

    // You should have received a copy of the GNU General Public License
    // along with this program.  If not, see <http://www.gnu.org/licenses/> 

    #endregion

    #region Example usage

    // public static void Main()
    // {     
    //     MathParser parser = new MathParser();
    //     string s1 = "pi+5*5+5*3-5*5-5*3+1E1";
    //     string s2 = "sin(cos(tg(sh(ch(th(100))))))";
    //     bool isRadians = false;
    //     double d1 = parser.Parse(s1, isRadians);
    //     double d2 = parser.Parse(s2, isRadians);
    //
    //     Console.WriteLine(d1); // 13.141592...
    //     Console.WriteLine(d2); // 0.0174524023974442
    //     Console.ReadKey(true); 
    // }   

    #endregion

    #region Common tips to extend this math parser

    // If you want to add new operator, then add new item to supportedOperators
    // dictionary and check next methods:
    // (1) IsRightAssociated, (2) GetPriority, (3) SyntaxAnalysisRPN, (4) NumberOfArguments
    //
    // If you want to add new function, then add new item to supportedFunctions
    // dictionary and check next above methods: (2), (3), (4).
    // 
    // if you want to add new constant, then add new item to supportedConstants

    #endregion

    public class MathParser
    {
        #region Fields

        #region Markers (each marker should have length equals to 1)

        private const string NumberMaker = "#";
        private const string OperatorMarker = "$";
        private const string FunctionMarker = "@";

        #endregion

        #region Internal tokens

        private const string Plus = MathParser.OperatorMarker + "+";
        private const string UnPlus = MathParser.OperatorMarker + "un+";
        private const string Minus = MathParser.OperatorMarker + "-";
        private const string UnMinus = MathParser.OperatorMarker + "un-";
        private const string Multiply = MathParser.OperatorMarker + "*";
        private const string Divide = MathParser.OperatorMarker + "/";
        private const string Degree = MathParser.OperatorMarker + "^";
        private const string LeftParent = MathParser.OperatorMarker + "(";
        private const string RightParent = MathParser.OperatorMarker + ")";
        private const string Sqrt = MathParser.FunctionMarker + "sqrt";
        private const string Sin = MathParser.FunctionMarker + "sin";
        private const string Cos = MathParser.FunctionMarker + "cos";
        private const string Tg = MathParser.FunctionMarker + "tg";
        private const string Ctg = MathParser.FunctionMarker + "ctg";
        private const string Sh = MathParser.FunctionMarker + "sh";
        private const string Ch = MathParser.FunctionMarker + "ch";
        private const string Th = MathParser.FunctionMarker + "th";
        private const string Log = MathParser.FunctionMarker + "log";
        private const string Ln = MathParser.FunctionMarker + "ln";
        private const string Exp = MathParser.FunctionMarker + "exp";
        private const string Abs = MathParser.FunctionMarker + "abs";

        #endregion

        #region Dictionaries (containts supported input tokens, exclude number)

        // Key -> supported input token, Value -> internal token or number

        /// <summary>
        /// Contains supported operators
        /// </summary>
        private readonly Dictionary<string, string> supportedOperators =
            new Dictionary<string, string>
            {
                { "+", MathParser.Plus },                
                { "-", MathParser.Minus },
                { "*", MathParser.Multiply },
                { "/", MathParser.Divide },
                { "^", MathParser.Degree },
                { "(", MathParser.LeftParent },
                { ")", MathParser.RightParent }
            };

        /// <summary>
        /// Contains supported functions
        /// </summary>
        private readonly Dictionary<string, string> supportedFunctions =
            new Dictionary<string, string>
            {
                { "sqrt", MathParser.Sqrt },
                { "√", MathParser.Sqrt },
                { "sin", MathParser.Sin },
                { "cos", MathParser.Cos },
                { "tg", MathParser.Tg },
                { "ctg", MathParser.Ctg },
                { "sh", MathParser.Sh },
                { "ch", MathParser.Ch },
                { "th", MathParser.Th },
                { "log", MathParser.Log },
                { "exp", MathParser.Exp },
                { "abs", MathParser.Abs }
            };

        private readonly Dictionary<string, string> supportedConstants =
            new Dictionary<string, string>
            {
                {"pi", MathParser.NumberMaker +  Math.PI.ToString() },
                {"e", MathParser.NumberMaker + Math.E.ToString() }
            };

        #endregion

        #endregion

        private readonly char decimalSeparator;
        private bool isRadians;

        #region Constructors

        /// <summary>
        /// Initialize new instance of MathParser
        /// (symbol of decimal separator is read
        /// from regional settings in system)
        /// </summary>
        public MathParser()
        {
            try
            {
                this.decimalSeparator = Char.Parse(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Error: can't read char decimal separator from system, check your regional settings.", ex);
            }
        }

        /// <summary>
        /// Initialize new instance of MathParser
        /// </summary>
        /// <param name="decimalSeparator">Set decimal separator</param>
        public MathParser(char decimalSeparator)
        {
            this.decimalSeparator = decimalSeparator;
        }

        #endregion

        /// <summary>
        /// Produce result of the given math expression
        /// </summary>
        /// <param name="expression">Math expression (infix/standard notation)</param>
        /// <returns>Result</returns>
        public double Parse(string expression, bool isRadians = true)
        {
            this.isRadians = isRadians;

            try
            {
                return Calculate(ConvertToRPN(FormatString(expression)));
            }
            catch (DivideByZeroException e)
            {
                throw e;
            }
            catch (FormatException e)
            {
                throw e;
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Produce formatted string by the given string
        /// </summary>
        /// <param name="expression">Unformatted math expression</param>
        /// <returns>Formatted math expression</returns>
        private string FormatString(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("Expression is null or empty");
            }

            StringBuilder formattedString = new StringBuilder();
            int balanceOfParenth = 0; // Check number of parenthesis

            // Format string in one iteration and check number of parenthesis
            // (this function do 2 tasks because performance priority)
            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];

                if (ch == '(')
                {
                    balanceOfParenth++;
                }
                else if (ch == ')')
                {
                    balanceOfParenth--;
                }

                if (Char.IsWhiteSpace(ch))
                {
                    continue;
                }
                else if (Char.IsUpper(ch))
                {
                    formattedString.Append(Char.ToLower(ch));
                }
                else
                {
                    formattedString.Append(ch);
                }
            }

            if (balanceOfParenth != 0)
            {
                throw new FormatException("Number of left and right parenthesis is not equal");
            }

            return formattedString.ToString();
        }

        #region Convert to Reverse-Polish Notation

        /// <summary>
        /// Produce math expression in reverse polish notation
        /// by the given string
        /// </summary>
        /// <param name="expression">Math expression in infix notation</param>
        /// <returns>Math expression in postfix notation (RPN)</returns>
        private string ConvertToRPN(string expression)
        {
            int pos = 0; // Current position of lexical analysis
            StringBuilder outputString = new StringBuilder();
            Stack<string> stack = new Stack<string>();

            // While there is unhandled char in expression
            while (pos < expression.Length)
            {
                string token = LexicalAnalysisInfixNotation(expression, ref pos);

                outputString = SyntaxAnalysisInfixNotation(token, outputString, stack);
            }

            // Pop all elements from stack to output string            
            while (stack.Count > 0)
            {
                // There should be only operators
                if (stack.Peek()[0] == MathParser.OperatorMarker[0])
                {
                    outputString.Append(stack.Pop());
                }
                else
                {
                    throw new FormatException("Format exception,"
                    + " there is function without parenthesis");
                }
            }

            return outputString.ToString();
        }

        /// <summary>
        /// Produce token by the given math expression
        /// </summary>
        /// <param name="expression">Math expression in infix notation</param>
        /// <param name="pos">Current position in string for lexical analysis</param>
        /// <returns>Token</returns>
        private string LexicalAnalysisInfixNotation(string expression, ref int pos)
        {
            // Receive first char
            StringBuilder token = new StringBuilder();
            token.Append(expression[pos]);

            // If it is a operator
            if (this.supportedOperators.ContainsKey(token.ToString()))
            {
                // Determine it is unary or binary operator
                bool isUnary = pos == 0 || expression[pos - 1] == '(';
                pos++;

                switch (token.ToString())
                {
                    case "+":
                        return isUnary ? MathParser.UnPlus : MathParser.Plus;
                    case "-":
                        return isUnary ? MathParser.UnMinus : MathParser.Minus;
                    default:
                        return this.supportedOperators[token.ToString()];
                }
            }
            else if (Char.IsLetter(token[0])
                || this.supportedFunctions.ContainsKey(token.ToString())
                || this.supportedConstants.ContainsKey(token.ToString()))
            {
                // Read function or constant name

                while (++pos < expression.Length
                    && Char.IsLetter(expression[pos]))
                {
                    token.Append(expression[pos]);
                }

                if (this.supportedFunctions.ContainsKey(token.ToString()))
                {
                    return this.supportedFunctions[token.ToString()];
                }
                else if (this.supportedConstants.ContainsKey(token.ToString()))
                {
                    return this.supportedConstants[token.ToString()];
                }
                else
                {
                    throw new ArgumentException("Unknown token");
                }

            }
            else if (Char.IsDigit(token[0]) || token[0] == this.decimalSeparator)
            {
                // Read number

                // Read the whole part of number
                if (Char.IsDigit(token[0]))
                {
                    while (++pos < expression.Length
                    && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos]);
                    }
                }
                else
                {
                    // Because system decimal separator
                    // will be added below
                    token.Clear();
                }

                // Read the fractional part of number
                if (pos < expression.Length
                    && expression[pos] == this.decimalSeparator)
                {
                    // Add current system specific decimal separator
                    token.Append(CultureInfo.CurrentCulture
                        .NumberFormat.NumberDecimalSeparator);

                    while (++pos < expression.Length
                    && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos]);
                    }
                }

                // Read scientific notation (suffix)
                if (pos + 1 < expression.Length && expression[pos] == 'e'
                    && (Char.IsDigit(expression[pos + 1])
                        || (pos + 2 < expression.Length
                            && (expression[pos + 1] == '+'
                                || expression[pos + 1] == '-')
                            && Char.IsDigit(expression[pos + 2]))))
                {
                    token.Append(expression[pos++]); // e

                    if (expression[pos] == '+' || expression[pos] == '-')
                        token.Append(expression[pos++]); // sign

                    while (pos < expression.Length
                        && Char.IsDigit(expression[pos]))
                    {
                        token.Append(expression[pos++]); // power
                    }

                    // Convert number from scientific notation to decimal notation
                    return MathParser.NumberMaker + Convert.ToDouble(token.ToString());
                }

                return MathParser.NumberMaker + token.ToString();
            }
            else
            {
                throw new ArgumentException("Unknown token in expression");
            }
        }

        /// <summary>
        /// Syntax analysis of infix notation
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="outputString">Output string (math expression in RPN)</param>
        /// <param name="stack">Stack which contains operators (or functions)</param>
        /// <returns>Output string (math expression in RPN)</returns>
        private StringBuilder SyntaxAnalysisInfixNotation(string token, StringBuilder outputString, Stack<string> stack)
        {
            // If it's a number just put to string            
            if (token[0] == MathParser.NumberMaker[0])
            {
                outputString.Append(token);
            }
            else if (token[0] == MathParser.FunctionMarker[0])
            {
                // if it's a function push to stack
                stack.Push(token);
            }
            else if (token == MathParser.LeftParent)
            {
                // If its '(' push to stack
                stack.Push(token);
            }
            else if (token == MathParser.RightParent)
            {
                // If its ')' pop elements from stack to output string
                // until find the ')'

                string elem;
                while ((elem = stack.Pop()) != MathParser.LeftParent)
                {
                    outputString.Append(elem);
                }

                // if after this a function is in the peek of stack then put it to string
                if (stack.Count > 0 &&
                    stack.Peek()[0] == MathParser.FunctionMarker[0])
                {
                    outputString.Append(stack.Pop());
                }
            }
            else
            {
                // While priority of elements at peek of stack >= (>) token's priority
                // put these elements to output string
                while (stack.Count > 0 &&
                    Priority(token, stack.Peek()))
                {
                    outputString.Append(stack.Pop());
                }

                stack.Push(token);
            }

            return outputString;
        }

        /// <summary>
        /// Is priority of token less (or equal) to priority of p
        /// </summary>
        private bool Priority(string token, string p)
        {
            return IsRightAssociated(token) ?
                GetPriority(token) < GetPriority(p) :
                GetPriority(token) <= GetPriority(p);
        }

        /// <summary>
        /// Is right associated operator
        /// </summary>
        private bool IsRightAssociated(string token)
        {
            return token == MathParser.Degree;
        }

        /// <summary>
        /// Get priority of operator
        /// </summary>
        private int GetPriority(string token)
        {
            switch (token)
            {
                case MathParser.LeftParent:
                    return 0;
                case MathParser.Plus:
                case MathParser.Minus:
                    return 2;
                case MathParser.UnPlus:
                case MathParser.UnMinus:
                    return 6;
                case MathParser.Multiply:
                case MathParser.Divide:
                    return 4;
                case MathParser.Degree:
                case MathParser.Sqrt:
                    return 8;
                case MathParser.Sin:
                case MathParser.Cos:
                case MathParser.Tg:
                case MathParser.Ctg:
                case MathParser.Sh:
                case MathParser.Ch:
                case MathParser.Th:
                case MathParser.Log:
                case MathParser.Ln:
                case MathParser.Exp:
                case MathParser.Abs:
                    return 10;
                default:
                    throw new ArgumentException("Unknown operator");
            }
        }

        #endregion

        #region Calculate expression in RPN

        /// <summary>
        /// Calculate expression in reverse-polish notation
        /// </summary>
        /// <param name="expression">Math expression in reverse-polish notation</param>
        /// <returns>Result</returns>
        private double Calculate(string expression)
        {
            int pos = 0; // Current position of lexical analysis
            var stack = new Stack<double>(); // Contains operands

            // Analyse entire expression
            while (pos < expression.Length)
            {
                string token = LexicalAnalysisRPN(expression, ref pos);

                stack = SyntaxAnalysisRPN(stack, token);
            }

            // At end of analysis in stack should be only one operand (result)
            if (stack.Count > 1)
            {
                throw new ArgumentException("Excess operand");
            }

            return stack.Pop();
        }

        /// <summary>
        /// Produce token by the given math expression
        /// </summary>
        /// <param name="expression">Math expression in reverse-polish notation</param>
        /// <param name="pos">Current position of lexical analysis</param>
        /// <returns>Token</returns>
        private string LexicalAnalysisRPN(string expression, ref int pos)
        {
            StringBuilder token = new StringBuilder();

            // Read token from marker to next marker

            token.Append(expression[pos++]);

            while (pos < expression.Length && expression[pos] != MathParser.NumberMaker[0]
                && expression[pos] != MathParser.OperatorMarker[0]
                && expression[pos] != MathParser.FunctionMarker[0])
            {
                token.Append(expression[pos++]);
            }

            return token.ToString();
        }

        /// <summary>
        /// Syntax analysis of reverse-polish notation
        /// </summary>
        /// <param name="stack">Stack which contains operands</param>
        /// <param name="token">Token</param>
        /// <returns>Stack which contains operands</returns>
        private Stack<double> SyntaxAnalysisRPN(Stack<double> stack, string token)
        {
            // if it's operand then just push it to stack
            if (token[0] == MathParser.NumberMaker[0])
            {
                stack.Push(double.Parse(token.Remove(0, 1)));
            }
            // Otherwise apply operator or function to elements in stack
            else if (NumberOfArguments(token) == 1)
            {
                double arg = stack.Pop();
                double rst;

                switch (token)
                {
                    case MathParser.UnPlus:
                        rst = arg;
                        break;
                    case MathParser.UnMinus:
                        rst = -arg;
                        break;
                    case MathParser.Sqrt:
                        rst = Math.Sqrt(arg);
                        break;
                    case MathParser.Sin:
                        rst = ApplyTrigFunction(Math.Sin, arg);
                        break;
                    case MathParser.Cos:
                        rst = ApplyTrigFunction(Math.Cos, arg);
                        break;
                    case MathParser.Tg:
                        rst = ApplyTrigFunction(Math.Tan, arg);
                        break;
                    case MathParser.Ctg:
                        rst = 1 / ApplyTrigFunction(Math.Tan, arg);
                        break;
                    case MathParser.Sh: 
                        rst = Math.Sinh(arg); 
                        break;
                    case MathParser.Ch:rst = 
                        rst = Math.Cosh(arg); 
                        break;
                    case MathParser.Th:
                        rst = Math.Tanh(arg); 
                        break;
                    case MathParser.Ln: 
                        rst = Math.Log(arg); 
                        break;
                    case MathParser.Exp: 
                        rst = Math.Exp(arg); 
                        break;
                    case MathParser.Abs: 
                        rst = Math.Abs(arg); 
                        break;
                    default:
                        throw new ArgumentException("Unknown operator");
                }

                stack.Push(rst);
            }
            else
            {
                // otherwise operator's number of arguments equals to 2

                double arg2 = stack.Pop();
                double arg1 = stack.Pop();

                double rst;

                switch (token)
                {
                    case MathParser.Plus:
                        rst = arg1 + arg2;
                        break;
                    case MathParser.Minus:
                        rst = arg1 - arg2;
                        break;
                    case MathParser.Multiply:
                        rst = arg1 * arg2;
                        break;
                    case MathParser.Divide:
                        if (arg2 == 0)
                        {
                            throw new DivideByZeroException("Second argument is zero");
                        }
                        rst = arg1 / arg2;
                        break;
                    case MathParser.Degree:
                        rst = Math.Pow(arg1, arg2);
                        break;
                    case MathParser.Log:
                        rst = Math.Log(arg2, arg1);
                        break;
                    default:
                        throw new ArgumentException("Unknown operator");
                }

                stack.Push(rst);
            }

            return stack;
        }

        /// <summary>
        /// Apply trigonometric function
        /// </summary>
        /// <param name="func">Trigonometric function</param>
        /// <param name="arg">Argument</param>
        /// <returns>Result of function</returns>
        private double ApplyTrigFunction(Func<double, double> func, double arg)
        {
            if (!this.isRadians)
            {
                arg = arg * Math.PI / 180; // Convert value to degree
            }

            return func(arg);
        }

        /// <summary>
        /// Produce number of arguments for the given operator
        /// </summary>
        private int NumberOfArguments(string token)
        {
            switch (token)
            {
                case MathParser.UnPlus:
                case MathParser.UnMinus:
                case MathParser.Sqrt:
                case MathParser.Tg:
                case MathParser.Sh:
                case MathParser.Ch:
                case MathParser.Th:
                case MathParser.Ln:
                case MathParser.Ctg:
                case MathParser.Sin:
                case MathParser.Cos:
                case MathParser.Exp:
                case MathParser.Abs:
                    return 1;
                case MathParser.Plus:
                case MathParser.Minus:
                case MathParser.Multiply:
                case MathParser.Divide:
                case MathParser.Degree:
                case MathParser.Log:
                    return 2;
                default:
                    throw new ArgumentException("Unknown operator");
            }
        }

        #endregion
    }
}