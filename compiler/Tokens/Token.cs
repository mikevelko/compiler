using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.Tokens
{
    public enum TokenType
    {
        // SingleToken
        PLUS,
        MINUS,
        MULTIPLE,
        DIVIDE,

        ASSIGN,

        OR,
        AND,

        NEGATION,

        EQUAL,
        NOT_EQUAL,
        MORE,
        MORE_EQUAL,
        LESS,
        LESS_EQUAL,

        COMMA,

        LEFT_ROUND_BRACKET,
        RIGHT_ROUND_BRACKET,

        LEFT_CURLY_BRACKET,
        RIGHT_CURLY_BRACKET,

        //Keywords

        IF,
        ELSE,
        WHILE,
        RETURN,

        INT,
        DOUBLE,
        TRUE,
        FALSE,

        IDENTIFIER,
        NUMBER_INT,
        NUMBER_DOUBLE,

        UNKNOWN
    }
    public class Token
    {
        public TokenType tokenType;
        public string text; //dla double i int przejsc na ten typ
        public int intValue;
        public double doubleValue;
        public (int, int) position;

        public Token(TokenType tokenType, string text, (int,int) position) 
        {
            this.tokenType = tokenType;
            this.text = text;
            this.position = position;
        }
        public Token(TokenType tokenType, int value, (int, int) position)
        {
            this.tokenType = tokenType;
            this.intValue = value;
            this.position = position;
        }
        public Token(TokenType tokenType, double value, (int, int) position)
        {
            this.tokenType = tokenType;
            this.doubleValue = value;
            this.position = position;
        }
    }
}
