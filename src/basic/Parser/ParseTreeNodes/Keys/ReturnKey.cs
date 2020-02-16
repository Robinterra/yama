using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class ReturnKey : IParseTreeNode
    {

        private NormalExpression normal;

        #region get/set

        public IParseTreeNode Statement
        {
            get;
            set;
        }

        public SyntaxToken Token
        {
            get;
            set;
        }

        public NormalExpression Normal
        {
            get
            {
                if (this.normal != null) return this.normal;

                this.normal = new NormalExpression();

                return this.normal;
            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                result.Add ( this.Statement );

                return result;
            }
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Return ) return null;

            IParseTreeNode node = this.Normal.Parse ( parser, parser.Peek ( token, 1 ) );

            if ( node == null ) return null;

            ReturnKey result = new ReturnKey();

            result.Statement = node;
            result.Token = token;
            token.Node = result;
            node.Token.ParentNode = result;

            return result;
        }

        #endregion methods

    }
}