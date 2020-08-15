using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ContainerExpression : IParseTreeNode, IPriority, IContainer
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IParseTreeNode ExpressionParent
        {
            get;
            set;
        }

        public IdentifierToken Ende
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

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.OpenBracket ) return null;

            IdentifierToken kind = parser.FindEndToken ( token, IdentifierKind.CloseBracket, IdentifierKind.OpenBracket );

            if ( kind == null ) return null;

            if ( kind.Node != null ) return null;

            ContainerExpression expression = new ContainerExpression (  );

            expression.Token = token;
            expression.Ende = kind;
            expression.Ende.Node = expression;
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

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            return this.ExpressionParent.Indezieren(index, parent);
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.ExpressionParent.Compile(compiler, mode);

            return true;
        }
    }
}