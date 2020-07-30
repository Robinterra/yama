using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;

namespace Yama.Parser
{
    public class ExplicitConverting : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexMethodReference Reference
        {
            get;
            set;
        }

        public IndexVariabelnReference BooleascherReturn
        {
            get;
            set;
        }

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

        public SyntaxToken RightToken
        {
            get;
            set;
        }

        public SyntaxToken Token
        {
            get;
            set;
        }

        public int Prio
        {
            get;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                result.Add ( this.LeftNode );

                return result;
            }
        }

        public SyntaxKind ValidKind
        {
            get;
        }
        public List<string> ValidOperators
        {
            get;
        }
        public SyntaxToken ReferenceDeklaration
        {
            get;
            set;
        }
        public IndexVariabelnDeklaration Deklaration { get; private set; }

        #endregion get/set

        #region ctor

        public ExplicitConverting ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidTypeDefinition ( SyntaxToken token )
        {
            if (token == null) return false;

            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Char) return true;
            if (token.Kind == SyntaxKind.Byte) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;
            if (token.Kind == SyntaxKind.Float32Bit) return true;

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Is ) return null;

            ExplicitConverting node = new ExplicitConverting ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.LeftNode = parser.ParseCleanToken ( parser.Peek ( token, -1 ) );

            node.RightToken = parser.Peek ( token, 1 );
            if ( !this.CheckHashValidTypeDefinition ( node.RightToken ) ) return null;

            node.ReferenceDeklaration = parser.Peek ( node.RightToken, 1 );
            if (node.ReferenceDeklaration.Kind != SyntaxKind.Word) return null;

            node.LeftNode.Token.ParentNode = node;
            node.RightToken.Node = node;
            node.ReferenceDeklaration.Node = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            IndexVariabelnDeklaration reference = new IndexVariabelnDeklaration();
            reference.Use = this;
            reference.Name = this.ReferenceDeklaration.Text;
            container.VariabelnDeklarations.Add(reference);
            IndexVariabelnReference type = new IndexVariabelnReference { Name = this.RightToken.Text, Use = this };
            reference.Type = type;

            container.VariabelnReferences.Add(type);
            this.BooleascherReturn = new IndexVariabelnReference { Name = "Bool", Use = this };
            container.VariabelnReferences.Add(this.BooleascherReturn);

            this.Deklaration = reference;

            return true;
        }

        #endregion methods
    }
}