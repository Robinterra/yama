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

        public SyntaxToken Ende
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
            get;
            set;
        }

        #endregion get/set

        #region  ctor

        public ContainerExpression (  )
        {

        }

        public ContainerExpression ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        /**
         * @todo Ab in die Parser klasse damit!
         */
        private SyntaxToken FindEndToken ( Parser parser, SyntaxToken begin)
        {
            SyntaxToken kind = begin;

            for ( int i = 1; kind.Kind != SyntaxKind.CloseKlammer; i++ )
            {
                kind = parser.Peek ( begin, i );

                if ( kind == null ) return null;

                if ( kind.Kind != SyntaxKind.OpenKlammer ) continue;

                IParseTreeNode nodeCon = parser.ParseCleanToken ( kind );

                if ( nodeCon == null ) return null;

                if ( !(nodeCon is ContainerExpression c) ) return null;

                i = c.Ende.Position;
            }

            return kind;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.OpenKlammer ) return null;

            SyntaxToken kind = this.FindEndToken ( parser, token );

            if ( kind == null ) return null;

            if ( kind.Node != null ) return null;

            ContainerExpression expression = new ContainerExpression (  );

            expression.Token = token;
            expression.Ende = kind;

            token.Node = expression;
            kind.Node = expression;

            List<IParseTreeNode> nodes = parser.ParseCleanTokens ( token.Position + 1, kind.Position );

            parser.Repleace ( token, kind.Position );

            if ( nodes == null ) return null;
            if ( nodes.Count != 1 ) return null;

            expression.ExpressionParent = nodes[0];

            nodes[0].Token.ParentNode = expression;

            return expression;
        }
    }
}