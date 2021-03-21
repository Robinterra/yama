using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class WhileKey : IParseTreeNode, IContainer
    {

        #region get/set

        public IParseTreeNode Condition
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

        public IndexContainer IndexContainer
        {
            get;
            set;
        }

        #endregion get/set

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

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.While ) return null;
            if ( parser.Peek ( token, 1 ).Kind != IdentifierKind.OpenBracket ) return null;

            WhileKey key = new WhileKey (  );
            key.Token = token;

            IdentifierToken conditionkind = parser.Peek ( token, 1 );

            IParseTreeNode rule = parser.GetRule<ContainerExpression>();

            key.Condition = rule.Parse(parser, conditionkind);

            if (key.Condition == null) return null;

            key.Condition.Token.ParentNode = key;

            IdentifierToken Statementchild = parser.Peek ( ((ContainerExpression)key.Condition).Ende, 1);
            if (!this.IsAllowedStatmentToken (Statementchild)) return null;

            key.Statement = parser.ParseCleanToken(Statementchild);

            if (key.Statement == null) return null;

            key.Statement.Token.ParentNode = key;
            key.Token.Node = key;

            return key;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            this.IndexContainer =  container;

            this.Statement.Indezieren(index, parent);
            if (this.Statement is Container ec) this.IndexContainer = ec.IndexContainer;

            this.Condition.Indezieren(index, parent);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.CompileContainer.Begin = new CompileSprungPunkt();

            this.CompileContainer.Ende = new CompileSprungPunkt();

            compiler.PushContainer(this.CompileContainer, this.IndexContainer.ThisUses, true);

            CompileJumpTo jumpbegin = new CompileJumpTo();

            CompileJumpWithCondition jumpende = new CompileJumpWithCondition();

            this.CompileContainer.Begin.Compile(compiler, this, mode);

            //compiler.IsLoopHeaderBegin = true;

            this.Condition.Compile(compiler, mode);

            //compiler.IsLoopHeaderBegin = false;

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