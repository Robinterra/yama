namespace LearnCsStuf.Basic
{
    public class IfKey : IParseTreeNode
    {

        #region get/set

        public IParseTreeNode Condition
        {
            get;
            set;
        }

        public IParseTreeNode IfStatement
        {
            get;
            set;
        }

        public IParseTreeNode ElseStatement
        {
            get;
            set;
        }

        public SyntaxToken Token
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.If ) return null;
            if ( parser.Peek ( token, 1 ).Kind != SyntaxKind.OpenKlammer ) return null;


            return null;
        }

        #endregion methods

    }
}