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

            this.NextByte (  );

            return UnknownToken;
        }

        // -----------------------------------------------

        public bool NextByte (  )
        {
            this.position++;

            this.column++;

            this.Daten.Seek ( this.position, SeekOrigin.Begin );

            this.CurrentByte = (byte) this.Daten.ReadByte();

            if (this.position > this.Daten.Length) return false;

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

            this.Daten.Seek ( start, SeekOrigin.Begin );

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