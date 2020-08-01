using System;
using System.Collections.Generic;
using System.Text;

namespace Yama.Lexer
{
    public class BedingtesCompilieren : ILexerToken
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
                return SyntaxKind.BedingtesCompilieren;
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

        public BedingtesCompilieren ( ZeichenKette begin, ZeichenKette end )
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
                if ( lexer.CurrentByte == 0 ) return TokenStatus.SyntaxError;

                isOnBox = this.End.CheckChar ( lexer ) != TokenStatus.Complete;

                if ( isOnBox ) lexer.NextByte (  );
            }

            return TokenStatus.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( byte[] data )
        {
            Lexer lexer = new Lexer ( new System.IO.MemoryStream ( data ) );
            lexer.LexerTokens.Add ( new Replacer ( this.Begin, string.Empty ) );
            lexer.LexerTokens.Add ( new Replacer ( this.End, string.Empty ) );

            List<byte> daten = new List<byte>();
            foreach ( SyntaxToken token in lexer )
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