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

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.Namespace ) return null;
            if ( parser.Peek ( token, 1 ).Kind != IdentifierKind.Text ) return null;

            NamespaceKey key = new NamespaceKey (  );
            token.Node = key;

            IdentifierToken keyNamenToken = parser.Peek ( token, 1 );

            key.Token = keyNamenToken;
            keyNamenToken.Node = key;

            IdentifierToken Statementchild = parser.Peek ( keyNamenToken, 1);

            key.Statement = parser.ParseCleanToken(Statementchild, this.NextLayer);

            if (key.Statement == null) return null;

            key.Statement.Token.ParentNode = key;


            return key;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            IndexNamespaceDeklaration dek = new IndexNamespaceDeklaration();
            dek.Name = this.Token.Value.ToString();

            dek = index.NamespaceAdd(dek);
            this.Deklaration = dek;

            dek.Files.Add(this.Token.FileInfo);

            this.Statement.Indezieren(index, dek);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            //this.Statement.Compile(compiler, mode);

            return true;
        }

        #endregion methods

    }
}