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

        public ParserLayer NextLayer { get; }
        public IndexNamespaceDeklaration Deklaration { get; private set; }

        #endregion get/set

        #region ctor

        public NamespaceKey()
        {
            
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
            request.Token.Node = key;

            IdentifierToken keyNamenToken = request.Parser.Peek ( request.Token, 1 );

            key.Token = keyNamenToken;
            keyNamenToken.Node = key;

            IdentifierToken Statementchild = request.Parser.Peek ( keyNamenToken, 1);

            key.Statement = request.Parser.ParseCleanToken(Statementchild, this.NextLayer);

            if (key.Statement == null) return null;

            key.Statement.Token.ParentNode = key;

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