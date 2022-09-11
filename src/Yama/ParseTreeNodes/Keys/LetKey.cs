using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class LetKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IParseTreeNode? Statement
        {
            get;
            set;
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        private ParserLayer expressionLayer;

        #endregion get/set

        #region ctor

        public LetKey (ParserLayer expressionLayer)
        {
            this.Token = new();
            this.Ende = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.expressionLayer = expressionLayer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Let ) return null;

            LetKey key = new LetKey ( this.expressionLayer );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken? dekToken = request.Parser.Peek ( request.Token, 1 );
            if (dekToken is null) return null;

            VariabelDeklaration dek = request.Parser.GetRule<VariabelDeklaration>();

            VariabelDeklaration? deklaration = request.Parser.TryToParse(dek, dekToken);
            if (deklaration is not VariabelDeklaration varDek) return null;
            key.Statement = deklaration;

            IdentifierToken? semikolon = request.Parser.Peek(varDek.Ende, 1);
            if (semikolon is null) return null;
            if (semikolon.Kind != IdentifierKind.EndOfCommand) return null;

            key.Ende = semikolon;
            key.AllTokens.Add(semikolon);

            return key;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            if (this.Statement is IIndexNode node) node.Indezieren(request);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Statement is not ICompileNode statementNode) return false;

            return statementNode.Compile(new RequestParserTreeCompile(request.Compiler, request.Mode, true));
        }

        #endregion methods

    }
}