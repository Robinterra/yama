using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ForKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {

        #region get/set

        public IParseTreeNode? Deklaration
        {
            get;
            set;
        }

        public IParseTreeNode? Condition
        {
            get;
            set;
        }

        public IParseTreeNode? Inkrementation
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

                if (this.Deklaration != null) result.Add ( this.Deklaration );
                if (this.Condition != null) result.Add ( this.Condition );
                if (this.Inkrementation != null) result.Add ( this.Inkrementation );
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

        private ParserLayer expressionLayer;

        #endregion get/set

        #region ctor

        public ForKey (ParserLayer expressionLayer)
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.expressionLayer = expressionLayer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.For ) return null;

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return new ParserError(request.Token, $"Expectet a open Bracket '(' after a 'for' Keyword {request.Token.Kind}");
            if ( token.Kind != IdentifierKind.OpenBracket ) return new ParserError(token, $"Expectet a open Bracket '(' after Keyword 'for' and not a {token.Kind}", request.Token);

            ForKey key = new ForKey ( this.expressionLayer );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken? conditionToken = request.Parser.Peek ( request.Token, 1 );
            if (conditionToken is null) return new ParserError(request.Token, $"Expectet a begin of a Condition after '('", token);

            IParseTreeNode rule = new Container(IdentifierKind.OpenBracket, IdentifierKind.CloseBracket);

            IParseTreeNode? klammer = request.Parser.TryToParse ( rule, conditionToken );

            if (klammer is not Container t) return new ParserError(conditionToken, $"Can not parse Condition of a for", token, request.Token);
            if (t.Statements.Count != 3) return new ParserError(conditionToken, $"Can not parse Condition of a for, expected 3 statments and not {t.Statements.Count}", token, request.Token);

            t.Token.ParentNode = key;

            key.Deklaration = t.Statements[0];
            key.Condition = t.Statements[1];
            key.Inkrementation = t.Statements[2];

            IdentifierToken? statementchild = request.Parser.Peek ( t.Ende, 1);
            if (statementchild is null) return new ParserError(request.Token, $"Can not find a Statement after a for", token, conditionToken);

            IParseTreeNode? statement = request.Parser.ParseCleanToken(statementchild);
            if (statement is null) return new ParserError(statementchild, $"for statement can not be parse", token, conditionToken, request.Token);

            key.Statement = statement;

            return key;
        }

        /*public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.For ) return null;

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return new ParserError(request.Token, $"Expectet a open Bracket '(' after a 'for' Keyword {request.Token.Kind}");
            if ( token.Kind != IdentifierKind.OpenBracket ) return new ParserError(token, $"Expectet a open Bracket '(' after Keyword 'for' and not a {token.Kind}", request.Token);

            ForKey key = new ForKey ( this.expressionLayer );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);
            key.AllTokens.Add(token);

            IdentifierToken? conditionToken = request.Parser.Peek ( token, 1 );
            if (conditionToken is null) return new ParserError(request.Token, $"Expectet a begin of a Condition after '('", token);

            IParseTreeNode? rule = request.Parser.GetRule<NormalStatementNode>();
            if (rule is null) return null;

            key.Deklaration = request.Parser.TryToParse(rule, conditionToken);
            if (key.Deklaration is not NormalStatementNode deklaration) return null;

            token = request.Parser.Peek ( deklaration.Ende, 1 );
            if (token is null) return null;
            if (token.Kind != IdentifierKind.EndOfCommand) return null;

            token = request.Parser.Peek ( token, 1 );
            if (token is null) return null;

            key.Condition = request.Parser.ParseCleanToken(token, this.expressionLayer);

            key.Inkrementation = request.Parser.TryToParse(rule, token);

            IdentifierToken? statementchild = request.Parser.Peek ( token, 1);
            if (statementchild is null) return new ParserError(request.Token, $"Can not find a Statement after a for", token, conditionToken);

            IParseTreeNode? statement = request.Parser.ParseCleanToken(statementchild);
            if (statement is null) return new ParserError(statementchild, $"for statement can not be parse", token, conditionToken, request.Token);

            key.Statement = statement;

            return key;
        }*/

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Statement is null) return request.Index.CreateError(this);

            this.IndexContainer = container;

            if (this.Statement is IIndexNode statementIndexNode) statementIndexNode.Indezieren(request);
            if (this.Statement is Container ec) this.IndexContainer = ec.IndexContainer;

            if (this.Inkrementation is IIndexNode inkrementationIndexNode) inkrementationIndexNode.Indezieren(request);
            if (this.Condition is IIndexNode conditionIndexNode) conditionIndexNode.Indezieren(request);
            if (this.Deklaration is IIndexNode deklarationIndexNode) deklarationIndexNode.Indezieren(request);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Statement is null) return false;
            if (this.IndexContainer is null) return false;

            this.CompileContainer.Begin = new CompileSprungPunkt();

            this.CompileContainer.Ende = new CompileSprungPunkt();

            CompileSprungPunkt sprungPunktSkipInc = new CompileSprungPunkt();

            if (this.Deklaration is ICompileNode deklarationNode) deklarationNode.Compile(request);

            request.Compiler.PushContainer(this.CompileContainer, this.IndexContainer.ThisUses, true);

            CompileJumpTo jumpbegin = new CompileJumpTo();

            CompileJumpTo jumpSkipInc = new CompileJumpTo();

            CompileJumpWithCondition jumpende = new CompileJumpWithCondition();

            jumpSkipInc.Compile(request.Compiler, sprungPunktSkipInc, request.Mode);

            this.CompileContainer.Begin.Compile(request.Compiler, this, request.Mode);

            if (this.Inkrementation is ICompileNode inkrementationNode) inkrementationNode.Compile(request);

            sprungPunktSkipInc.Compile(request.Compiler, this, request.Mode);

            if (this.Condition is ICompileNode conditionNode) conditionNode.Compile(request);

            this.CompileContainer.Ende.Node = this;
            jumpende.Compile(request.Compiler, this.CompileContainer.Ende, "isZero");

            if (this.Statement is ICompileNode statementNode) statementNode.Compile(request);

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