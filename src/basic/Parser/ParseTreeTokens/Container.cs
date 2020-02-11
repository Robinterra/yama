using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Container : IParseTreeToken
    {

        #region get/set

        public SyntaxKind Kind
        {
            get;
        }

        public List<IParseTreeToken> Statements
        {
            get;
            set;
        }

        #endregion get/set

        public bool Parse ( Parser parser )
        {
            return true;
        }
    }
}