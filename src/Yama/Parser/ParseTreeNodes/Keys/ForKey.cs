using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ForKey : IParseTreeNode, IContainer
    {

        #region get/set

        public IParseTreeNode Deklaration
        {
            get;
            set;
        }

        public IParseTreeNode Condition
        {
            get;
            set;
        }
        public IParseTreeNode Inkrementation
        {
            get;
            set;
        }
        public IParseTreeNode Statement
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
                return (this.Statement is IContainer t) ? t.Ende : this.Statement.Token;
            }
            set
            {

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

        public IndexContainer IndexContainer
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.For ) return null;
            if ( request.Parser.Peek ( request.Token, 1 ).Kind != IdentifierKind.OpenBracket ) return null;

            ForKey key = new ForKey (  );
            key.Token = request.Token;

            IdentifierToken conditionkind = request.Parser.Peek ( request.Token, 1 );

            IParseTreeNode rule = new Container(IdentifierKind.OpenBracket, IdentifierKind.CloseBracket);

            IParseTreeNode klammer = rule.Parse(new Request.RequestParserTreeParser(request.Parser, conditionkind));

            if (klammer == null) return null;
            if (!(klammer is Container t)) return null;
            if (t.Statements.Count != 3) return null;

            t.Token.ParentNode = key;

            key.Deklaration = t.Statements[0];
            key.Condition = t.Statements[1];
            key.Inkrementation = t.Statements[2];

            IdentifierToken Statementchild = request.Parser.Peek ( t.Ende, 1);

            key.Statement = request.Parser.ParseCleanToken(Statementchild);

            if (key.Statement == null) return null;

            key.Token.Node = key;
            key.Statement.Token.ParentNode = key;

            return key;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

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

            jumpende.Compile(request.Compiler, this.CompileContainer.Ende, "isZero");

            this.Statement.Compile(request);

            jumpbegin.Compile(request.Compiler, this.CompileContainer.Begin, request.Mode);

            this.CompileContainer.Ende.Compile(request.Compiler, this, request.Mode);

            CompileFreeLoop freeLoop = new CompileFreeLoop();
            freeLoop.Compile(request.Compiler, this, request.Mode);

            request.Compiler.PopContainer();

            return true;
        }

        #endregion methods

    }
}