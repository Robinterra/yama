using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
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
                this.position = 0;
                this.column = 0;
                this.line = 1;
                this.daten = value;
                this.NextChar();
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
            get
            {
                if (this.IsEnde (  )) return '\0';

                return System.Convert.ToChar ( this.CurrentByte );//this.Text[position];
            }
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

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public SyntaxToken NextToken (  )
        {
            if (this.IsEnde (  ) ) return null;

            foreach ( ILexerToken lexerToken in this.LexerTokens )
            {
                SyntaxToken result = this.ExecuteLexerToken ( lexerToken );

                if (result == null) continue;

                return result;
            }

            SyntaxToken UnknownToken = new SyntaxToken ( SyntaxKind.Unknown, this.position, this.line, this.column, new byte[] { this.CurrentByte }, this.CurrentChar.ToString() );

            this.NextChar (  );

            return UnknownToken;
        }

        // -----------------------------------------------

        public bool NextChar (  )
        {
            this.position++;

            this.column++;

            this.CurrentByte = (byte) this.Daten.ReadByte();

            if (this.position > this.Daten.Length) return false;

            if (this.CurrentChar != '\n') return true;

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

        private SyntaxToken ExecuteLexerToken ( ILexerToken token )
        {
            int start = this.position;
            int column = this.column;
            int line = this.line;

            TokenStatus status = token.CheckChar ( this );

            if (status != TokenStatus.Complete)
            {
                this.position = start;
                this.column = column;
                this.line = line;

                this.Daten.Seek ( this.position, SeekOrigin.Begin );

                return null;
            }

            byte[] daten = new byte[this.position - start];

            this.Daten.Seek ( start - 1, SeekOrigin.Begin );

            this.Daten.Read ( daten, 0, this.position - start );

            return new SyntaxToken ( token.Kind, this.position, this.line, this.column, daten, token.GetValue ( daten ) );
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