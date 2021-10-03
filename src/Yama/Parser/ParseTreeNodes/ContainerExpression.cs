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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region  ctor

        public ContainerExpression (  )
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        public ContainerExpression ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        /**
         * @todo Ab in die Parser klasse damit!
         */

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.OpenBracket ) return null;

            IdentifierToken kind = request.Parser.FindEndTokenWithoutParse ( request.Token, IdentifierKind.CloseBracket, IdentifierKind.OpenBracket );

            if ( kind == null ) return null;
            if ( kind.Node != null ) return null;

            ContainerExpression expression = new ContainerExpression (  );

            expression.Token = request.Token;
            expression.AllTokens.Add(request.Token);
            expression.Ende = kind;
            expression.AllTokens.Add(kind);

            List<IParseTreeNode> nodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, kind.Position );

            request.Parser.Repleace ( request.Token, kind.Position );

            if ( nodes == null ) return null;
            if ( nodes.Count != 1 ) return null;

            expression.ExpressionParent = nodes[0];

            return expression;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            return this.ExpressionParent.Indezieren(request);
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            this.ExpressionParent.Compile(request);

            return true;
        }
    }
}