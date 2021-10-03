using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NamespaceKey : IParseTreeNode
    {

        #region get/set

        public IParseTreeNode Statement
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

                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        public ParserLayer NextLayer
        {
            get;
        }

        public IndexNamespaceDeklaration Deklaration
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

        public NamespaceKey()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        public NamespaceKey(ParserLayer nextLayer)
        {
            this.NextLayer = nextLayer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Namespace ) return null;
            if ( request.Parser.Peek ( request.Token, 1 ).Kind != IdentifierKind.Text ) return null;

            NamespaceKey key = new NamespaceKey (  );
            key.AllTokens.Add ( request.Token );

            IdentifierToken keyNamenToken = request.Parser.Peek ( request.Token, 1 );

            key.Token = keyNamenToken;
            key.AllTokens.Add ( keyNamenToken );

            IdentifierToken Statementchild = request.Parser.Peek ( keyNamenToken, 1);

            key.Statement = request.Parser.ParseCleanToken(Statementchild, this.NextLayer);

            if (key.Statement == null) return null;

            return key;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            IndexNamespaceDeklaration dek = new IndexNamespaceDeklaration();
            dek.Name = this.Token.Value.ToString();

            dek = request.Index.NamespaceAdd(dek);
            this.Deklaration = dek;

            dek.Files.Add(this.Token.FileInfo);

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