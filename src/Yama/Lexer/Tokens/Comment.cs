using System;

namespace Yama.Lexer
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

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.Comment;
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

        public TokenState CheckChar ( Lexer lexer )
        {
            if ( this.Begin.CheckChar ( lexer ) != TokenState.Complete ) return TokenState.Cancel;

            bool isOnBox = true;

            while ( isOnBox )
            {
                if ( lexer.CurrentByte == 0 ) return TokenState.Complete;

                isOnBox = this.End.CheckChar ( lexer ) != TokenState.Complete;

                if ( isOnBox ) lexer.NextByte (  );
            }

            return TokenState.Complete;
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