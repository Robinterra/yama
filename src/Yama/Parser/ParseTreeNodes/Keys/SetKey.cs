using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class SetKey : IParseTreeNode, IContainer
    {
        private ParserLayer layer;

        #region get/set

        public Container Statement
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

        public SetKey()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        public SetKey(ParserLayer layer)
        {
            this.layer = layer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Set ) return null;

            SetKey key = new SetKey (  );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken conditionkind = request.Parser.Peek ( request.Token, 1 );

            if (request.Parser.ParseCleanToken(conditionkind, this.layer) is Container container) key.Statement = container;

            if (key.Statement == null) return null;

            return key;
        }

        private bool IndezierenVektor(Index.Index index, IndexVektorDeklaration parent)
        {
            IndexContainer container = new IndexContainer();

            parent.SetContainer = container;

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(index, container));

            return true;
        }

        private bool IndezierenPropertyGetSet(Index.Index index, IndexPropertyGetSetDeklaration parent)
        {
            IndexContainer container = new IndexContainer();

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

            IndexContainer container = new IndexContainer();

            propertyDeklaration.SetContainer = container;

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, container));

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            this.Statement.Compile(request);

            return true;
        }

        #endregion methods

    }
}