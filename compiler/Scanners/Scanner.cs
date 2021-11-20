using compiler.CharReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.Tokens;

namespace compiler.Scanners
{
    public class Scanner : IScanner
    {
        public Reader charReader;
        public Token token;
        public Dictionary<string, TokenType> KeywordTokens;
        public Dictionary<string, TokenType> SignsTokens;
        public Scanner(Reader charReader) 
        {
            this.charReader = charReader;
            charReader.MoveToNextChar();
            token = null;
            KeywordTokens = new Dictionary<string, TokenType>();
            SignsTokens = new Dictionary<string, TokenType>();
            CreateTokens();
        }
        void CreateTokens() 
        {
            KeywordTokens.Add("if", TokenType.IF);
            KeywordTokens.Add("else", TokenType.ELSE);
            KeywordTokens.Add("while", TokenType.WHILE);
            KeywordTokens.Add("return", TokenType.RETURN);
            KeywordTokens.Add("true", TokenType.TRUE);
            KeywordTokens.Add("false", TokenType.FALSE);
            KeywordTokens.Add("double", TokenType.DOUBLE);
            KeywordTokens.Add("int", TokenType.INT);


            SignsTokens.Add("=", TokenType.ASSIGN);
            SignsTokens.Add("+", TokenType.PLUS);
            SignsTokens.Add("-", TokenType.MINUS);
            SignsTokens.Add("*", TokenType.MULTIPLE);
            SignsTokens.Add("/", TokenType.DIVIDE);

            SignsTokens.Add("&&", TokenType.AND);
            SignsTokens.Add("||", TokenType.OR);

            SignsTokens.Add("==", TokenType.EQUAL);
            SignsTokens.Add("!=", TokenType.NOT_EQUAL);
            SignsTokens.Add(">=", TokenType.MORE_EQUAL);
            SignsTokens.Add("<=", TokenType.LESS_EQUAL);
            SignsTokens.Add(">", TokenType.MORE);
            SignsTokens.Add("<", TokenType.LESS);

            SignsTokens.Add("(", TokenType.LEFT_ROUND_BRACKET);
            SignsTokens.Add(")", TokenType.RIGHT_ROUND_BRACKET);
            SignsTokens.Add("{", TokenType.LEFT_CURLY_BRACKET);
            SignsTokens.Add("}", TokenType.RIGHT_CURLY_BRACKET);

            SignsTokens.Add(",", TokenType.COMMA);
        }
        public bool NextToken()
        {
            while (CharIsWhite(charReader.currentChar))
            {
                charReader.MoveToNextChar();
            }
            if (charReader.currentChar == (char)0)
            {
                token = null;
                return false;
            }
            (int, int) position = (charReader.currentLine, charReader.currentPositionInLine);
            if (char.IsLetter(charReader.currentChar)) 
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (char.IsLetterOrDigit(charReader.currentChar)) 
                {
                    stringBuilder.Append(charReader.currentChar);
                    charReader.MoveToNextChar();
                }
                string text = stringBuilder.ToString();
                TokenType type;
                if (KeywordTokens.TryGetValue(text, out type)) 
                {
                    token = new Token(type, text, position);
                    return true;
                }
                token = new Token(TokenType.IDENTIFIER, text, position);
                return true;
            }
            if (char.IsDigit(charReader.currentChar)) 
            {
                StringBuilder stringBuilder = new StringBuilder();
                bool isDouble = false;
                while (char.IsDigit(charReader.currentChar)) 
                {
                    stringBuilder.Append(charReader.currentChar);
                    charReader.MoveToNextChar();
                }
                if(charReader.currentChar == '.') 
                {
                    stringBuilder.Append(charReader.currentChar);
                    charReader.MoveToNextChar();
                    isDouble = true;
                }
                while (char.IsDigit(charReader.currentChar))
                {
                    stringBuilder.Append(charReader.currentChar);
                    charReader.MoveToNextChar();
                }
                if (char.IsLetter(charReader.currentChar)) 
                {
                    while (char.IsLetterOrDigit(charReader.currentChar)) 
                    {
                        stringBuilder.Append(charReader.currentChar);
                        charReader.MoveToNextChar();
                    }
                    token = new Token(TokenType.UNKNOWN, stringBuilder.ToString(), position);
                    return true;
                }
                if (isDouble) 
                {
                    token = new Token(TokenType.NUMBER_DOUBLE, stringBuilder.ToString(), position);
                }
                else 
                {
                    token = new Token(TokenType.NUMBER_INT, stringBuilder.ToString(), position);
                }
                return true;
            }
            if (CharIsBracket(charReader.currentChar))
            {
                string text = charReader.currentChar.ToString();
                TokenType type;
                if (!SignsTokens.TryGetValue(text, out type))
                {
                    throw new Exception("Invalid Dictionary");
                }

                token = new Token(type, text, position);

                charReader.MoveToNextChar();

                return true;
            }
            if (CharIsCorrectSign(charReader.currentChar)) 
            {
                if (CharIsCorrectSign(charReader.currentChar))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    while (CharIsCorrectSign(charReader.currentChar))
                    {
                        stringBuilder.Append(charReader.currentChar);
                        charReader.MoveToNextChar();
                    }
                    string text = stringBuilder.ToString();

                    TokenType type;
                    if (SignsTokens.TryGetValue(text, out type))
                    {
                        token = new Token(type, text, position);
                        return true;
                    }

                    // TODO: Commit wrong operator error to IError 

                    token = new Token(TokenType.UNKNOWN, text, position);

                    return true;
                }
            }


            token = new Token(TokenType.UNKNOWN, charReader.currentChar.ToString(), position);
            charReader.MoveToNextChar();

            // TODO: Commit incorect sign error to IError

            return false;
        }




        bool CharIsWhite(char c)
        {
            if (char.IsWhiteSpace(c) || char.IsSeparator(c) || c == '\t' || c == '\n')
            {
                return true;
            }
            return false;
        }

        bool CharIsBracket(char c)
        {
            if (c == '(' || c == ')' || c == '{' || c == '}')
            {
                return true;
            }
            return false;
        }

        bool CharIsCorrectSign(char c)
        {
            if (c == '=' || c == '<' || c == '>' || c == '!'
               || c == '+' || c == '-' || c == '*' || c == '/'
               || c == ',' || c == '|' || c == '&'
               )
            {
                return true;
            }
            return false;
        }
    }
}
