using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class ContainerExpression : IParseTreeNode, IPriority
    {

        #region get/set

        public SyntaxToken Token
        {
            get;
            set;
        }

        public IParseTreeNode ExpressionParent
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new List<IParseTreeNode> { this.ExpressionParent };
            }
        }

        public int Prio
        {
            get
            {
                return 10;
            }
        }

        #endregion get/set

        public IParseTreeNode SwapChild ( IPriority node )
        {
            IParseTreeNode result = this.Token.ParentNode;

            if ( result is IPriority t && t.Prio < node.Prio ) return t.SwapChild ( node );

            return result;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.OpenKlammer ) return null;

            SyntaxToken kind = token;

            int openKlammers = 0;
            for ( int i = 1; kind.Kind != SyntaxKind.CloseKlammer || openKlammers >= 0; i++ )
            {
                kind = parser.Peek ( token, i );

                if ( kind == null ) return null;

                if ( kind.Kind == SyntaxKind.OpenKlammer ) openKlammers++;
                if ( kind.Kind == SyntaxKind.CloseKlammer ) openKlammers--;
            }

            if ( kind.Node != null ) return null;

            ContainerExpression expression = new ContainerExpression (  );

            expression.Token = token;

            token.Node = expression;
            kind.Node = expression;
            parser.Repleace ( token, kind.Position );

            List<IParseTreeNode> nodes = parser.ParseCleanTokens ( token.Position + 1, kind.Position );

            if ( nodes == null ) return null;
            if ( nodes.Count != 1 ) return null;

            expression.ExpressionParent = nodes[0];

            //if ( expression.ExpressionParent is IPriority t && t.Prio < this.Prio ) expression.ExpressionParent = t.SwapChild ( expression );

            nodes[0].Token.ParentNode = expression;

            return expression;
        }
    }
}