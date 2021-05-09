using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Yama.Lexer
{
    public class Lexer : IEnumerator, IEnumerable
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        private int position;

        private int line = 1;

        private int column = 0;

        // -----------------------------------------------

        #endregion vars

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        private Stream daten;

        // -----------------------------------------------

        public Stream Daten
        {
            get
            {
                return this.daten;
            }
            set
            {
                this.daten = value;
                this.CurrentByte = (byte) this.Daten.ReadByte();
                this.position = 0;
                this.column = 0;
                this.line = 1;
            }
        }

        // -----------------------------------------------

        public List<ILexerToken> LexerTokens
        {
            get;
            set;
        } = new List<ILexerToken> ();

        // -----------------------------------------------

        public byte CurrentByte
        {
            get;
            private set;
        }

        // -----------------------------------------------

        public char CurrentChar
        {
            get;
            set;
        }

        // -----------------------------------------------

        public object Current
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Lexer (  )
        {

        }

        // -----------------------------------------------

        public Lexer ( Stream daten )
        {
            this.Daten = daten;
        }

        // -----------------------------------------------

        /**
         * Zum erstellen eines Sub Lexers
         */
        public Lexer ( Lexer lexer, List<ILexerToken> tokens )
        {
            this.daten = lexer.daten;
            this.position = lexer.position;
            this.CurrentByte = lexer.CurrentByte;
            this.CurrentChar = lexer.CurrentChar;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public IdentifierToken NextToken (  )
        {
            if (this.IsEnde (  ) ) return null;

            foreach ( ILexerToken lexerToken in this.LexerTokens )
            {
                if (lexerToken == null) continue;

                IdentifierToken result = this.ExecuteLexerToken ( lexerToken );

                if (result == null) continue;

                return result;
            }

            IdentifierToken UnknownToken = new IdentifierToken ( IdentifierKind.Unknown, this.position, this.line, this.column, new byte[] { this.CurrentByte }, this.CurrentChar.ToString() );

            UnknownToken.CleanDaten = new byte[] { this.CurrentByte };

            this.NextByte (  );

            return UnknownToken;
        }

        // -----------------------------------------------

        public IdentifierToken NextFindMatch()
        {
            foreach (IdentifierToken token in this)
            {
                if (token.Kind == IdentifierKind.Unknown) continue;

                return token;
            }

            return null;
        }

        // -----------------------------------------------

        public TokenState SubLexen ( List<ILexerToken> tokens )
        {
            foreach ( ILexerToken token in tokens )
            {
                if ( this.ExecuteLexerToken ( token, out _, out _, out _ ) == TokenState.Complete ) return TokenState.Complete;
            }

            return TokenState.Cancel;
        }

        // -----------------------------------------------

        public bool NextByte (  )
        {
            this.position++;

            this.column++;

            if (this.position >= this.Daten.Length)
            {
                this.CurrentByte = 0;
                this.CurrentChar = '\0';

                return false;
            }

            this.Daten.Seek ( this.position, SeekOrigin.Begin );

            this.CurrentByte = (byte) this.Daten.ReadByte();

            if (System.Convert.ToChar(this.CurrentByte) != '\n') return true;

            this.line++;

            this.column = 0;

            return true;
        }

        // -----------------------------------------------

        public bool CurrentCharMode (  )
        {
            if ( this.position >= this.Daten.Length )
            {
                this.CurrentChar = '\0';
                this.CurrentByte = 0;

                return false;
            }

            BinaryReader reader = new BinaryReader(this.Daten, System.Text.Encoding.UTF8);

            this.Daten.Seek ( this.position, SeekOrigin.Begin );

            this.CurrentChar = reader.ReadChar();

            return true;
        }

        // -----------------------------------------------

        public bool NextChar (  )
        {

            this.column++;

            BinaryReader reader = new BinaryReader(this.Daten, System.Text.Encoding.UTF8);

            this.position = (int)this.Daten.Position;

            if (this.position >= this.Daten.Length)
            {
                this.CurrentByte = 0;
                this.CurrentChar = '\0';

                return false;
            }

            this.CurrentByte = (byte)this.Daten.ReadByte (  );

            this.Daten.Seek ( this.position, SeekOrigin.Begin );

            this.CurrentChar = reader.ReadChar();

            if (System.Convert.ToChar(this.CurrentByte) != '\n') return true;

            this.line++;

            this.column = 0;

            return true;
        }

        // -----------------------------------------------

        private bool IsEnde (  )
        {
            return this.position >= this.Daten.Length;
        }

        // -----------------------------------------------

        private TokenState ExecuteLexerToken ( ILexerToken token, out int start, out int column, out int line )
        {
            start = this.position;
            column = this.column;
            line = this.line;
            byte current = this.CurrentByte;

            TokenState state = token.CheckChar ( this );

            if (state == TokenState.Complete) return state;

            this.position = start;
            this.column = column;
            this.line = line;
            this.CurrentByte = current;

            this.Daten.Seek ( this.position, SeekOrigin.Begin );

            return state;
        }

        // -----------------------------------------------

        private IdentifierToken ExecuteLexerToken ( ILexerToken token )
        {
            TokenState state = this.ExecuteLexerToken ( token, out int start, out int column, out int line );

            if (state != TokenState.Complete) return null;

            byte[] daten = new byte[this.position - start];

            this.Daten.Seek ( start, SeekOrigin.Begin );

            this.Daten.Read ( daten, 0, this.position - start );

            object data = token.GetValue ( daten );

            IdentifierToken result = new IdentifierToken ( token.Kind, this.position, this.line, this.column, daten, data );

            if (!(data is string t)) return result;

            result.CleanDaten = System.Text.Encoding.UTF8.GetBytes(t);

            return result;
        }

        // -----------------------------------------------

        public bool MoveNext()
        {
            this.Current = this.NextToken (  );

            return this.Current != null;
        }

        // -----------------------------------------------

        public void Reset()
        {
            this.position = 0;
            this.column = 1;
            this.line = 1;
        }

        // -----------------------------------------------

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------
    }
}

// -- [EOF] --