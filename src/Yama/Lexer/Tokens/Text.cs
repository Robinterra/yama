using System;
using System.Collections.Generic;
using System.Text;

namespace Yama.Lexer
{
    public class Text : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.Text;
            }
        }

        // -----------------------------------------------

        public ZeichenKette Begin
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ZeichenKette Ende
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Escaper Escape
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Text ( ZeichenKette begin, ZeichenKette ende, Escaper escape )
        {
            this.Begin = begin;
            this.Ende = ende;
            this.Escape = escape;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            if ( this.Begin.CheckChar ( lexer ) != TokenStatus.Complete ) return TokenStatus.Cancel;

            while ( true )
            {
                if ( this.Ende.CheckChar ( lexer ) == TokenStatus.Complete ) return TokenStatus.Complete;

                if ( this.Escape.CheckChar ( lexer ) == TokenStatus.Complete ) continue;

                lexer.NextByte (  );

                if ( lexer.CurrentByte == 0 ) return TokenStatus.SyntaxError;
            }

        }

        // -----------------------------------------------

        public object GetValue ( byte[] data )
        {
            Lexer lexer = new Lexer ( new System.IO.MemoryStream ( data ) );
            lexer.LexerTokens.Add ( this.Escape );
            lexer.LexerTokens.Add ( new Replacer ( this.Begin, string.Empty ) );
            lexer.LexerTokens.Add ( new Replacer ( this.Ende, string.Empty ) );

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