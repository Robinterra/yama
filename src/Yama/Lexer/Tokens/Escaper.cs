using System;
using System.Collections.Generic;
using System.Text;

namespace Yama.Lexer
{
    public class Escaper : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.Escape;
            }
        }

        // -----------------------------------------------

        public ZeichenKette EscapeZeichen
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<Replacer> Replacers
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Escaper ( ZeichenKette escapeZeichen, List<Replacer> replaces )
        {
            this.EscapeZeichen = escapeZeichen;
            this.Replacers = replaces;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            if ( this.EscapeZeichen.CheckChar ( lexer ) != TokenStatus.Complete ) return TokenStatus.Cancel;

            foreach ( Replacer replacer in this.Replacers )
            {
                if ( replacer.CheckChar ( lexer ) == TokenStatus.Complete ) return TokenStatus.Complete;
            }

            return TokenStatus.SyntaxError;
        }

        // -----------------------------------------------

        public object GetValue ( byte[] daten )
        {
            byte[] newData = new byte[daten.Length - this.EscapeZeichen.Length];

            Buffer.BlockCopy(daten, this.EscapeZeichen.Length, newData, 0, daten.Length - this.EscapeZeichen.Length);

            //text = text.Substring ( this.EscapeZeichen.Length );
            System.IO.MemoryStream stream = new System.IO.MemoryStream ( newData );

            Lexer lexer = new Lexer ( stream );
            lexer.LexerTokens.AddRange ( this.Replacers );

            StringBuilder builder = new StringBuilder();
            foreach ( IdentifierToken token in lexer )
            {
                if (token.Kind == IdentifierKind.Unknown) continue;
                builder.Append ( token.Value );
            }

            return builder.ToString (  );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --