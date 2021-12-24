using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using Yama.Compiler;

namespace Yama.Parser
{
    public class Operator1ChildLeft : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference? Reference
        {
            get;
            set;
        }

        public IParseTreeNode? ChildNode
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

                if (this.ChildNode != null) result.Add ( this.ChildNode );

                return result;
            }
        }

        public List<string> ValidOperators
        {
            get;
        }

        public CompileReferenceCall OperatorCall
        {
            get;
            set;
        } = new CompileReferenceCall();

        public CompileExecuteCall FunctionExecute
        {
            get;
            set;
        } = new CompileExecuteCall();

        public List<IdentifierKind> ValidChilds
        {
            get;
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

        public Operator1ChildLeft ()
        {
            this.Token = new();
            this.ValidOperators = new List<string>();
            this.AllTokens = new List<IdentifierToken> ();
            this.ValidChilds = new List<IdentifierKind>();
        }

        public Operator1ChildLeft ( int prio ) : this ()
        {
            this.Prio = prio;
        }

        public Operator1ChildLeft ( List<string> validOperators, int prio, List<IdentifierKind> validChilds )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
            this.ValidChilds = validChilds;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidChild ( IdentifierToken? token )
        {
            if (token is null) return false;

            foreach ( IdentifierKind op in this.ValidChilds )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }

        private bool CheckHashValidOperator ( IdentifierToken token )
        {
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

            IdentifierToken? lexerRight = request.Parser.Peek ( request.Token, 1 );
            if ( this.CheckHashValidChild ( lexerRight ) ) return null;

            IdentifierToken? lexerLeft = request.Parser.Peek ( request.Token, -1 );
            if (lexerLeft is null) return null;
            if ( !this.CheckHashValidChild ( lexerLeft ) ) return null;

            Operator1ChildLeft node = new Operator1ChildLeft ( this.Prio );
            node.Token = request.Token;
            node.AllTokens.Add ( request.Token );

            node.ChildNode = request.Parser.ParseCleanToken ( lexerLeft );

            if ( node.ChildNode == null ) return null;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.ChildNode is null) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            this.ChildNode.Indezieren(request);
            IndexVariabelnReference varref = container.VariabelnReferences.Last();

            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.ChildNode is null) return false;
            if (this.Reference is null) return false;

            this.ChildNode.Compile(request);

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(request.Compiler, null, "default");

            this.OperatorCall.Compile(request.Compiler, this.Reference, "methode");

            this.FunctionExecute.Compile(request.Compiler, null, request.Mode);

            this.ChildNode.Compile(new Request.RequestParserTreeCompile(request.Compiler, "set"));

            return true;
        }

        #endregion methods
    }
}