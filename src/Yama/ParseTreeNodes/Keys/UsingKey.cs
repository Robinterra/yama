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
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Using ) return null;

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return new ParserError(request.Token, "Wrong Syntax for a using. Expected a Text after the using");
            if (token.Kind != IdentifierKind.Text) return new ParserError(request.Token, $"Wrong Syntax for a using. Expected: 'using \"YourUsingNameSpaceName\"' and not 'using {token.Text}'", token);

            UsingKey key = new UsingKey (  );
            key.AllTokens.Add ( request.Token );

            key.Token = token;
            key.AllTokens.Add ( token );

            return key;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexNamespaceDeklaration dek) return request.Index.CreateError(this, "Kein Namespace als Parent dieses Usings");

            string? value = this.Token.Value is null ? null : this.Token.Value.ToString();
            if (value is null) return request.Index.CreateError(this);

            IndexNamespaceReference deklaration = new IndexNamespaceReference(this, value);

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