using System.Collections;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Parser : IEnumerator, IEnumerable
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        private int position;

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

        /*public Parser ( List<> )
        {
            this.Text = text;
        }*/

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
                if ( lexerToken.CheckChar ( this.current, false ) != TokenStatus.Accept ) continue;

                return this.ExecuteLexerToken ( lexerToken );
            }

            //SyntaxToken UnknownToken = new SyntaxToken ( SyntaxKind.Unknown, this.position, this.current.ToString(), null );

            this.position++;

            return null;
        }

        // -----------------------------------------------

        private bool NextChar (  )
        {
            this.position++;

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

            while ( lexer.CheckChar ( this.current, false ) == TokenStatus.Accept )
            {
                if (!this.NextChar (  )) return null;
            }

            string text = this.Text.Substring ( start, this.position - start );

            //return new SyntaxToken ( lexer.Kind, this.position, text, lexer.GetValue ( text ) );

            return null;
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