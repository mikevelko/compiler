using compiler.CharReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.Tokens;
using System.Globalization;

namespace compiler.Scanners
{
    public class Scanner : IScanner
    {
        public Reader reader;
        public Token token;
        public Dictionary<string, TokenType> KeywordTokens;
        public Dictionary<string, TokenType> SignsTokens;
        public Scanner(Reader reader) 
        {
            this.reader = reader;
            reader.MoveToNextChar();
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
            SignsTokens.Add("!", TokenType.NEGATION);

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
        //podzielic na funkcje
        public bool NextToken()
        {
            while (CharIsWhite(reader.currentChar))
            {
                if (!reader.MoveToNextChar()) 
                {
                    return false;
                }
            }
            if (reader.currentChar == (char)0)
            {
                token = null;
                return false;
            }
            (int, int) position = (reader.currentLineNumber, reader.currentPositionInLine);
            if (char.IsLetter(reader.currentChar)) 
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (char.IsLetterOrDigit(reader.currentChar)) 
                {
                    stringBuilder.Append(reader.currentChar);
                    reader.MoveToNextChar();
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
            if (char.IsDigit(reader.currentChar)) 
            {
                StringBuilder stringBuilder = new StringBuilder();
                bool isDouble = false;
                //dodac obsluge np 0007 oraz przerobic na wartosc od razu 
                while (char.IsDigit(reader.currentChar)) 
                {
                    stringBuilder.Append(reader.currentChar);
                    reader.MoveToNextChar();
                }
                if(reader.currentChar == '.') 
                {
                    stringBuilder.Append(reader.currentChar);
                    reader.MoveToNextChar();
                    isDouble = true;
                }
                while (char.IsDigit(reader.currentChar))
                {
                    stringBuilder.Append(reader.currentChar);
                    reader.MoveToNextChar();
                }
                if (char.IsLetter(reader.currentChar)) 
                {
                    while (char.IsLetterOrDigit(reader.currentChar)) 
                    {
                        stringBuilder.Append(reader.currentChar);
                        reader.MoveToNextChar();
                    }
                    token = new Token(TokenType.UNKNOWN, stringBuilder.ToString(), position);
                    return true;
                }
                if (isDouble) 
                {
                    double value = Double.Parse(stringBuilder.ToString(), CultureInfo.InvariantCulture);
                    token = new Token(TokenType.NUMBER_DOUBLE, value, position);
                }
                else 
                {
                    token = new Token(TokenType.NUMBER_INT, Convert.ToInt32(stringBuilder.ToString()), position);
                }
                return true;
            }
            //zrobic w jednym 
            if (CharIsBracket(reader.currentChar))
            {
                string text = reader.currentChar.ToString();
                TokenType type;
                if (!SignsTokens.TryGetValue(text, out type))
                {
                    throw new Exception("Invalid Dictionary");
                }

                token = new Token(type, text, position);

                reader.MoveToNextChar();

                return true;
            }
            if (CharIsCorrectSign(reader.currentChar)) 
            {
                if (CharIsCorrectSign(reader.currentChar))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    while (CharIsCorrectSign(reader.currentChar))
                    {
                        stringBuilder.Append(reader.currentChar);
                        reader.MoveToNextChar();
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


            token = new Token(TokenType.UNKNOWN, reader.currentChar.ToString(), position);
            reader.MoveToNextChar();

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
