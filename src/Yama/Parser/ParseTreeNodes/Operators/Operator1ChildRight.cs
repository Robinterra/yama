using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Operator1ChildRight : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference Reference
        {
            get;
            set;
        }

        public IParseTreeNode ChildNode
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

                result.Add ( this.ChildNode );

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
        public IndexVariabelnReference VariabelReference { get; private set; }

        #endregion get/set

        #region ctor

        public Operator1ChildRight ( int prio )
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

        private bool CheckHashValidChild ( IdentifierToken token )
        {
            if (token == null) return false;

            foreach ( IdentifierKind op in this.ValidChilds )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }

        private bool CheckAusnahmen ( IdentifierToken token )
        {
            if (token == null) return false;

            foreach ( IdentifierKind op in this.Ausnahmen )
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

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Operator ) return null;
            if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            IdentifierToken lexerLeft = request.Parser.Peek ( request.Token, -1 );

            if ( this.CheckHashValidChild ( lexerLeft ) && !this.CheckAusnahmen ( lexerLeft ) ) return null;

            IdentifierToken lexerRight = request.Parser.Peek ( request.Token, 1 );

            if ( !this.CheckHashValidChild ( lexerRight ) ) return null;

            Operator1ChildRight node = new Operator1ChildRight ( this.Prio );
            node.Token = request.Token;

            node.ChildNode = request.Parser.ParseCleanToken ( lexerRight );

            if ( node.ChildNode == null ) return null;

            node.Token.Node = node;
            node.ChildNode.Token.ParentNode = node;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            reference.IsOperator = true;
            this.ChildNode.Indezieren(request);
            IndexVariabelnReference varref = container.VariabelnReferences.LastOrDefault();

            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.Reference.Deklaration.Use is MethodeDeclarationNode t)
            {
                bool isok = this.CompileCopy(request.Compiler, request.Mode, t);

                if (isok) return true;
            }

            this.ChildNode.Compile(request);

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

            this.ChildNode.Compile(new Request.RequestParserTreeCompile(compiler, mode));

            t.Statement.Compile(new Request.RequestParserTreeCompile(compiler, "default"));

            return true;
        }

        #endregion methods
    }
}