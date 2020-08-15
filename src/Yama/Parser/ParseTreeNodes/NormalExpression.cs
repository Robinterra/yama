using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NormalExpression : IParseTreeNode, IEndExpression
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
                return 0;
            }
        }

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            //SyntaxToken kind = parser.FindAToken ( token, SyntaxKind.EndOfCommand );
            
            //if ( kind == null ) return null;
            //if ( kind.Node != null ) return null;
            if ( token.Kind != IdentifierKind.EndOfCommand ) return null;

            IdentifierToken left = parser.Peek ( token, -1 );

            NormalExpression expression = new NormalExpression (  );

            expression.Token = token;

            token.Node = expression;

            if (left == null) return expression;

            List<IParseTreeNode> nodes = parser.ParseCleanTokens ( parser.Start, token.Position );

            IParseTreeNode node = null;

            if ( nodes == null ) return null;
            //if ( nodes.Count > 1 ) return null;
//            if ( nodes.Count == 1 ) node = nodes[0];
            node = nodes[nodes.Count - 1];

            expression.ExpressionParent = node;

            if ( node != null ) node.Token.ParentNode = expression;

            return expression;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            //if (!(parent is IndexContainer container)) return index.CreateError(this);

            if (this.ExpressionParent == null) return true;

            return this.ExpressionParent.Indezieren(index, parent);
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.ExpressionParent.Compile(compiler, mode);

            return true;
        }
    }
}