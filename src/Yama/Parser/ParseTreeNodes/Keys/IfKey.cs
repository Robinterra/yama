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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public IfKey ()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.If ) return null;
            if ( request.Parser.Peek ( request.Token, 1 ).Kind != IdentifierKind.OpenBracket ) return null;

            IfKey key = new IfKey (  );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken conditionkind = request.Parser.Peek ( request.Token, 1 );

            IParseTreeNode rule = request.Parser.GetRule<ContainerExpression>();

            key.Condition = request.Parser.TryToParse ( rule, conditionkind );

            if (key.Condition == null) return null;

            IdentifierToken ifStatementchild = request.Parser.Peek ( ((ContainerExpression)key.Condition).Ende, 1);

            if (!this.IsAllowedStatmentToken (ifStatementchild)) return null;

            key.IfStatement = request.Parser.ParseCleanToken(ifStatementchild);

            if (key.IfStatement == null) return null;

            IdentifierToken elsePeekToken = (key.IfStatement is IContainer t) ? t.Ende : key.IfStatement.Token;

            IdentifierToken elseStatementChild = request.Parser.Peek ( elsePeekToken, 1 );

            if ( elseStatementChild == null ) return key;

            if ( elseStatementChild.Node != null ) return key;

            if ( elseStatementChild.Kind != IdentifierKind.Else ) return key;

            key.ElseStatement = request.Parser.ParseCleanToken ( elseStatementChild );

            if (key.ElseStatement == null) return null;

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

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            this.IfContainer = container;
            this.ElseContainer = container;

            if (this.ElseStatement != null)
            {
                this.ElseStatement.Indezieren(request);
                if (this.ElseStatement is Container ec) this.ElseContainer = ec.IndexContainer;
            }

            this.IfStatement.Indezieren(request);
            if (this.IfStatement is Container c) this.IfContainer = c.IndexContainer;

            this.Condition.Indezieren(request);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            CompileContainer ifcontainer = new CompileContainer();
            ifcontainer.Begin = request.Compiler.ContainerMgmt.CurrentContainer.Begin;
            ifcontainer.Ende = new CompileSprungPunkt();

            CompileContainer elsecontainer = new CompileContainer();
            elsecontainer.Begin = request.Compiler.ContainerMgmt.CurrentContainer.Begin;
            elsecontainer.Ende = new CompileSprungPunkt();

            CompileJumpTo jumpafterelse = new CompileJumpTo();

            CompileJumpWithCondition jumpWithCondition = new CompileJumpWithCondition();

            this.Condition.Compile(request);

            jumpWithCondition.Compile(request.Compiler, ifcontainer.Ende, "isZero");

            request.Compiler.PushContainer(ifcontainer, this.IfContainer.ThisUses);

            this.IfStatement.Compile(request);

            request.Compiler.PopContainer();

            if (this.ElseStatement != null) jumpafterelse.Compile(request.Compiler, elsecontainer.Ende, request.Mode);

            ifcontainer.Ende.Compile(request.Compiler, this, request.Mode);

            if (this.ElseStatement == null) return true;

            request.Compiler.PushContainer(elsecontainer, this.ElseContainer.ThisUses);

            this.ElseStatement.Compile(request);

            request.Compiler.PopContainer();

            elsecontainer.Ende.Compile(request.Compiler, this, request.Mode);

            return true;
        }

        #endregion methods

    }
}