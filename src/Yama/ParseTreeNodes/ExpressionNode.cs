using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class ExpressionNode : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {

        #region get/set

        public IParseTreeNode? ChildNode
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.ChildNode != null) result.Add ( this.ChildNode );

                return result;
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        private ParserLayer expressionLayer;
        private ParserLayer expressionIdenLayer;
        private ParserLayer expressionCallLayer;

        #endregion get/set

        #region ctor

        public ExpressionNode (ParserLayer expressionLayer, ParserLayer expressionIdenLayer, ParserLayer expressionCallLayer)
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.expressionLayer = expressionLayer;
            this.expressionCallLayer = expressionCallLayer;
            this.expressionIdenLayer = expressionIdenLayer;
            this.Token = new();
            this.Ende = new();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            IdentifierToken? operatorToken = request.Token;
            bool isKlammerung = operatorToken.Kind == IdentifierKind.OpenBracket;
            if (isKlammerung) operatorToken = request.Parser.Peek(request.Token, 1);
            if (operatorToken is null) return null;

            ExpressionNode node = new ExpressionNode(this.expressionLayer, this.expressionIdenLayer, this.expressionCallLayer);

            request.Parser.ActivateLayer(this.expressionIdenLayer);

            IParseTreeNode? firstNode = this.ParseFirstNode(request, operatorToken);
            node.ChildNode = firstNode;

            request.Parser.VorherigesLayer();

            if (firstNode is null) return null;
            if (firstNode is IContainer firstNodeContainer) operatorToken = firstNodeContainer.Ende;
            node.Token = operatorToken;

            IdentifierToken? callConvertToken = request.Parser.Peek(operatorToken, 1);
            if (callConvertToken is null) return node;
            if (callConvertToken.Kind == IdentifierKind.EndOfCommand) return node;

            IdentifierToken? operationExpressionToken = this.ParseCallConvertToken(callConvertToken, request, node);
            if (operationExpressionToken is null) return node;
            if (operationExpressionToken.Kind == IdentifierKind.EndOfCommand) return node;

            AssigmentNode node = new AssigmentNode(this.expressionLayer);
            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return null;

            node.ChildNode = request.Parser.ParseCleanToken ( token, this.expressionLayer );

            node.Ende = token;
            if (node is IContainer container) node.Ende = container.Ende;

            return node;
        }

        private IdentifierToken? ParseCallConvertToken(IdentifierToken callConvertToken, RequestParserTreeParser request, ExpressionNode node)
        {
            if (callConvertToken.Kind != IdentifierKind.As
            || callConvertToken.Kind != IdentifierKind.OpenBracket
            || callConvertToken.Kind != IdentifierKind.OpenSquareBracket) return callConvertToken;

            IParseTreeNode? callNode = request.Parser.ParseCleanToken(callConvertToken, this.expressionCallLayer);
            if (callNode is not IParentNode parentNode) return null;
            if (callNode is not IContainer container) return null;

            parentNode.LeftNode = node.ChildNode;
            node.ChildNode = callNode;

            return request.Parser.Peek(container.Ende, 1);
        }

        private IParseTreeNode? ParseFirstNode(RequestParserTreeParser request, IdentifierToken operatorToken)
        {
            IParseTreeNode? operator1ChildRule = request.Parser.GetRule<Operator1ChildRight>();
            if (operator1ChildRule is null)  return null;
            if (operatorToken.Kind == IdentifierKind.Operator) return request.Parser.TryToParse(operator1ChildRule, operatorToken);

            return request.Parser.ParseCleanToken(operatorToken);
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.ChildNode is not IIndexNode rightNode) return request.Index.CreateError(this);

            IndexVariabelnReference? varref = container.VariabelnReferences.LastOrDefault();

            rightNode.Indezieren(request);

            IndexVariabelnReference? indexVariabelnReference = container.VariabelnReferences.LastOrDefault();

            if (varref is null) return false;
            if (indexVariabelnReference is null) return false;

            request.Index.IndexTypeSafeties.Add(new IndexSetValue(varref, indexVariabelnReference));

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.ChildNode is not ICompileNode rightNode) return false;

            return rightNode.Compile(request);
        }

        #endregion methods
    }
}