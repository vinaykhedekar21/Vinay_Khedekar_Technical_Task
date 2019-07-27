namespace MathExpressionEvaluator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /*InfixToPostfixConverter converts a formula from infix notation to postfix notation*/
    public class InfixToPostfixConverter
    {
      
        /* Enum: associativity of a operation*/
        public enum Associativity
        {
            Left,
            Right
        }

        private readonly Operation[] Operations = new Operation[]
                                            {
                                                new Operation('+', 2, Associativity.Left), 
                                                new Operation('-', 2, Associativity.Left),
                                                new Operation('*', 3, Associativity.Left),
                                                new Operation('/', 3, Associativity.Left),
                                                new Operation('p', 4, Associativity.Right),
                                                new Operation('m', 4, Associativity.Right)
                                            };
        private readonly List<char> knownSymbols = new List<char>() { '(', ')'};

        private readonly TokenizerInterface myTokenizer = null;

        private Stack<string> operatorStack = new Stack<string>();
        private Queue<string> output = new Queue<string>();

        public InfixToPostfixConverter(TokenizerInterface tokenizer)
        {
            myTokenizer = tokenizer;
        }

        /*Convert a formula from infix notation to postfix notation*/
        public string Convert(string infix)
        {
            operatorStack.Clear();
            output.Clear();

            knownSymbols.AddRange(Operations.Where(x => !knownSymbols.Contains(x.Symbol)).Select(x => x.Symbol).ToList());
            
            //pass input infix string and symbols to Tokenizer
            myTokenizer.Initialize(infix, knownSymbols.ToArray());

            return DoShuntingYardAlgorithm(myTokenizer).Trim();
        }
        
        protected string DoShuntingYardAlgorithm(TokenizerInterface tokenizer)
        {
            // I have referred shunting-yard algorithm https://en.wikipedia.org/wiki/Shunting-yard_algorithm 

            Token token = tokenizer.GetNextToken();
            Token lastToken = null;
            bool first = true;


            bool afterOpenParenthesis = false;
            while (token.Type != TokenType.None)
            {
                // Token is a number, add it to the output queue.
                if (IsDigit(token))
                    AddToOutput(token.GetValueAsString());

                else if (token.GetValueAsString() == ".")
                    AddToOutput(token.GetValueAsString());

                // Token is a left parenthesis "(", push it onto the stack.
                else if (token.GetValueAsString() == "(")
                    operatorStack.Push(token.GetValueAsString());

                // Token is a right parenthesis ")"
                else if (token.GetValueAsString() == ")")
                {
                    string lastOperation = operatorStack.Peek();
                    while (lastOperation != "(")
                    {
                        AddToOutput(operatorStack.Pop());
                        lastOperation = operatorStack.Peek();
                    }
                    // Pop the left parenthesis from the stack
                    operatorStack.Pop();

                    // Mark parenthesis flag to avoid false unary operation detection
                    afterOpenParenthesis = true;
                }

                // Token is an operator, o1
                else if (IsOperator(token))
                {
                    string currentOperation = token.GetValueAsString();

                    // check for negation at the beginning of the input formal.
                    if (first && currentOperation == "-")
                    {
                        currentOperation = "m";
                    }
                    // check for plus sign at the beginning of the input formal.
                    else if (first && currentOperation == "+")
                    {
                        currentOperation = "p";
                    }
                    else if (operatorStack.Count > 0)
                    {
                        string lastOperation = operatorStack.Peek();

                        // If parenthesis flag is set avoid false unary operation detection
                        if (!afterOpenParenthesis)
                        {
                            // check for negation direct after a '(' or after another operator.
                            if ((IsOperator(lastOperation) || lastOperation == "(") && currentOperation == "-" && lastOperation != "p" && lastOperation != "m" && !IsDigit(lastToken))
                            {
                                currentOperation = "m";
                            }

                            // check for plus sign direct after a '(' or after another operator.
                            if ((IsOperator(lastOperation) || lastOperation == "(") && currentOperation == "+" && lastOperation != "p" && lastOperation != "m" && !IsDigit(lastToken))
                            {
                                currentOperation = "p";
                            }
                        }

                        //while there is an operator token, lastOperation
                        while (IsOperator(lastOperation))
                        {
                            // Check precedence of operators
                            if (IsLeftAssociative(currentOperation) && LessOrEqualPrecedence(currentOperation, lastOperation) ||
                                IsRightAssociative(currentOperation) && LessPrecedence(currentOperation, lastOperation))
                            {
                             AddToOutput(operatorStack.Pop());

                                if (operatorStack.Count == 0)
                                    break;
                                else
                                    lastOperation = operatorStack.Peek();
                            }
                            else
                                break;
                        }
                    }
          
                    operatorStack.Push(currentOperation);

                    // Reset parenthesis flag.
                    afterOpenParenthesis = false;
                }
                
                lastToken = token;
                token = tokenizer.GetNextToken();
                first = false;
            }

            // If there are still operator tokens in the stack:
            while (operatorStack.Count > 0)
            { 
                // Pop the operator onto the output queue.
                AddToOutput(operatorStack.Pop());
            }

            return CreateOutput(output);
        }

        protected bool IsOperator(Token token)
        {
            return (token.Type == TokenType.Symbol && IsOperator(token.GetValueAsString()));
        }

        protected bool IsOperator(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return false;

            return Operations.Any(x => x.Symbol == symbol[0]);
        }

        protected bool IsDigit(Token token)
        {
            return token != null && (token.Type == TokenType.Integer || token.Type == TokenType.Double);
        }

        protected void AddToOutput(string text)
        {
            output.Enqueue(" ");
            output.Enqueue(text);
        }

        protected bool LessPrecedence(string currentOperationSymbol, string lastOperationSymbol)
        {
            var o1 = GetOperation(currentOperationSymbol);
            var o2 = GetOperation(lastOperationSymbol);
            if (o1 == null || o2 == null)
                return false;

            return o1.Precedence < o2.Precedence;
        }

        protected bool LessOrEqualPrecedence(string currentOperationSymbol, string lastOperationSymbol)
        {
            var o1 = GetOperation(currentOperationSymbol);
            var o2 = GetOperation(lastOperationSymbol);
            if (o1 == null || o2 == null)
                return false;

            return o1.Precedence <= o2.Precedence;
        }

        protected bool IsLeftAssociative(string operationSymbol)
        {
            var o1 = GetOperation(operationSymbol);
            if (o1 == null)
                return false;

            return o1.Associativity == Associativity.Left;
        }

        protected bool IsRightAssociative(string operationSymbol)
        {
            var o1 = GetOperation(operationSymbol);
            if (o1 == null)
                return false;

            return o1.Associativity == Associativity.Right;
        }

        protected Operation GetOperation(string operationSymbol)
        {
            return Operations.FirstOrDefault(x => x.Symbol == operationSymbol[0]);
        }

        protected string CreateOutput(Queue<string> output)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var entry in output)
                sb.Append(entry);

            return sb.ToString();
        }

        public class Operation
        {
            public char Symbol;
            public int Precedence;
            public Associativity Associativity;

            public Operation(char symbol, int precendence, Associativity associativity)
            {
                this.Symbol = symbol;
                this.Precedence = precendence;
                this.Associativity = associativity;
            }
        }
    }
}
