using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class SetKey : IParseTreeNode, IContainer
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

        public SetKey(ParserLayer layer)
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.layer = layer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Set ) return null;

            SetKey key = new SetKey ( this.layer );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken? conditionkind = request.Parser.Peek ( request.Token, 1 );
            if (conditionkind is null) return new ParserError(request.Token, "Expectet a open Bracket '{' after Keyword 'set'");

            IParseTreeNode? node = request.Parser.ParseCleanToken(conditionkind, this.layer);
            if (node is not Container c) return new ParserError(request.Token, "failed to parse 'set' statement");

            key.Statement = c;

            return key;
        }

        private bool IndezierenVektor(Index.Index index, IndexVektorDeklaration parent)
        {
            if (this.Statement is null) return index.CreateError(this);

            IndexContainer container = new IndexContainer(this, "vektor container");

            parent.SetContainer = container;

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(index, container));

            return true;
        }

        private bool IndezierenPropertyGetSet(Index.Index index, IndexPropertyGetSetDeklaration parent)
        {
            if (this.Statement is null) return index.CreateError(this);

            IndexContainer container = new IndexContainer(this, "indez get set property");

            parent.SetContainer = container;

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(index, container));

            return true;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is IndexVektorDeklaration vekdek) return this.IndezierenVektor(request.Index, vekdek);
            if (request.Parent is IndexPropertyGetSetDeklaration popdek) return this.IndezierenPropertyGetSet(request.Index, popdek);
            if (!(request.Parent is IndexPropertyDeklaration propertyDeklaration)) return request.Index.CreateError(this);
            if (this.Statement == null) return request.Index.CreateError(this);

            IndexContainer container = new IndexContainer(this, "set container");

            propertyDeklaration.SetContainer = container;

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, container));

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.Statement is null) return false;

            this.Statement.Compile(request);

            return true;
        }

        #endregion methods

    }
}