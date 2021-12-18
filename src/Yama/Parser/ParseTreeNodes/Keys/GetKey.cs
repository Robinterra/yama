using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class GetKey : IParseTreeNode, IContainer
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
            set
            {

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
            if (conditionkind is null) return null;

            IParseTreeNode? statement = request.Parser.ParseCleanToken ( conditionkind, this.layer );
            if ( statement is not Container container ) return null;

            key.Statement = container;

            return key;
        }

        private bool IndezierenVektor(Index.Index index, IndexVektorDeklaration parent)
        {
            if (this.Statement is null) return false;

            IndexContainer container = new IndexContainer();

            parent.GetContainer = container;

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(index, container));

            return true;
        }

        private bool IndezierenPropertyGetSet(Index.Index index, IndexPropertyGetSetDeklaration parent)
        {
            if (this.Statement is null) return false;

            IndexContainer container = new IndexContainer();

            parent.GetContainer = container;

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(index, container));

            return true;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is IndexVektorDeklaration vekdek) return this.IndezierenVektor(request.Index, vekdek);
            if (request.Parent is IndexPropertyGetSetDeklaration popdek) return this.IndezierenPropertyGetSet(request.Index, popdek);
            if (!(request.Parent is IndexPropertyDeklaration propertyDeklaration)) return request.Index.CreateError(this);
            if (this.Statement == null) return request.Index.CreateError(this);

            IndexContainer container = new IndexContainer();

            propertyDeklaration.GetContainer = container;

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, container));

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.Statement is null) return false;

            return this.Statement.Compile(request);
        }

        #endregion methods

    }
}