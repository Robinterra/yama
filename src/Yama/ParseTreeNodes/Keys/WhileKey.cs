using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class WhileKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {

        #region vars

        private ParserLayer expressionLayer;

        #endregion vars

        #region get/set

        public IParseTreeNode? Condition
        {
            get;
            set;
        }

        public IParseTreeNode? Statement
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IdentifierToken Ende
        {
            get
            {
                if (this.Statement is null) return this.Token;

                return (this.Statement is IContainer t) ? t.Ende : this.Statement.Token;
            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.Condition != null) result.Add ( this.Condition );
                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        public CompileContainer CompileContainer
        {
            get;
            set;
        } = new CompileContainer();

        public IndexContainer? IndexContainer
        {
            get;
            set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public WhileKey (ParserLayer expressionLayer)
        {
            this.expressionLayer = expressionLayer;
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        private bool IsAllowedStatmentToken(IdentifierToken ifStatementchild)
        {
            if (ifStatementchild.Kind == IdentifierKind.Continue) return true;
            if (ifStatementchild.Kind == IdentifierKind.BeginContainer) return true;
            if (ifStatementchild.Kind == IdentifierKind.If) return true;
            if (ifStatementchild.Kind == IdentifierKind.While) return true;
            if (ifStatementchild.Kind == IdentifierKind.For) return true;

            return false;
        }

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.While ) return null;

            IdentifierToken? openBrackettoken = request.Parser.Peek ( request.Token, 1 );
            if (openBrackettoken is null) return new ParserError(request.Token, $"Expectet a open Bracket '(' after a 'while' Keyword {request.Token.Kind}");
            if (openBrackettoken.Kind != IdentifierKind.OpenBracket) return new ParserError(openBrackettoken, $"Expectet a open Bracket '(' after Keyword 'while' and not a {openBrackettoken.Kind}", request.Token);

            WhileKey key = new WhileKey ( this.expressionLayer );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);
            key.AllTokens.Add(openBrackettoken);

            IdentifierToken? conditionToken = request.Parser.Peek ( openBrackettoken, 1 );
            if (conditionToken is null) return new ParserError(request.Token, $"Expectet a begin of a Condition after '('", openBrackettoken);

            IParseTreeNode? condition = request.Parser.ParseCleanToken(conditionToken, this.expressionLayer, false);
            if (condition is not IContainer conCon) return new ParserError(request.Token, $"Can not parse Condition of a while", openBrackettoken);
            key.Condition = condition;

            IdentifierToken? closeBracket = request.Parser.Peek ( conCon.Ende, 1);
            if (closeBracket is null) return null;
            if (closeBracket.Kind != IdentifierKind.CloseBracket) return new ParserError(closeBracket, $"Expected ) and not", request.Token, openBrackettoken);
            key.AllTokens.Add(closeBracket);

            IdentifierToken? statementchild = request.Parser.Peek ( closeBracket, 1);
            if (statementchild is null) return new ParserError(request.Token, $"Can not find a Statement after a while", openBrackettoken, conditionToken);

            if (!this.IsAllowedStatmentToken (statementchild)) return new ParserError(statementchild, $"Can not find a allowd Statement after a while", openBrackettoken, conditionToken, request.Token);

            IParseTreeNode? statementNode = request.Parser.ParseCleanToken(statementchild);
            if (statementNode is null) return new ParserError(statementchild, $"while statement can not be parse", openBrackettoken, conditionToken, request.Token);

            key.Statement = statementNode;

            return key;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Statement is not IIndexNode statementNode) return request.Index.CreateError(this);
            if (this.Condition is not IIndexNode conditionNode) return request.Index.CreateError(this);

            this.IndexContainer =  container;

            statementNode.Indezieren(request);
            if (this.Statement is Container ec) this.IndexContainer = ec.IndexContainer;

            conditionNode.Indezieren(request);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.IndexContainer is null) return false;
            if (this.Condition is not ICompileNode conditionNode) return false;
            if (this.Statement is not ICompileNode statementNode) return false;

            this.CompileContainer.Begin = new CompileSprungPunkt();
            this.CompileContainer.Ende = new CompileSprungPunkt();
            this.CompileContainer.CurrentNode = this;

            request.Compiler.PushContainer(this.CompileContainer, this.IndexContainer.ThisUses, true);

            CompileJumpTo jumpbegin = new CompileJumpTo();

            CompileJumpWithCondition jumpende = new CompileJumpWithCondition();

            this.CompileContainer.Begin.Compile(request.Compiler, this, request.Mode);

            //compiler.IsLoopHeaderBegin = true;

            conditionNode.Compile(request);

            //compiler.IsLoopHeaderBegin = false;

            this.CompileContainer.Ende.Node = this;
            jumpende.Compile(request.Compiler, this.CompileContainer.Ende, "isZero");

            statementNode.Compile(request);

            CompileCleanMemory cleanMemory = new CompileCleanMemory();
            cleanMemory.Compile(request.Compiler, this);

            jumpbegin.Compile(request.Compiler, this.CompileContainer.Begin, request.Mode);

            this.CompileContainer.Ende.Compile(request.Compiler, this, request.Mode);

            if (this.CompileContainer.Begin.Line is null) return false;
            CompileFreeLoop freeLoop = new CompileFreeLoop(this.CompileContainer.Begin.Line);

            freeLoop.Compile(request.Compiler, this, request.Mode);

            request.Compiler.PopContainerForLoops(freeLoop.Phis);

            return true;
        }

        #endregion methods

    }
}