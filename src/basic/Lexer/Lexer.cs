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

        public string Text
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<ILexerToken> LexerTokens
        {
            get;
            set;
        } = new List<ILexerToken> ();

        // -----------------------------------------------

        public char CurrentChar
        {
            get
            {
                if (this.IsEnde (  )) return '\0';
                return this.Text[position];
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

        public Lexer ( string text )
        {
            this.Text = text;
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

            SyntaxToken UnknownToken = new SyntaxToken ( SyntaxKind.Unknown, this.position, this.line, this.column, this.CurrentChar.ToString(), this.CurrentChar.ToString() );

            this.NextChar (  );

            return UnknownToken;
        }

        // -----------------------------------------------

        public bool NextChar (  )
        {
            this.position++;

            this.column++;

            if (this.position > this.Text.Length) return false;

            if (this.CurrentChar != '\n') return true;

            this.line++;

            this.column = 0;

            return true;
        }

        // -----------------------------------------------

        private bool IsEnde (  )
        {
            return this.position >= this.Text.Length;
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

                return null;
            }

            string text = this.Text.Substring ( start, this.position - start );

            return new SyntaxToken ( token.Kind, this.position, this.line, this.column, text, token.GetValue ( text ) );
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