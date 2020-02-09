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
            set;
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
                if ( lexerToken.CheckChar ( this.current, false ) == TokenStatus.Cancel ) continue;

                SyntaxToken result = this.ExecuteLexerToken ( lexerToken );

                if (result == null) continue;

                return result;
            }

            if (this.current == '\0') return new SyntaxToken ( SyntaxKind.Whitespaces, this.position, this.line, this.column, "EndOfFile", "EndOfFile" );

            SyntaxToken UnknownToken = new SyntaxToken ( SyntaxKind.Unknown, this.position, this.line, this.column, this.current.ToString(), null );

            this.NextChar (  );

            return UnknownToken;
        }

        // -----------------------------------------------

        private bool NextChar (  )
        {
            this.position++;

            this.column++;

            if (this.position > this.Text.Length) return false;

            if (this.current != '\n') return true;

            this.line++;

            this.column = 1;

            return true;
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
            int column = this.column;
            int line = this.line;

            TokenStatus status = TokenStatus.Accept;

            while ( (status = lexer.CheckChar ( this.current, true )) == TokenStatus.Accept )
            {
                if (!this.NextChar (  )) return null;
            }

            if (status == TokenStatus.CompleteOne)
            {
                status = TokenStatus.Complete;

                this.NextChar();
            }

            if (status != TokenStatus.Complete)
            {
                this.position = start;
                this.column = column;
                this.line = line;
                return null;
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