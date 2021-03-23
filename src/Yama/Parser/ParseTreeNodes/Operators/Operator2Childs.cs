using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Operator2Childs : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference VariabelReference
        {
            get;
            set;
        }

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

        public IParseTreeNode RightNode
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
                result.Add ( this.RightNode );

                return result;
            }
        }

        public IdentifierKind ValidKind
        {
            get;
        }
        public List<string> ValidOperators
        {
            get;
        }
        public IndexVariabelnReference Reference { get; private set; }

        #endregion get/set

        #region ctor

        public Operator2Childs ( int prio )
        {
            this.Prio = prio;
        }

        public Operator2Childs ( List<string> validOperators, int prio )
            : this ( prio )
        {
            this.ValidKind = IdentifierKind.Operator;
            this.ValidOperators = validOperators;
        }

        public Operator2Childs ( IdentifierKind kind, int prio )
            : this ( prio )
        {
            this.ValidKind = kind;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidOperator ( IdentifierToken token )
        {
            if (this.ValidKind != IdentifierKind.Operator) return true;

            foreach ( string op in this.ValidOperators )
            {
                if ( op == token.Text ) return true;
            }

            return false;
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != this.ValidKind ) return null;
            if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            Operator2Childs node = new Operator2Childs ( this.Prio );
            node.Token = request.Token;
            node.Token.Node = node;

            node.LeftNode = request.Parser.ParseCleanToken ( request.Parser.Peek ( request.Token, -1 ) );

            node.RightNode = request.Parser.ParseCleanToken ( request.Parser.Peek ( request.Token, 1 ) );

            node.LeftNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            reference.IsOperator = true;
            this.LeftNode.Indezieren(request);
            IndexVariabelnReference varref = container.VariabelnReferences.LastOrDefault();


            this.RightNode.Indezieren(request);
            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            if (this.Token.Text == "=") return true;
            if (varref == null) return request.Index.CreateError(this);

            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            this.RightNode.Compile(request);

            if (this.Token.Text == "=")
            {
                this.LeftNode.Compile(new Request.RequestParserTreeCompile ( request.Compiler, "set" ));

                return true;
            }

            if (this.Reference.Deklaration.Use is MethodeDeclarationNode t)
            {
                bool isok = this.CompileCopy(request.Compiler, request.Mode, t);

                if (isok) return true;
            }

            this.FunctionsCall(request.Compiler, request.Mode);

            return true;
        }

        private bool CompileCopy(Compiler.Compiler compiler, string mode, MethodeDeclarationNode t)
        {
            if (t.AccessDefinition == null) return false;
            if (t.AccessDefinition.Kind != IdentifierKind.Copy) return false;

            this.LeftNode.Compile(new Request.RequestParserTreeCompile (compiler, mode));

            t.Statement.Compile(new Request.RequestParserTreeCompile(compiler, "default"));

            return true;
        }

        private bool FunctionsCall(Compiler.Compiler compiler, string mode)
        {
            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            this.LeftNode.Compile(new Request.RequestParserTreeCompile(compiler, mode));

            compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            this.OperatorCall.Compile(compiler, this.Reference, "methode");

            this.FunctionExecute.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods
    }
}