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

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.For ) return null;
            if ( parser.Peek ( token, 1 ).Kind != IdentifierKind.OpenBracket ) return null;

            ForKey key = new ForKey (  );
            key.Token = token;
            token.Node = key;

            IdentifierToken conditionkind = parser.Peek ( token, 1 );

            IParseTreeNode rule = new Container(IdentifierKind.OpenBracket, IdentifierKind.CloseBracket);

            IParseTreeNode klammer = rule.Parse(parser, conditionkind);

            if (klammer == null) return null;
            if (!(klammer is Container t)) return null;
            if (t.Statements.Count != 3) return null;

            t.Token.ParentNode = key;

            key.Deklaration = t.Statements[0];
            key.Condition = t.Statements[1];
            key.Inkrementation = t.Statements[2];

            IdentifierToken Statementchild = parser.Peek ( t.Ende, 1);

            key.Statement = parser.ParseCleanToken(Statementchild);

            if (key.Statement == null) return null;

            key.Statement.Token.ParentNode = key;

            return key;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            this.IndexContainer = container;

            this.Statement.Indezieren(index, parent);
            if (this.Statement is Container ec) this.IndexContainer = ec.IndexContainer;
            this.Inkrementation.Indezieren(index, parent);
            this.Condition.Indezieren(index, parent);
            this.Deklaration.Indezieren(index, parent);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.CompileContainer.Begin = new CompileSprungPunkt();

            this.CompileContainer.Ende = new CompileSprungPunkt();

            CompileSprungPunkt sprungPunktSkipInc = new CompileSprungPunkt();

            this.Deklaration.Compile(compiler, mode);

            compiler.PushContainer(this.CompileContainer, this.IndexContainer.ThisUses, true);

            CompileJumpTo jumpbegin = new CompileJumpTo();

            CompileJumpTo jumpSkipInc = new CompileJumpTo();

            CompileJumpWithCondition jumpende = new CompileJumpWithCondition();

            jumpSkipInc.Compile(compiler, sprungPunktSkipInc, mode);

            this.CompileContainer.Begin.Compile(compiler, this, mode);

            this.Inkrementation.Compile(compiler, mode);

            sprungPunktSkipInc.Compile(compiler, this, mode);

            this.Condition.Compile(compiler, mode);

            jumpende.Compile(compiler, this.CompileContainer.Ende, "isZero");

            this.Statement.Compile(compiler, mode);

            jumpbegin.Compile(compiler, this.CompileContainer.Begin, mode);

            this.CompileContainer.Ende.Compile(compiler, this, mode);

            CompileFreeLoop freeLoop = new CompileFreeLoop();
            freeLoop.Compile(compiler, this, mode);

            compiler.PopContainer();

            return true;
        }

        #endregion methods

    }
}