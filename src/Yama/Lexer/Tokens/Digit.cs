using System;

namespace Yama.Lexer
{
    public class Digit : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.NumberToken;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            lexer.CurrentCharMode (  );

            if ( !char.IsDigit ( lexer.CurrentChar ) ) return TokenStatus.Cancel;

            bool isFirst0 = lexer.CurrentChar == '0';
            bool ishex = false;

            while ( this.CheckCurrentChar ( lexer.CurrentChar, ishex ) )
            {
                lexer.NextChar (  );

                if (!isFirst0) continue;

                isFirst0 = false;

                bool isok = lexer.CurrentChar == 'x';
                ishex = isok;
                if (!isok) isok = lexer.CurrentChar == 'b';
                if (!isok) return TokenStatus.Complete;

                lexer.NextChar (  );
            }

            return TokenStatus.Complete;
        }

        private bool CheckCurrentChar(char currentChar, bool ishex)
        {
            if (!ishex) return char.IsDigit ( currentChar );

            return currentChar.IsHex();
        }

        // -----------------------------------------------

        public object GetValue ( byte[] daten )
        {
            string text = System.Text.Encoding.UTF8.GetString ( daten );

            try
            {
                if (text.Contains("0x")) return int.Parse(text.Replace("0x", string.Empty), System.Globalization.NumberStyles.HexNumber);
            }
            catch { }

            if (!int.TryParse ( text, out int result )) return 0;

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

/**
 * @link https://stackoverflow.com/questions/228523/char-ishex-in-c-sharp
 */
public static class Extensions
{
   public static bool IsHex(this char c)
   {
      return   (c >= '0' && c <= '9') ||
               (c >= 'a' && c <= 'f') ||
               (c >= 'A' && c <= 'F');
   }
}

// -- [EOF] --