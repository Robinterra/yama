using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class WhileKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {

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

        public WhileKey ()
        {
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

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return new ParserError(request.Token, $"Expectet a open Bracket '(' after a 'while' Keyword {request.Token.Kind}");
            if (token.Kind != IdentifierKind.OpenBracket) return new ParserError(token, $"Expectet a open Bracket '(' after Keyword 'while' and not a {token.Kind}", request.Token);

            WhileKey key = new WhileKey (  );
            key.Token = request.Token;
            key.AllTokens.Add ( request.Token );

            IdentifierToken? conditionToken = request.Parser.Peek ( request.Token, 1 );
            IParseTreeNode? rule = request.Parser.GetRule<ContainerExpression>();

            if (conditionToken is null) return new ParserError(request.Token, $"Expectet a begin of a Condition after '('", token);
            if (rule is null) return null;

            key.Condition = request.Parser.TryToParse ( rule, conditionToken );

            if (key.Condition is null) return new ParserError(request.Token, $"Expectet a begin of a Condition after '('", token);

            IdentifierToken? statementchild = request.Parser.Peek ( ((ContainerExpression)key.Condition).Ende, 1);
            if (statementchild is null) return new ParserError(request.Token, $"Can not find a Statement after a while", token, conditionToken);

            if (!this.IsAllowedStatmentToken (statementchild)) return new ParserError(statementchild, $"Can not find a allowd Statement after a while", token, conditionToken, request.Token);

            key.Statement = request.Parser.ParseCleanToken(statementchild);
            if (key.Statement is null) return new ParserError(statementchild, $"while statement can not be parse", token, conditionToken, request.Token);

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

            jumpbegin.Compile(request.Compiler, this.CompileContainer.Begin, request.Mode);

            this.CompileContainer.Ende.Compile(request.Compiler, this, request.Mode);

            if (this.CompileContainer.Begin.Line is null) return false;
            CompileFreeLoop freeLoop = new CompileFreeLoop(this.CompileContainer.Begin.Line);

            freeLoop.Compile(request.Compiler, this, request.Mode);

            request.Compiler.PopContainer();

            return true;
        }

        #endregion methods

    }
}