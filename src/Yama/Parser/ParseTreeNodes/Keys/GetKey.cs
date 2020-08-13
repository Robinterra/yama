using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class GetKey : IParseTreeNode, IContainer
    {
        private ParserLayer layer;

        #region get/set

        public IParseTreeNode Statement
        {
            get;
            set;
        }

        public SyntaxToken Token
        {
            get;
            set;
        }

        public SyntaxToken Ende
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

        #endregion get/set

        #region ctor

        public GetKey()
        {

        }

        public GetKey(ParserLayer layer)
        {
            this.layer = layer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Get ) return null;

            GetKey key = new GetKey (  );
            key.Token = token;
            token.Node = key;

            SyntaxToken conditionkind = parser.Peek ( token, 1 );

            key.Statement = parser.ParseCleanToken(conditionkind, this.layer);

            if (key.Statement == null) return null;

            key.Statement.Token.ParentNode = key;

            return key;
        }

        private bool IndezierenVektor(Index.Index index, IndexVaktorDeklaration parent)
        {
            IndexContainer container = new IndexContainer();

            parent.GetContainer = container;

            this.Statement.Indezieren(index, container);

            return true;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (parent is IndexVaktorDeklaration vekdek) return this.IndezierenVektor(index, vekdek);
            if (!(parent is IndexPropertyDeklaration propertyDeklaration)) return index.CreateError(this);
            if (this.Statement == null) return index.CreateError(this);

            IndexContainer container = new IndexContainer();

            propertyDeklaration.GetContainer = container;

            this.Statement.Indezieren(index, container);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.Statement.Compile(compiler, mode);

            return true;
        }

        #endregion methods

    }
}