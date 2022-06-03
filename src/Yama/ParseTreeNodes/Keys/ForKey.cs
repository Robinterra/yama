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

            IdentifierToken? openBrackettoken = request.Parser.Peek ( request.Token, 1 );
            if (openBrackettoken is null) return new ParserError(request.Token, $"Expectet a open Bracket '(' after a 'for' Keyword {request.Token.Kind}");
            if ( openBrackettoken.Kind != IdentifierKind.OpenBracket ) return new ParserError(openBrackettoken, $"Expectet a open Bracket '(' after Keyword 'for' and not a {openBrackettoken.Kind}", request.Token);

            ForKey key = new ForKey ( this.expressionLayer );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);
            key.AllTokens.Add(openBrackettoken);

            IdentifierToken? deklarationToken = request.Parser.Peek ( openBrackettoken, 1 );
            if (deklarationToken is null) return new ParserError(request.Token, $"Expectet a begin of a Condition after '('", openBrackettoken);

            NormalStatementNode normalStatementRule = request.Parser.GetRule<NormalStatementNode>();

            NormalStatementNode? deklaration = request.Parser.TryToParse(normalStatementRule, deklarationToken);
            if (deklaration is null) return new ParserError(request.Token, $"Can not parse Condition of a for", openBrackettoken);
            key.Deklaration = deklaration;

            IdentifierToken? conditionToken = request.Parser.Peek ( deklaration.Ende, 1 );
            if (conditionToken is null) return null;

            IParseTreeNode? condition = request.Parser.ParseCleanToken(conditionToken, this.expressionLayer, false);
            if (condition is not IContainer conCon) return new ParserError(request.Token, $"Can not parse Condition of a for", openBrackettoken);
            key.Condition = condition;

            IdentifierToken? semikolonToken = request.Parser.Peek ( conCon.Ende, 1 );
            if (semikolonToken is null) return new ParserError(request.Token, $"Can not parse Condition of a for, missing ';'", openBrackettoken);
            key.AllTokens.Add(semikolonToken);

            IdentifierToken? inkrementToken = request.Parser.Peek ( semikolonToken, 1 );
            if (inkrementToken is null) return null;

            NormalStatementNode? inkrement = request.Parser.TryToParse(normalStatementRule, inkrementToken);
            if (inkrement is null) return new ParserError(request.Token, $"Can not parse inkrement of a for", openBrackettoken);
            key.Inkrementation = inkrement;

            IdentifierToken? closeBracket = request.Parser.Peek ( inkrement.Ende, 1);
            if (closeBracket is null) return null;
            if (closeBracket.Kind != IdentifierKind.CloseBracket) return new ParserError(request.Token, $"Expected a ')' and not a '{closeBracket.Text}' on the", key.GetAllChilds, request.Token, openBrackettoken, semikolonToken);
            key.AllTokens.Add(closeBracket);

            IdentifierToken? statementchild = request.Parser.Peek ( closeBracket, 1);
            if (statementchild is null) return new ParserError(request.Token, $"Can not find a Statement after a for", openBrackettoken, deklarationToken);

            IParseTreeNode? statement = request.Parser.ParseCleanToken(statementchild);
            if (statement is null) return new ParserError(request.Token, $"for statement can not be parse", openBrackettoken);

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

            IEnumerable<KeyValuePair<string, SSAVariableMap>> variableMaps = request.Compiler.PopContainerAndReturnVariableMapperForLoops();

            CompilePhi compilePhis = new CompilePhi();
            compilePhis.Compile(request.Compiler, variableMaps, this);

            return true;
        }

        #endregion methods

    }
}