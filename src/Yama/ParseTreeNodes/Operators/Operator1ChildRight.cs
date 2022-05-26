using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Operator1ChildRight : IParseTreeNode, IIndexNode, ICompileNode, IPriority, IContainer, IParentNode
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

        public int Prio
        {
            get;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.ChildNode is not null) result.Add ( this.ChildNode );

                return result;
            }
        }

        public List<string> ValidOperators
        {
            get;
        }

        public List<IdentifierKind> ValidChilds
        {
            get;
        }

        public List<IdentifierKind> Ausnahmen
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

        public IdentifierToken Ende
        {
            get
            {
                if (this.ChildNode is IContainer con) return con.Ende;

                return this.Token;
            }
        }

        public IParseTreeNode? LeftNode
        {
            get
            {
                return this.ChildNode;
            }
            set
            {
                this.ChildNode = value;
            }
        }

        #endregion get/set

        #region ctor

        public Operator1ChildRight ()
        {
            this.ValidOperators = new();
            this.ValidChilds = new();
            this.Token = new();
            this.Ausnahmen = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        public Operator1ChildRight ( int prio ) : this ()
        {
            this.Prio = prio;
        }

        public Operator1ChildRight ( List<string> validOperators, int prio, List<IdentifierKind> validChilds, List<IdentifierKind> ausnahmen )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
            this.ValidChilds = validChilds;
            this.Ausnahmen = ausnahmen;
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

        private bool CheckAusnahmen ( IdentifierToken? token )
        {
            if (token is null) return false;

            foreach ( IdentifierKind op in this.Ausnahmen )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }
        private bool CheckHashValidOperator ( IdentifierToken? token )
        {
            if ( token is null ) return false;

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

            Operator1ChildRight node = new Operator1ChildRight ( this.Prio );
            node.Token = request.Token;
            node.AllTokens.Add ( request.Token );

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.ChildNode is not IIndexNode childnode) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, this.Token.Text);
            reference.IsOperator = true;

            childnode.Indezieren(request);

            IndexVariabelnReference? varref = container.VariabelnReferences.LastOrDefault();
            if (varref is null) return request.Index.CreateError(this);

            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Reference is null) return false;
            if (this.ChildNode is not ICompileNode compileNode) return false;
            if (this.Reference.Deklaration is null) return false;

            if (this.Reference.Deklaration.Use is MethodeDeclarationNode t)
            {
                bool isok = this.CompileCopy(request.Compiler, request.Mode, t);

                if (isok) return true;
            }

            compileNode.Compile(request);

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(request.Compiler, null, "default");

            this.OperatorCall.Compile(request.Compiler, this.Reference, "methode");

            this.FunctionExecute.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        private bool CompileCopy(Compiler.Compiler compiler, string mode, MethodeDeclarationNode t)
        {
            if (t.AccessDefinition == null) return false;
            if (t.AccessDefinition.Kind != IdentifierKind.Copy) return false;
            if (this.ChildNode is not ICompileNode childNode) return false;
            if (t.Statement is not ICompileNode statement) return false;

            childNode.Compile(new RequestParserTreeCompile(compiler, mode));

            statement.Compile(new RequestParserTreeCompile(compiler, "default"));

            return true;
        }

        #endregion methods
    }
}