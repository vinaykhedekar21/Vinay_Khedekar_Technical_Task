namespace MathExpressionEvaluator
{
    using System;
    using System.Collections.Generic;


    /* The PostfixToFinalResult evaluates the result of a formula in postfix notation. */
    public class PostfixToFinalResult
    {

        /*calculates the result of a formula in postfix notation*/
        public decimal Evaluate(string postfixString)
        {
            var stack = new Stack<decimal>();
            var input = new Queue<string>();

            foreach (var entry in postfixString.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                input.Enqueue(entry);

            while (input.Count > 0)
            {
                var entry = input.Dequeue();

                if (string.IsNullOrEmpty(entry))
                    continue;

                // If the character is an operator, add it to the stack.
                if (!IsOperator(entry))
                {
                    stack.Push(decimal.Parse(entry));
                }

                // Case# Unary Operator 
                else if (IsUnaryOperator(entry))
                {
                    var x = stack.Pop();
                    switch (entry)
                    {
                        case "m":
                            // if operator is a minus sign evaluate -1 * top-stack and push the result on the stack.
                            stack.Push(x * -1);
                            break;
                        case "p":
                            // if operator is a additional plus sign just push topstack back on the stack.
                            stack.Push(x);
                            break;
                    }
                }

                // Case# Binary operator
                else if (IsBinaryOperator(entry))
                {
                    decimal rightOperand = stack.Pop();
                    decimal leftOperand = stack.Pop();
                    switch (entry)
                    {
                        case "+":
                            stack.Push(leftOperand + rightOperand);
                            break;
                        case "-":
                            stack.Push(leftOperand - rightOperand);
                            break;
                        case "*":
                            stack.Push(leftOperand * rightOperand);
                            break;
                        case "/":
                            stack.Push(leftOperand / rightOperand);
                            break;
                    }
                }
                else
                {
                    throw new Exception("Unknown symbol!");
                }
            }
            
            // After all characters are scanned, Return topStack.
               return stack.Pop();
        }

        protected bool IsUnaryOperator(string entry)
        {
            return (entry == "m") || (entry == "p");
        }

        protected bool IsBinaryOperator(string entry)
        {
            return (entry == "+") || (entry == "-") || (entry == "*") || (entry == "/") || (entry == "^");
        }

        protected bool IsOperator(string entry)
        {
            return IsBinaryOperator(entry) || IsUnaryOperator(entry);
        }
    }
}
