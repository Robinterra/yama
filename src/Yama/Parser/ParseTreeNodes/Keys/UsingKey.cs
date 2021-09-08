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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public UsingKey()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Using ) return null;
            if ( request.Parser.Peek ( request.Token, 1 ).Kind != IdentifierKind.Text ) return null;

            UsingKey key = new UsingKey (  );
            key.AllTokens.Add ( request.Token );

            IdentifierToken keyNamenToken = request.Parser.Peek ( request.Token, 1 );

            key.Token = keyNamenToken;
            key.AllTokens.Add ( keyNamenToken );

            return key;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexNamespaceDeklaration dek)) return request.Index.CreateError(this, "Kein Namespace als Parent dieses Usings");

            IndexNamespaceReference deklaration = new IndexNamespaceReference();
            deklaration.Name = this.Token.Value.ToString();
            deklaration.Use = this;

            dek.Usings.Add(deklaration);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            return true;
        }

        #endregion methods

    }
}