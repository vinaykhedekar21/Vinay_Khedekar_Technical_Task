namespace MathExpressionEvaluator
{
    /*List of types of Tokens*/
    public enum TokenType
    {
        None,
        Integer,
        Double,
        Symbol,
        String
    }

    public class Token
    {
        protected string value;

        public TokenType Type { get; protected set; }

        public Token()
        {
            Type = TokenType.None;
            value = string.Empty;
        }

        public Token(TokenType type, string value)
        {
            Type = type;
            this.value = value;
        }

        /*Get the TokenType as string*/
        public string GetTypeAsString()
        { 
            switch (Type) 
            { 
                default:      
                    return "None";
                
                case TokenType.Integer:
                    return "Integer";

                case TokenType.Double:
                    return "Decimal";
                
                case TokenType.Symbol:
                    return "Symbol";

                case TokenType.String:
                    return "String";
            }
        }

        /*Get the token as a Integer*/
        public int GetValueAsInt()
        {
            return int.Parse(value);
        }

        /*Get the value as a string*/
        public string GetValueAsString()
        {
            return value;
        }

        /*Get the value as a double*/
        public double GetValueAsDouble()
        {
            return double.Parse(value);
        }

        public bool Equals(TokenType type, string value)
        {
            return (Type == type && this.value == value);
        }
    }
}