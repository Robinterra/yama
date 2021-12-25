using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ContainerExpression : IParseTreeNode, IPriority, IContainer
    {

        private IdentifierToken? ende;

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IParseTreeNode? ExpressionParent
        {
            get;
            set;
        }

        public IdentifierToken Ende
        {
            get
            {
                if (this.ende is null) return this.Token;

                return this.ende;
            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                if (this.ExpressionParent is null) return new();

                return new () { this.ExpressionParent };
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

        public ContainerExpression ( int prio )
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
        }

        #endregion ctor

        /**
         * @todo Ab in die Parser klasse damit!
         */

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.OpenBracket ) return null;

            IdentifierToken? kind = request.Parser.FindEndTokenWithoutParse ( request.Token, IdentifierKind.CloseBracket, IdentifierKind.OpenBracket );
            if ( kind == null ) return null;
            if ( kind.Node != null ) return null;

            ContainerExpression expression = new ContainerExpression ( this.Prio );

            expression.Token = request.Token;
            expression.AllTokens.Add(request.Token);
            expression.ende = kind;
            expression.AllTokens.Add(kind);

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, kind.Position );

            request.Parser.Repleace ( request.Token, kind.Position );

            if ( nodes is null ) return null;
            if ( nodes.Count != 1 ) return null;

            expression.ExpressionParent = nodes[0];

            return expression;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.ExpressionParent is null) return request.Index.CreateError(this);

            return this.ExpressionParent.Indezieren(request);
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.ExpressionParent is null) return false;

            this.ExpressionParent.Compile(request);

            return true;
        }
    }
}