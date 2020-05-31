using System;

namespace LearnCsStuf.Basic
{
    public class Comment : ILexerToken
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        // -----------------------------------------------

        #endregion vars

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Comment;
            }
        }

        // -----------------------------------------------

        public ZeichenKette Begin
        {
            get;
        }

        // -----------------------------------------------

        public ZeichenKette End
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Comment ( ZeichenKette begin, ZeichenKette end )
        {
            this.Begin =  begin;

            this.End = end;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            if ( this.Begin.CheckChar ( lexer ) != TokenStatus.Complete ) return TokenStatus.Cancel;

            bool isOnBox = true;

            while ( isOnBox )
            {
                if ( lexer.CurrentByte == 0 ) return TokenStatus.Complete;

                isOnBox = this.End.CheckChar ( lexer ) != TokenStatus.Complete;

                if ( isOnBox ) lexer.NextByte (  );
            }

            return TokenStatus.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( byte[] daten )
        {
            return System.Text.Encoding.UTF8.GetString ( daten );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --