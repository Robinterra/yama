using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class GetKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {
        private ParserLayer layer;

        #region get/set

        public Container? Statement
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

                return (this.Statement is IContainer a) ? a.Ende : this.Statement.Token;
            }
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

        #endregion get/set

        #region ctor

        public GetKey(ParserLayer layer)
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.layer = layer;
            this.Token = new();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Get ) return null;

            GetKey key = new GetKey ( this.layer );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken? conditionkind = request.Parser.Peek ( request.Token, 1 );
            if (conditionkind is null) return new ParserError(request.Token, "Expectet a open Bracket '{' after Keyword 'get'");

            IParseTreeNode? statement = request.Parser.ParseCleanToken ( conditionkind, this.layer );
            if ( statement is not Container container ) return new ParserError(request.Token, "failed to parse 'get' statement");

            key.Statement = container;

            return key;
        }

        private bool IndezierenVektor(Index.Index index, IndexVektorDeklaration parent)
        {
            if (this.Statement is null) return false;

            IndexContainer container = new IndexContainer(this, "vektor container");

            parent.GetContainer = container;

            this.Statement.Indezieren(new RequestParserTreeIndezieren(index, container));

            return true;
        }

        private bool IndezierenPropertyGetSet(Index.Index index, IndexPropertyGetSetDeklaration parent)
        {
            if (this.Statement is null) return false;

            IndexContainer container = new IndexContainer(this, "prop get set container");

            parent.GetContainer = container;

            this.Statement.Indezieren(new RequestParserTreeIndezieren(index, container));

            return true;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is IndexVektorDeklaration vekdek) return this.IndezierenVektor(request.Index, vekdek);
            if (request.Parent is IndexPropertyGetSetDeklaration popdek) return this.IndezierenPropertyGetSet(request.Index, popdek);
            if (!(request.Parent is IndexPropertyDeklaration propertyDeklaration)) return request.Index.CreateError(this);
            if (this.Statement == null) return request.Index.CreateError(this);

            IndexContainer container = new IndexContainer(this, "get container");

            propertyDeklaration.GetContainer = container;

            this.Statement.Indezieren(new RequestParserTreeIndezieren(request.Index, container));

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Statement is null) return false;

            return this.Statement.Compile(request);
        }

        #endregion methods

    }
}