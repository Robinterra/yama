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

        private int column = 1;

        // -----------------------------------------------

        #endregion vars

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Text
        {
            get;
        }

        // -----------------------------------------------

        public List<ILexerToken> LexerTokens
        {
            get;
        } = new List<ILexerToken> ();

        // -----------------------------------------------

        private char current
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
                if ( lexerToken.CheckChar ( this.current, false ) == TokenStatus.Cancel ) continue;

                return this.ExecuteLexerToken ( lexerToken );
            }

            SyntaxToken UnknownToken = new SyntaxToken ( SyntaxKind.Unknown, this.position, this.line, this.column, this.current.ToString(), null );

            this.NextChar (  );

            return UnknownToken;
        }

        // -----------------------------------------------

        private bool NextChar (  )
        {
            this.position++;

            this.column++;

            return this.position <= this.Text.Length;
        }

        // -----------------------------------------------

        private bool IsEnde (  )
        {
            return this.position >= this.Text.Length;
        }

        // -----------------------------------------------

        private SyntaxToken ExecuteLexerToken ( ILexerToken lexer )
        {
            int start = this.position;

            while ( lexer.CheckChar ( this.current, true ) )
            {
                if (!this.NextChar (  )) return null;

                if (this.current != '\n') continue;

                this.line++;

                this.column = 1;
            }

            string text = this.Text.Substring ( start, this.position - start );

            return new SyntaxToken ( lexer.Kind, this.position, this.line, this.column, text, lexer.GetValue ( text ) );
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