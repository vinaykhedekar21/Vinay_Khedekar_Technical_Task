using System.Collections.Generic;

namespace MathExpressionEvaluator
{
    using System;
    using System.Linq;

    /*The Tokenizer separates the source string into individual Tokens (Symbols/operators and integer/doubles)*/
    public class Tokenizer : TokenizerInterface
    {
        protected char[] knownSymbols = new char[] { };
        protected int    currentCharIndex = -1;
	    protected char   currentChar = '\0';
	    protected char   lastChar = '\0';
	    protected string source = string.Empty;
        protected string tokenValueBuffer = string.Empty;

        public Tokenizer()
        {
        }

        public Tokenizer(string source, char[] knownSymbols)
        {
            Initialize(source, knownSymbols);
        }

        /*Initializes the Tokenizer with the source string and symbols*/
        public TokenizerInterface Initialize(string source, char[] knownSymbols)
        {
            this.knownSymbols = new char[] { };
            currentCharIndex = -1;
            currentChar = '\0';
            lastChar = '\0';
            this.source = string.Empty;
            tokenValueBuffer = string.Empty;

            if (source.Length == 0)
                throw new Exception("Empty source!");
            if (knownSymbols.Length == 0)
                throw new Exception("Empty symbols!");

            this.source = source;
            this.knownSymbols = knownSymbols;
            tokenValueBuffer = string.Empty;
            ReadNextChar();

            return this;
        }

        /*Read the next available Token*/
        public Token GetNextToken()
        {
	        tokenValueBuffer = string.Empty;
	        SkipWhiteSpace();

	        if (EoF())
		        return new Token();

            if (IsDigit(currentChar))
                return ReadDigits();

            // If current char is period(.) in case of optional decimal point.
            if (currentChar == '.')
                return ReadOptionalDecimal();

            return ReadSymbol();
        }

        /*Reads all available Tokens*/
        public Token[] GetAllTokens()
        {
            Token currentToken = GetNextToken();
            if (currentToken.Type == TokenType.None)
                return new Token[] {};

            List<Token> allTokens = new List<Token>();
            while (currentToken.Type != TokenType.None)
            {
                allTokens.Add(currentToken);
                currentToken = GetNextToken();
            }

            return allTokens.ToArray();
        }

        protected void ReadNextChar()
        {
            lastChar = currentChar;
            currentChar = source.Length > ++currentCharIndex ? source[currentCharIndex] : '\0';

            if (source.Length <= 0)
		        currentChar = '\0';
        }

        protected void SkipWhiteSpace()
        {
	        while (currentChar > '\0' && currentChar <= ' ')
		        ReadNextChar();
        }

        protected void StoreAndReadNext()
        {
	        tokenValueBuffer += currentChar;
	        ReadNextChar();
        }

        protected string GetTokenValue()
        {
	        return tokenValueBuffer; 
        }

        protected bool EoF()
        {
	        return (currentChar == '\0');
        }

        protected void CheckUnexpectedEoF()
        {
	        if (EoF())
		        throw new Exception("Unexpected end of source!");
        }

        protected void ThrowInvalidCharException()
        {
	        string msg = string.Empty;
	        if (tokenValueBuffer.Length > 0)
                msg = string.Format("Invalid character {0} after {1} at position '{2}'.", currentChar, tokenValueBuffer, currentCharIndex + 1);
	        else
                msg = string.Format("Invalid character {0} at position '{1}'.", currentChar, currentCharIndex + 1);

	        throw new Exception(msg);
        }

        protected bool IsDigit(char character)
        {
            return (character >= '0' && character <= '9');
        }

        protected bool IsNumericSeparator(char character)
        {
            return character == '.' || character == ',';
        }

        protected bool IsKnownSymbol(char character)
        {
            return knownSymbols.Contains(currentChar);
        }

        protected Token ReadDigits()
        {
            do
            {
                StoreAndReadNext();
            }
            while (IsDigit(currentChar) || (IsNumericSeparator(currentChar) && !IsNumericSeparator(lastChar)));

            var value = GetTokenValue();
            if (value.Contains(".") || value.Contains(","))
                return new Token(TokenType.Double, GetTokenValue());
            else
                return new Token(TokenType.Integer, GetTokenValue());
        }

        //Function for handling case# optional decimal point [period(.) in the begining]
        protected Token ReadOptionalDecimal()
        {
            do
            {
                StoreAndReadNext();
            }
            while (IsDigit(currentChar) || (IsNumericSeparator(currentChar) && !IsNumericSeparator(lastChar)));
            //Append 0 (Zero) to for a complete decimal digit 
            var value = 0+GetTokenValue();
            if (value.Contains("."))
                return new Token(TokenType.Double, GetTokenValue());
            else
                return new Token(TokenType.Integer, GetTokenValue());
        }

        protected Token ReadSymbol()
        {
            if (IsKnownSymbol(currentChar))
            {
                StoreAndReadNext();
                return new Token(TokenType.Symbol, GetTokenValue());
            }

            throw new Exception(string.Format("Unknown symbol '{0}' at position '{1}'.", currentChar, currentCharIndex + 1));
        }

        protected Token ReadString()
        {
            while (!EoF() && currentChar != ' ' && !IsKnownSymbol(currentChar))
                StoreAndReadNext();

            return new Token(TokenType.String, GetTokenValue());
        }
    }
}
