using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class IfKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {

        #region get/set

        public IParseTreeNode? Condition
        {
            get;
            set;
        }

        public IParseTreeNode? IfStatement
        {
            get;
            set;
        }

        public IParseTreeNode? ElseStatement
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
                if (this.IfStatement is null) return this.Token;

                if ( this.ElseStatement == null ) return (this.IfStatement is IContainer t) ? t.Ende : this.IfStatement.Token;

                return (this.ElseStatement is IContainer a) ? a.Ende : this.ElseStatement.Token;
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

        public IndexContainer? IfContainer
        {
            get;
            set;
        }

        public IndexContainer? ElseContainer
        {
            get;
            set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        private ParserLayer expressionLayer;

        private ParserLayer statementLayer;

        #endregion get/set

        #region ctor

        public IfKey (ParserLayer expressionLayer, ParserLayer statementLayer)
        {
            this.Token = new();
            this.statementLayer = statementLayer;
            this.AllTokens = new List<IdentifierToken> ();
            this.expressionLayer = expressionLayer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.If ) return null;

            IdentifierToken? openBrackettoken = request.Parser.Peek ( request.Token, 1 );
            if (openBrackettoken is null) return new ParserError(request.Token, $"Expectet a open Bracket '(' after a 'if' Keyword {request.Token.Kind}");
            if ( openBrackettoken.Kind != IdentifierKind.OpenBracket ) return new ParserError(openBrackettoken, $"Expectet a open Bracket '(' after Keyword 'if' and not a {openBrackettoken.Kind}", request.Token);

            IfKey key = new IfKey ( this.expressionLayer, this.statementLayer );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);
            key.AllTokens.Add(openBrackettoken);

            IdentifierToken? conditionToken = request.Parser.Peek ( openBrackettoken, 1 );
            if (conditionToken is null) return new ParserError(request.Token, $"Expectet a begin of a Condition after '('", openBrackettoken);

            IParseTreeNode? condition = request.Parser.ParseCleanToken(conditionToken, this.expressionLayer, false);
            if (condition is not IContainer conCon) return new ParserError(request.Token, $"Can not parse Condition of a if", openBrackettoken);
            key.Condition = condition;

            IdentifierToken? closeBracket = request.Parser.Peek ( conCon.Ende, 1);
            if (closeBracket is null) return null;
            if (closeBracket.Kind != IdentifierKind.CloseBracket) return new ParserError(closeBracket, $"Expected ) and not", request.Token, openBrackettoken);
            key.AllTokens.Add(closeBracket);

            IdentifierToken? ifStatementchild = request.Parser.Peek (closeBracket, 1);
            if (ifStatementchild is null) return new ParserError(request.Token, $"Can not find a Statement after a if", openBrackettoken, conditionToken);
            //if (!this.IsAllowedStatmentToken (ifStatementchild)) return new ParserError(ifStatementchild, $"A if Statement can not begin with a '{ifStatementchild.Kind}'. Possilbe begins of a if Statement is: return, break, continue, '{{', if, while, for.", openBrackettoken, conditionToken, request.Token);

            IParseTreeNode? statementNode = request.Parser.ParseCleanToken(ifStatementchild, this.statementLayer, false);
            if (statementNode is null) return new ParserError(ifStatementchild, $"A if Statement can not begin with a '{ifStatementchild.Kind}'. Possilbe begins of a if Statement is: return, break, continue, '{{', if, while, for.", openBrackettoken, conditionToken, request.Token);

            key.IfStatement = statementNode;

            IdentifierToken elsePeekToken = (key.IfStatement is IContainer t) ? t.Ende : key.IfStatement.Token;

            IdentifierToken? elseStatementChild = request.Parser.Peek ( elsePeekToken, 1 );
            if ( elseStatementChild is null ) return key;
            if ( elseStatementChild.Node is not null ) return key;
            if ( elseStatementChild.Kind != IdentifierKind.Else ) return key;

            ElseKey elseRule = request.Parser.GetRule<ElseKey>();

            ElseKey? elseNode = request.Parser.TryToParse(elseRule, elseStatementChild);
            if (elseNode is null) return new ParserError(elseStatementChild, $"else statement can not be parse", openBrackettoken, conditionToken, request.Token);

            key.ElseStatement = elseNode;

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

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            this.IfContainer = container;
            this.ElseContainer = container;

            if (this.ElseStatement is IIndexNode elseNode)
            {
                elseNode.Indezieren(request);
                if (this.ElseStatement is Container ec) this.ElseContainer = ec.IndexContainer;
            }

            if (this.IfStatement is not IIndexNode statementNode) return request.Index.CreateError(this);

            statementNode.Indezieren(request);
            if (this.IfStatement is Container c) this.IfContainer = c.IndexContainer;

            if (this.Condition is not IIndexNode conditionNode) return request.Index.CreateError(this);
            conditionNode.Indezieren(request);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Condition is not ICompileNode conditionNode) return false;
            if (this.IfContainer is null) return false;
            if (this.IfStatement is not ICompileNode statementNode) return false;
            if (request.Compiler.ContainerMgmt.CurrentContainer is null) return false;

            CompilePhi compilePhi = new CompilePhi();

            CompileContainer ifcontainer = new CompileContainer();
            ifcontainer.Begin = request.Compiler.ContainerMgmt.CurrentContainer.Begin;
            ifcontainer.Ende = new CompileSprungPunkt();
            ifcontainer.Ende.Node = this;

            CompileContainer elsecontainer = new CompileContainer();
            elsecontainer.Begin = request.Compiler.ContainerMgmt.CurrentContainer.Begin;
            elsecontainer.Ende = new CompileSprungPunkt();
            elsecontainer.Ende.Node = this.ElseStatement;

            CompileJumpTo jumpafterelse = new CompileJumpTo();

            CompileJumpWithCondition jumpWithCondition = new CompileJumpWithCondition();

            request.Compiler.GetCopyOfCurrentContext();

            conditionNode.Compile(request);

            jumpWithCondition.Compile(request.Compiler, ifcontainer.Ende, "isZero");

            request.Compiler.PushContainer(ifcontainer, this.IfContainer.ThisUses);

            statementNode.Compile(request);

            IEnumerable<KeyValuePair<string, SSAVariableMap>> variableMaps = request.Compiler.PopContainerAndReturnVariableMapperForIfs();

            if (this.ElseStatement is not null) jumpafterelse.Compile(request.Compiler, elsecontainer.Ende, request.Mode);

            ifcontainer.Ende.Compile(request.Compiler, this, request.Mode);

            if (this.ElseStatement is not ICompileNode elseStatementNode) return compilePhi.Compile(request.Compiler, variableMaps, this);
            if (this.ElseContainer is null) return true;

            request.Compiler.PushContainer(elsecontainer, this.ElseContainer.ThisUses);

            elseStatementNode.Compile(request);

            IEnumerable<KeyValuePair<string, SSAVariableMap>> elseVariableMaps = request.Compiler.PopContainerAndReturnVariableMapperForIfs();

            elsecontainer.Ende.Compile(request.Compiler, this, request.Mode);

            compilePhi.Compile(request.Compiler, variableMaps, this);

            compilePhi = new CompilePhi();
            compilePhi.Compile(request.Compiler, elseVariableMaps, this.ElseStatement);

            return true;
        }

        #endregion methods

    }
}