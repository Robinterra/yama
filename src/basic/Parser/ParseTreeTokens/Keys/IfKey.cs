namespace LearnCsStuf.Basic
{
    public class IfKey : IParseTreeToken
    {

        #region get/set

        public SyntaxKind Kind
        {
            get;
        }

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

        public bool Parse ( Parser parser )
        {
            return true;
        }
    }
}