using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class EnumartionExpression : IParseTreeNode, IIndexNode, ICompileNode, IEndExpression
    {

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

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                if (this.ExpressionParent is null) return new();

                return new List<IParseTreeNode> { this.ExpressionParent };
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public EnumartionExpression ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Comma ) return null;

            IdentifierToken? left = request.Parser.Peek ( request.Token, -1 );

            EnumartionExpression expression = new EnumartionExpression (  );

            expression.Token = request.Token;
            expression.AllTokens.Add(request.Token);

            if (left is null) return expression;

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens (left, request.Parser.Start, request.Token.Position );

            if ( nodes is null ) return null;
            if ( nodes.Count > 1 ) return null;

            expression.ExpressionParent = nodes.FirstOrDefault();

            return expression;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (this.ExpressionParent is not IIndexNode expressionNode) return true;

            return expressionNode.Indezieren(request);
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.ExpressionParent is not ICompileNode compilenode) return true;

            compilenode.Compile(request);

            return true;
        }
    }
}