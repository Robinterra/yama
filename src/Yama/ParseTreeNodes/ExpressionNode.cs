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
            get
            {
                if (this.isCloseBracketEnde)
                {
                    if (this.ChildNode is IContainer cont && cont.Token.Position > this.CloseBracket!.Position) return cont.Ende;

                    return this.CloseBracket!;
                }

                if (this.ChildNode is IContainer con) return con.Ende;
                if (this.ChildNode is null) return this.Token;

                return this.ChildNode.Token;
            }
        }

        public IdentifierToken? CloseBracket
        {
            get;
            set;
        }

        private bool isCloseBracketEnde;

        private ParserLayer expressionIdenLayer;

        private ParserLayer expressionCallLayer;

        private ParserLayer operationExpressionLayer;

        #endregion get/set

        #region ctor

        public ExpressionNode (ParserLayer expressionIdenLayer, ParserLayer expressionCallLayer, ParserLayer operationExpressionLayer)
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.expressionCallLayer = expressionCallLayer;
            this.expressionIdenLayer = expressionIdenLayer;
            this.operationExpressionLayer = operationExpressionLayer;
            this.Token = new();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            IdentifierToken? expressionIdenToken = request.Token;
            bool isKlammerung = expressionIdenToken.Kind == IdentifierKind.OpenBracket;
            if (isKlammerung) expressionIdenToken = request.Parser.Peek(request.Token, 1);
            if (expressionIdenToken is null) return null;

            ExpressionNode node = new ExpressionNode(this.expressionIdenLayer, this.expressionCallLayer, operationExpressionLayer);

            IParseTreeNode? firstNode = this.ParseFirstNode(request, expressionIdenToken);
            node.ChildNode = firstNode;

            if (firstNode is null) return null;
            if (firstNode is IContainer firstNodeContainer) expressionIdenToken = firstNodeContainer.Ende;
            node.Token = request.Token;

            IdentifierToken? callConvertToken = request.Parser.Peek(expressionIdenToken, 1);
            if (callConvertToken is null) return this.GetResult(isKlammerung, node);
            if (callConvertToken.Kind == IdentifierKind.EndOfCommand) return this.GetResult(isKlammerung, node);

            IdentifierToken? operationExpressionToken = this.ParseCallConvertToken(callConvertToken, request, node, firstNode);
            if (node.ChildNode is null) return null;
            if (operationExpressionToken is null) return this.GetResult(isKlammerung, node);
            if (operationExpressionToken.Kind == IdentifierKind.EndOfCommand) return this.GetResult(isKlammerung, node);

            IdentifierToken? maybeCloseBracket = this.ParseOperationExpressionToken(operationExpressionToken, request, node, node.ChildNode);
            if (node.ChildNode is null) return null;
            if (maybeCloseBracket is null) return this.GetResult(isKlammerung, node);
            if (maybeCloseBracket.Kind == IdentifierKind.Comma) node.AllTokens.Add(maybeCloseBracket);
            if (!isKlammerung) return this.GetResult(false, node);
            if (maybeCloseBracket.Kind != IdentifierKind.CloseBracket) return new ParserError(maybeCloseBracket, "Expected a ')' and not a", node.AllTokens.ToArray());

            node.AllTokens.Add(request.Token);
            node.CloseBracket = maybeCloseBracket;
            node.AllTokens.Add(maybeCloseBracket);
            node.isCloseBracketEnde = true;

            IdentifierToken? maybeAsExpression = request.Parser.Peek(maybeCloseBracket, 1);
            if (maybeAsExpression is null) return this.GetResult(isKlammerung, node);

            IdentifierToken? maybeOperationExpression = this.TryAsExpression(request, maybeAsExpression, node, node.ChildNode);
            if (node.ChildNode is null) return null;
            if (maybeOperationExpression is null) return node;

            IdentifierToken? maybeComma = this.ParseOperationExpressionToken(maybeOperationExpression, request, node, node.ChildNode);
            if (maybeComma is null) return node;
            if (maybeComma.Kind != IdentifierKind.Comma) return node;

            node.AllTokens.Add(maybeComma);

            return node;
        }

        private IdentifierToken? TryAsExpression(RequestParserTreeParser request, IdentifierToken maybeAsExpression, ExpressionNode node, IParseTreeNode childNode)
        {
            if (maybeAsExpression.Kind != IdentifierKind.As) return maybeAsExpression;

            IParseTreeNode? rule = request.Parser.GetRule<ExplicitlyConvert>();
            if (rule is null) return null;

            IParseTreeNode? callNode = request.Parser.TryToParse(rule, maybeAsExpression);
            if (callNode is not IParentNode parentNode) return null;
            if (callNode is not IContainer container) return null;

            node.ChildNode = request.Parser.SetChild(parentNode, childNode);

            return request.Parser.Peek(container.Ende, 1);
        }

        private IParseTreeNode? GetResult(bool isKlammerung, ExpressionNode node)
        {
            if (isKlammerung && !node.isCloseBracketEnde) return new ParserError(node.Token, "missing close ')' bracket", node.AllTokens.ToArray());

            if (isKlammerung) return node;
            if (node.ChildNode is Operator2Childs) return node.ChildNode;

            return node;
        }

        private IdentifierToken? ParseOperationExpressionToken(IdentifierToken operationExpressionToken, RequestParserTreeParser request, ExpressionNode node, IParseTreeNode childNode)
        {
            if (operationExpressionToken.Kind != IdentifierKind.Operator) return operationExpressionToken;

            node.isCloseBracketEnde = false;

            IParseTreeNode? callNode = request.Parser.ParseCleanToken(operationExpressionToken, this.operationExpressionLayer, true);
            if (callNode is not Operator2Childs opNode) return null;

            request.Parser.SetChild(opNode, childNode);

            Operator2Childs newParent = CleanUpOperation2Childs(opNode, childNode, request);
            node.ChildNode = newParent;

            return request.Parser.Peek(newParent.Ende, 1);
        }

        private Operator2Childs CleanUpOperation2Childs(Operator2Childs opNode, IParseTreeNode childNode, RequestParserTreeParser request)
        {
            if (opNode.RightNode is not Operator2Childs rightNode) return opNode;
            if (rightNode.Prio >= opNode.Prio) return opNode;

            opNode.RightNode = rightNode.LeftNode;
            if (opNode.RightNode is not null) opNode.RightNode.Token.ParentNode = opNode;

            request.Parser.SetChild(rightNode, opNode);

            return rightNode;
        }

        private IdentifierToken? ParseCallConvertToken(IdentifierToken callConvertToken, RequestParserTreeParser request, ExpressionNode node, IParseTreeNode firstNode)
        {
            if (callConvertToken.Kind != IdentifierKind.As
            && callConvertToken.Kind != IdentifierKind.OpenBracket
            && callConvertToken.Kind != IdentifierKind.OpenSquareBracket
            && callConvertToken.Kind != IdentifierKind.Is) return callConvertToken;

            IParseTreeNode? callNode = request.Parser.ParseCleanToken(callConvertToken, this.expressionCallLayer, true);
            if (callNode is not IParentNode parentNode) return null;
            if (callNode is not IContainer container) return null;

            node.ChildNode = request.Parser.SetChild(parentNode, firstNode);

            return request.Parser.Peek(container.Ende, 1);
        }

        private IParseTreeNode? ParseFirstNode(RequestParserTreeParser request, IdentifierToken operatorToken)
        {
            return request.Parser.ParseCleanToken(operatorToken, this.expressionIdenLayer, true);
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.ChildNode is not IIndexNode rightNode) return request.Index.CreateError(this);

            return rightNode.Indezieren(request);
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.ChildNode is not ICompileNode rightNode) return false;

            return rightNode.Compile(request);
        }

        #endregion methods
    }
}