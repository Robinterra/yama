using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ForKey : IParseTreeNode, IContainer
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

        #endregion get/set

        #region ctor

        public ForKey ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.For ) return null;

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return null;
            if ( token.Kind != IdentifierKind.OpenBracket ) return null;

            ForKey key = new ForKey (  );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken? conditionkind = request.Parser.Peek ( request.Token, 1 );
            if (conditionkind is null) return null;

            IParseTreeNode rule = new Container(IdentifierKind.OpenBracket, IdentifierKind.CloseBracket);

            IParseTreeNode? klammer = request.Parser.TryToParse ( rule, conditionkind );

            if (klammer is not Container t) return null;
            if (t.Statements.Count != 3) return null;

            t.Token.ParentNode = key;

            key.Deklaration = t.Statements[0];
            key.Condition = t.Statements[1];
            key.Inkrementation = t.Statements[2];

            IdentifierToken? statementchild = request.Parser.Peek ( t.Ende, 1);
            if (statementchild is null) return null;

            IParseTreeNode? statement = request.Parser.ParseCleanToken(statementchild);
            if (statement is null) return null;

            key.Statement = statement;

            return key;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Statement is null) return request.Index.CreateError(this);
            if (this.Inkrementation is null) return request.Index.CreateError(this);
            if (this.Condition is null) return request.Index.CreateError(this);
            if (this.Deklaration is null) return request.Index.CreateError(this);

            this.IndexContainer = container;

            this.Statement.Indezieren(request);
            if (this.Statement is Container ec) this.IndexContainer = ec.IndexContainer;
            this.Inkrementation.Indezieren(request);
            this.Condition.Indezieren(request);
            this.Deklaration.Indezieren(request);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.Statement is null) return false;
            if (this.Inkrementation is null) return false;
            if (this.Condition is null) return false;
            if (this.Deklaration is null) return false;
            if (this.IndexContainer is null) return false;

            this.CompileContainer.Begin = new CompileSprungPunkt();

            this.CompileContainer.Ende = new CompileSprungPunkt();

            CompileSprungPunkt sprungPunktSkipInc = new CompileSprungPunkt();

            this.Deklaration.Compile(request);

            request.Compiler.PushContainer(this.CompileContainer, this.IndexContainer.ThisUses, true);

            CompileJumpTo jumpbegin = new CompileJumpTo();

            CompileJumpTo jumpSkipInc = new CompileJumpTo();

            CompileJumpWithCondition jumpende = new CompileJumpWithCondition();

            jumpSkipInc.Compile(request.Compiler, sprungPunktSkipInc, request.Mode);

            this.CompileContainer.Begin.Compile(request.Compiler, this, request.Mode);

            this.Inkrementation.Compile(request);

            sprungPunktSkipInc.Compile(request.Compiler, this, request.Mode);

            this.Condition.Compile(request);

            this.CompileContainer.Ende.Node = this;
            jumpende.Compile(request.Compiler, this.CompileContainer.Ende, "isZero");

            this.Statement.Compile(request);

            jumpbegin.Compile(request.Compiler, this.CompileContainer.Begin, request.Mode);

            this.CompileContainer.Ende.Compile(request.Compiler, this, request.Mode);

            CompileFreeLoop freeLoop = new CompileFreeLoop();
            freeLoop.Begin = this.CompileContainer.Begin.Line;
            freeLoop.Compile(request.Compiler, this, request.Mode);

            request.Compiler.PopContainer();

            return true;
        }

        #endregion methods

    }
}