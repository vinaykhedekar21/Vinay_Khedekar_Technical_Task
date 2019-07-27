namespace MathExpressionEvaluator
{
    public interface TokenizerInterface
    {
        /*Initializes the Tokenizer*/
        TokenizerInterface Initialize(string source, char[] knownSymbols);

        /*Read the next available Token*/
        Token GetNextToken();

        /* Reads all available Tokens (from the current position)*/
        Token[] GetAllTokens();
    }
}