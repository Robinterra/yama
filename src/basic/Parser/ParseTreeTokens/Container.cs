using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Container : IParseTreeToken
    {

        #region get/set

        public List<IParseTreeToken> Statements
        {
            get;
            set;
        }

        #endregion get/set

        public IParseTreeToken Parse ( Parser parser )
        {
            if ( parser.Current.Kind != SyntaxKind.BeginContainer ) return null;

            return null;
        }
    }
}