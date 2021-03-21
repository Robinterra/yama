using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class IfKey : IParseTreeNode, IContainer
    {

        #region get/set

        public IParseTreeNode Condition
        {
            get;
            set;
        }

        public IParseTreeNode IfStatement
        {
            get;
            set;
        }

        public IParseTreeNode ElseStatement
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
                if ( this.ElseStatement == null ) return (this.IfStatement is IContainer t) ? t.Ende : this.IfStatement.Token;

                return (this.ElseStatement is IContainer a) ? a.Ende : this.ElseStatement.Token;
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
                if (this.IfStatement != null) result.Add ( this.IfStatement );
                if (this.ElseStatement != null) result.Add ( this.ElseStatement );

                return result;
            }
        }

        public IndexContainer IfContainer
        {
            get;
            set;
        }

        public IndexContainer ElseContainer
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.If ) return null;
            if ( parser.Peek ( token, 1 ).Kind != IdentifierKind.OpenBracket ) return null;

            IfKey key = new IfKey (  );
            key.Token = token;

            IdentifierToken conditionkind = parser.Peek ( token, 1 );

            IParseTreeNode rule = parser.GetRule<ContainerExpression>();

            key.Condition = rule.Parse(parser, conditionkind);

            if (key.Condition == null) return null;

            key.Condition.Token.ParentNode = key;

            IdentifierToken ifStatementchild = parser.Peek ( ((ContainerExpression)key.Condition).Ende, 1);

            if (!this.IsAllowedStatmentToken (ifStatementchild)) return null;

            key.IfStatement = parser.ParseCleanToken(ifStatementchild);

            if (key.IfStatement == null) return null;

            key.IfStatement.Token.ParentNode = key;

            IdentifierToken elsePeekToken = (key.IfStatement is IContainer t) ? t.Ende : key.IfStatement.Token;

            IdentifierToken elseStatementChild = parser.Peek ( elsePeekToken, 1 );

            key.Token.Node = key;

            if ( elseStatementChild == null ) return key;

            if ( elseStatementChild.Node != null ) return key;

            if ( elseStatementChild.Kind != IdentifierKind.Else ) return key;

            key.ElseStatement = parser.ParseCleanToken ( elseStatementChild );

            if (key.ElseStatement == null) return null;

            key.ElseStatement.Token.ParentNode = key;

            return key;
        }

        private bool IsAllowedStatmentToken(IdentifierToken ifStatementchild)
        {
            if (ifStatementchild.Kind == IdentifierKind.Return) return true;
            if (ifStatementchild.Kind == IdentifierKind.Break) return true;
            if (ifStatementchild.Kind == IdentifierKind.Continue) return true;
            if (ifStatementchild.Kind == IdentifierKind.BeginContainer) return true;
            if (ifStatementchild.Kind == IdentifierKind.If) return true;
            if (ifStatementchild.Kind == IdentifierKind.While) return true;
            if (ifStatementchild.Kind == IdentifierKind.For) return true;

            return false;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            this.IfContainer = container;
            this.ElseContainer = container;

            if (this.ElseStatement != null)
            {
                this.ElseStatement.Indezieren(index, parent);
                if (this.ElseStatement is Container ec) this.ElseContainer = ec.IndexContainer;
            }

            this.IfStatement.Indezieren(index, parent);
            if (this.IfStatement is Container c) this.IfContainer = c.IndexContainer;

            this.Condition.Indezieren(index, parent);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            CompileContainer ifcontainer = new CompileContainer();
            ifcontainer.Begin = compiler.ContainerMgmt.CurrentContainer.Begin;
            ifcontainer.Ende = new CompileSprungPunkt();

            CompileContainer elsecontainer = new CompileContainer();
            elsecontainer.Begin = compiler.ContainerMgmt.CurrentContainer.Begin;
            elsecontainer.Ende = new CompileSprungPunkt();

            CompileJumpTo jumpafterelse = new CompileJumpTo();

            CompileJumpWithCondition jumpWithCondition = new CompileJumpWithCondition();

            this.Condition.Compile(compiler, mode);

            jumpWithCondition.Compile(compiler, ifcontainer.Ende, "isZero");

            compiler.PushContainer(ifcontainer, this.IfContainer.ThisUses);

            this.IfStatement.Compile(compiler, mode);

            compiler.PopContainer();

            if (this.ElseStatement != null) jumpafterelse.Compile(compiler, elsecontainer.Ende, mode);

            ifcontainer.Ende.Compile(compiler, this, mode);

            if (this.ElseStatement == null) return true;

            compiler.PushContainer(elsecontainer, this.ElseContainer.ThisUses);

            this.ElseStatement.Compile(compiler, mode);

            compiler.PopContainer();

            elsecontainer.Ende.Compile(compiler, this, mode);

            return true;
        }

        #endregion methods

    }
}