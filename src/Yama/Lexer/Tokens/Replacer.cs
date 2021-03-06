using System;

namespace Yama.Lexer
{
    public class Replacer : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.Replacer;
            }
        }

        // -----------------------------------------------

        public string ReplaceText
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ZeichenKette ReplaceKette
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Replacer ( ZeichenKette replaceKette, string replaceText )
        {
            this.ReplaceKette = replaceKette;
            this.ReplaceText = replaceText;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenState CheckChar ( Lexer lexer )
        {
            return this.ReplaceKette.CheckChar ( lexer );
        }

        // -----------------------------------------------

        public object GetValue ( byte[] data )
        {
            return this.ReplaceText;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --
