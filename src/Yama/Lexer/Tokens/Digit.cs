using System;
using System.Globalization;

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

        public bool EnablePunctuation
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Digit(bool enablePunctuation = true)
        {
            this.EnablePunctuation = enablePunctuation;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenState CheckChar ( Lexer lexer )
        {
            lexer.CurrentCharMode (  );

            if ( !char.IsDigit ( lexer.CurrentChar ) ) return TokenState.Cancel;

            bool isFirst0 = lexer.CurrentChar == '0';
            bool ishex = false;
            bool hasPoint = false;

            while ( this.CheckCurrentChar ( lexer.CurrentChar, ishex ) )
            {
                lexer.NextChar (  );

                if (!hasPoint)
                {
                    if (lexer.CurrentChar == '.' && this.EnablePunctuation)
                    {
                        isFirst0 = false;
                        hasPoint = true;
                        lexer.NextChar();
                    }
                }

                if (!isFirst0) continue;

                isFirst0 = false;

                bool isok = lexer.CurrentChar == 'x';
                ishex = isok;
                if (!isok) isok = lexer.CurrentChar == 'b';
                if (!isok) return TokenState.Complete;

                lexer.NextChar (  );
            }

            return TokenState.Complete;
        }

        private bool CheckCurrentChar(char currentChar, bool ishex)
        {
            if (ishex) return currentChar.IsHex();

            return char.IsDigit ( currentChar );
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

            if (text.Contains('.'))
            {
                if (!decimal.TryParse(text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dRes)) return (decimal)0.0;

                return dRes;
            }

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