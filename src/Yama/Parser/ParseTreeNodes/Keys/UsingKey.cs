using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class UsingKey : IParseTreeNode
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                return result;
            }
        }

        public ParserLayer NextLayer { get; }

        #endregion get/set

        #region ctor

        public UsingKey()
        {
            
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.Using ) return null;
            if ( parser.Peek ( token, 1 ).Kind != IdentifierKind.Text ) return null;

            UsingKey key = new UsingKey (  );
            token.Node = key;

            IdentifierToken keyNamenToken = parser.Peek ( token, 1 );

            key.Token = keyNamenToken;
            keyNamenToken.Node = key;

            return key;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexNamespaceDeklaration dek)) return index.CreateError(this, "Kein Namespace als Parent dieses Usings");

            IndexNamespaceReference deklaration = new IndexNamespaceReference();
            deklaration.Name = this.Token.Value.ToString();
            deklaration.Use = this;

            dek.Usings.Add(deklaration);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            return true;
        }

        #endregion methods

    }
}