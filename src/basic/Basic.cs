using System;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class BasicExpressionEvaluator
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string File
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string ExpressionLine
        {
            get;
            set;
        }

        // -----------------------------------------------

        private Lexer Tokenizer
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool PrintSyntaxError(SyntaxToken token)
        {
            if (token.Kind != SyntaxKind.Unknown) return false;

            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine ( "({0},{1}) Syntax error - unknown char \"{2}\"", token.Line, token.Column, token.Text );

            Console.ForegroundColor = colr;

            return true;
        }

        // -----------------------------------------------

        public bool DoStuff()
        {
            

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --