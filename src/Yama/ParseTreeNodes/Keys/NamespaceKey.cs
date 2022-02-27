using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NamespaceKey : IParseTreeNode
    {

        #region get/set

        public IParseTreeNode? Statement
        {
            get;
            set;
        }

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

                if (this.Statement is not null) result.Add ( this.Statement );

                return result;
            }
        }

        public ParserLayer NextLayer
        {
            get;
        }

        public IndexNamespaceDeklaration? Deklaration
        {
            get;
            set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public NamespaceKey(ParserLayer nextLayer)
        {
            this.NextLayer = nextLayer;
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Namespace ) return null;

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if ( token is null ) return null;
            if ( token.Kind != IdentifierKind.Text ) return null;

            NamespaceKey key = new NamespaceKey ( this.NextLayer );
            key.AllTokens.Add ( request.Token );

            IdentifierToken? keyNamenToken = request.Parser.Peek ( request.Token, 1 );

            if ( keyNamenToken is null ) return null;
            key.Token = keyNamenToken;
            key.AllTokens.Add ( keyNamenToken );

            IdentifierToken? statementchild = request.Parser.Peek ( keyNamenToken, 1);
            if ( statementchild is null ) return null;

            key.Statement = request.Parser.ParseCleanToken(statementchild, this.NextLayer);

            if (key.Statement == null) return null;

            return key;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if ( this.Token.Value is null ) return false;

            string? name = this.Token.Value.ToString ();
            if ( name is null ) return false;

            IndexNamespaceDeklaration dek = new IndexNamespaceDeklaration(this, name);

            dek = request.Index.NamespaceAdd(dek);
            this.Deklaration = dek;

            if (this.Token.Info is not null) dek.OriginKeys.Add(this.Token.Info.Origin);

            if ( this.Statement is null ) return false;
            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, dek));

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            //this.Statement.Compile(compiler, mode);

            return true;
        }

        #endregion methods

    }
}