using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionEvaluator
{
    /*Main application file to start executing the program*/
    static class MathExpressionEvaluatorApp
    {
        public static void Main(string[] args)
        {
            String expression;
            Console.Write("Enter a Math Expression To Evaluate: ");
            expression = Console.ReadLine();
            try
            { 
                //Pass on the input experession to the Infix to Postfix converter
                InfixToPostfixConverter converter = new InfixToPostfixConverter(new Tokenizer());
                var postfix = converter.Convert(expression);
    
                //Calculate the final result by evaluating postfix expression
                PostfixToFinalResult eva = new PostfixToFinalResult();
                var result = eva.Evaluate(postfix);

                //print the result
                Console.WriteLine("Final Result: {0}", result);
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}

