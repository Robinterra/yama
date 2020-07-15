using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class ElseKey : IParseTreeNode, IContainer
    {

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

        public SyntaxToken Ende
        {
            get
            {
                return (this.Statement is IContainer t) ? t.Ende : this.Statement.Token;
            }
            set
            {

            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Else ) return null;

            ElseKey key = new ElseKey (  );
            key.Token = token;
            token.Node = key;

            SyntaxToken statementchild = parser.Peek ( token, 1);

            key.Statement = parser.ParseCleanToken(statementchild);

            if (key.Statement == null) return null;

            key.Statement.Token.ParentNode = key;

            return key;
        }

        #endregion methods

    }
}