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
            get;
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

        public Escaper? Escape
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int MaxLength
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Text ( ZeichenKette begin, ZeichenKette ende, Escaper? escape, IdentifierKind identifier = IdentifierKind.Text, int maxLength = int.MaxValue )
        {
            this.Begin = begin;
            this.Ende = ende;
            this.Escape = escape;
            this.Kind = identifier;
            this.MaxLength = maxLength;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenState CheckChar ( Lexer lexer )
        {
            if ( this.Begin.CheckChar ( lexer ) != TokenState.Complete ) return TokenState.Cancel;

            int count = 0;
            while ( true )
            {
                if ( this.Ende.CheckChar ( lexer ) == TokenState.Complete )
                {
                    if (this.MaxLength == int.MaxValue) return TokenState.Complete;

                    return this.MaxLength >= count ? TokenState.Complete : TokenState.SyntaxError;
                }

                if ( this.Escape != null && this.Escape.CheckChar ( lexer ) == TokenState.Complete ) continue;

                lexer.NextByte (  );
                count += 1;

                if ( lexer.CurrentByte == 0 ) return TokenState.SyntaxError;
            }

        }

        // -----------------------------------------------

        public object GetValue ( byte[] data )
        {
            Lexer lexer = new Lexer ( new System.IO.MemoryStream ( data ) );
            if (this.Escape is not null) lexer.LexerTokens.Add ( this.Escape );
            lexer.LexerTokens.Add ( new Replacer ( this.Begin, string.Empty ) );
            lexer.LexerTokens.Add ( new Replacer ( this.Ende, string.Empty ) );

            List<byte> daten = new List<byte>();
            foreach ( IdentifierToken token in lexer )
            {
                if (token.CleanDaten == null) continue;

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