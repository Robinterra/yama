namespace Yama.Lexer
{

    public class TokenInfo : ITokenInfo
    {

        #region get/set

        public string Origin
        {
            get;
        }

        #endregion get/set

        #region ctor

        public TokenInfo(string origin)
        {
            this.Origin = origin;
        }

        #endregion ctor

    }

}