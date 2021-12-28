using System.Collections.Generic;
using System.Linq;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Operator3Childs : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference? Reference
        {
            get;
            set;
        }

        public IParseTreeNode? LeftNode
        {
            get;
            set;
        }

        public IParseTreeNode? MiddleNode
        {
            get;
            set;
        }

        public IParseTreeNode? RightNode
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

                if (this.LeftNode != null) result.Add ( this.LeftNode );
                if (this.MiddleNode != null) result.Add ( this.MiddleNode );
                if (this.RightNode != null) result.Add ( this.RightNode );

                return result;
            }
        }

        public List<string> ValidOperators
        {
            get;
        }

        public IdentifierKind Steuerzeichen
        {
            get;
            set;
        }

        public IndexVariabelnReference? VariabelReference
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

        public Operator3Childs ()
        {
            this.ValidOperators = new();
            this.Token = new IdentifierToken();
            this.AllTokens = new List<IdentifierToken> ();
        }

        public Operator3Childs ( int prio ) : this()
        {
            this.Prio = prio;
        }

        public Operator3Childs ( List<string> validOperators, IdentifierKind steuerzeichen, int prio )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
            this.Steuerzeichen = steuerzeichen;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidOperator ( IdentifierToken token )
        {
            if ( token == null ) return false;
            
            foreach ( string op in this.ValidOperators )
            {
                if ( op == token.Text ) return true;
            }

            return false;
        }

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Operator ) return null;
            if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            Operator3Childs node = new Operator3Childs ( this.Prio );
            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            IdentifierToken? steuerToken = request.Parser.FindAToken ( request.Token, this.Steuerzeichen );
            if ( steuerToken is null ) return null;

            node.AllTokens.Add(steuerToken);

            IdentifierToken? token = request.Parser.Peek ( request.Token, -1 );
            if (token is null) return null;

            node.LeftNode = request.Parser.ParseCleanToken ( token );

            token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return null;

            node.MiddleNode = request.Parser.ParseCleanToken ( token );

            token = request.Parser.Peek ( steuerToken, 1 );
            if (token is null) return null;

            node.RightNode = request.Parser.ParseCleanToken ( token );

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);
            if (this.LeftNode is null) return request.Index.CreateError(this);
            if (this.MiddleNode is null) return request.Index.CreateError(this);
            if (this.RightNode is null) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, this.Token.Text);
            this.LeftNode.Indezieren(request);
            IndexVariabelnReference varref = container.VariabelnReferences.Last();

            this.MiddleNode.Indezieren(request);
            this.RightNode.Indezieren(request);
            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            return true;
        }

        #endregion methods
    }
}