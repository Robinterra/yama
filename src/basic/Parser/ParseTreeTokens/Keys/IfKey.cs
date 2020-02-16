namespace LearnCsStuf.Basic
{
    public class IfKey : IParseTreeToken
    {

        #region get/set

        public IParseTreeToken Condition
        {
            get;
            set;
        }

        public IParseTreeToken IfStatement
        {
            get;
            set;
        }

        public IParseTreeToken ElseStatement
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public IParseTreeToken Parse ( Parser parser )
        {
            if ( parser.Current.Kind != SyntaxKind.If ) return null;

            return null;
        }

        #endregion methods

    }
}