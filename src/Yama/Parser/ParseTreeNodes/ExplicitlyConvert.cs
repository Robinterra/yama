using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System.Linq;

namespace Yama.Parser
{
    public class ExplicitlyConvert : IParseTreeNode
    {

        #region get/set

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

        public IdentifierToken RightToken
        {
            get;
            set;
        }

        public IdentifierToken Token
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

        public IdentifierKind ValidKind
        {
            get;
        }

        public IndexVariabelnDeklaration Deklaration
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public ExplicitlyConvert ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidTypeDefinition ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;

            return false;
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.As ) return null;

            ExplicitlyConvert node = new ExplicitlyConvert ( this.Prio );
            node.Token = request.Token;

            node.LeftNode = request.Parser.ParseCleanToken ( request.Parser.Peek ( request.Token, -1 ) );

            node.RightToken = request.Parser.Peek ( request.Token, 1 );
            if ( !this.CheckHashValidTypeDefinition ( node.RightToken ) ) return null;

            node.Token.Node = node;
            node.LeftNode.Token.ParentNode = node;
            node.RightToken.Node = node;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            this.LeftNode.Indezieren(request);

            IndexVariabelnReference type = new IndexVariabelnReference { Name = this.RightToken.Text, Use = this };
            container.VariabelnReferences.Add(type);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            return this.LeftNode.Compile(request);
        }

        #endregion methods
    }
}