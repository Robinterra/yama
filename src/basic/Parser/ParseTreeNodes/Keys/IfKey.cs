using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class IfKey : IParseTreeNode, IContainer
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

        public SyntaxToken Ende
        {
            get
            {
                if ( this.ElseStatement == null ) return (this.IfStatement is IContainer t) ? t.Ende : this.IfStatement.Token;

                return (this.ElseStatement is IContainer a) ? a.Ende : this.ElseStatement.Token;
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

                if (this.Condition != null) result.Add ( this.Condition );
                if (this.IfStatement != null) result.Add ( this.IfStatement );
                if (this.ElseStatement != null) result.Add ( this.ElseStatement );

                return result;
            }
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.If ) return null;
            if ( parser.Peek ( token, 1 ).Kind != SyntaxKind.OpenKlammer ) return null;

            IfKey key = new IfKey (  );
            key.Token = token;
            token.Node = key;

            SyntaxToken conditionkind = parser.Peek ( token, 1 );

            IParseTreeNode rule = parser.GetRule<ContainerExpression>();

            key.Condition = rule.Parse(parser, conditionkind);

            if (key.Condition == null) return null;

            key.Condition.Token.ParentNode = key;

            SyntaxToken ifStatementchild = parser.Peek ( ((ContainerExpression)key.Condition).Ende, 1);

            key.IfStatement = parser.ParseCleanToken(ifStatementchild);

            if (key.IfStatement == null) return null;

            key.IfStatement.Token.ParentNode = key;

            SyntaxToken wlsePeekToken = (key.IfStatement is IContainer t) ? t.Ende : key.IfStatement.Token;

            SyntaxToken elseStatementChild = parser.Peek ( wlsePeekToken, 1 );

            if ( elseStatementChild == null ) return key;

            if ( elseStatementChild.Node != null ) return key;

            if ( elseStatementChild.Kind != SyntaxKind.Else ) return key;

            key.ElseStatement = parser.ParseCleanToken ( elseStatementChild );

            if (key.ElseStatement == null) return null;

            key.ElseStatement.Token.ParentNode = key;

            return key;
        }

        #endregion methods

    }
}