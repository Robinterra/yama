using System;
using System.Collections.Generic;
using System.Text;

namespace Yama.Lexer
{
    public class ConditionalCompilationToken : ILexerToken
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
                return IdentifierKind.ConditionalCompilation;
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

        public ConditionalCompilationToken ( ZeichenKette begin, ZeichenKette end )
        {
            this.Begin = begin;

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
                if ( lexer.CurrentByte == 0 ) return TokenState.SyntaxError;

                isOnBox = this.End.CheckChar ( lexer ) != TokenState.Complete;

                if ( isOnBox ) lexer.NextByte (  );
            }

            return TokenState.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( byte[] data )
        {
            Lexer lexer = new Lexer ( new System.IO.MemoryStream ( data ) );
            lexer.LexerTokens.Add ( new Replacer ( this.Begin, string.Empty ) );
            lexer.LexerTokens.Add ( new Replacer ( this.End, string.Empty ) );

            List<byte> daten = new List<byte>();
            foreach ( IdentifierToken token in lexer )
            {
                daten.AddRange(token.CleanDaten);
            }

            return Encoding.UTF8.GetString(daten.ToArray());
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --