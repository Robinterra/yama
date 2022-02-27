using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class Operator2Childs : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference? VariabelReference
        {
            get;
            set;
        }

        public IParseTreeNode? LeftNode
        {
            get;
            set;
        }

        public IParseTreeNode? RightNode
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

                if (this.LeftNode != null) result.Add ( this.LeftNode );
                if (this.RightNode != null) result.Add ( this.RightNode );

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

        public IndexVariabelnReference? Reference
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

        public Operator2Childs ()
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.Token = new();
            this.ValidOperators = new();
        }

        public Operator2Childs ( int prio ) : this()
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

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != this.ValidKind ) return null;
            if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            Operator2Childs node = new Operator2Childs ( this.Prio );
            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            IdentifierToken? token = request.Parser.Peek ( request.Token, -1 );
            if (token is null) return null;

            node.LeftNode = request.Parser.ParseCleanToken ( token );

            token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return null;

            node.RightNode = request.Parser.ParseCleanToken ( token );

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.LeftNode is null) return request.Index.CreateError(this);
            if (this.RightNode is null) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, this.Token.Text);
            reference.IsOperator = true;
            int anzahl = container.VariabelnReferences.Count;

            this.LeftNode.Indezieren(request);
            IndexVariabelnReference? varref = container.VariabelnReferences.LastOrDefault();
            if (anzahl == container.VariabelnReferences.Count) varref = null;

            this.RightNode.Indezieren(request);
            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            if (this.Token.Text == "=") return this.SetIndex(request, varref, container.VariabelnReferences.LastOrDefault());
            if (varref == null) return request.Index.CreateError(this);

            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            reference.ChildUse = varref;
            this.Reference = reference;
            container.VariabelnReferences.Add(reference);

            return true;
        }

        private bool SetIndex(RequestParserTreeIndezieren request, IndexVariabelnReference? varref, IndexVariabelnReference? indexVariabelnReference)
        {
            if (varref is null) return false;
            if (indexVariabelnReference is null) return false;

            request.Index.IndexTypeSafeties.Add(new IndexSetValue(varref, indexVariabelnReference));

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.RightNode is null) return false;
            if (this.LeftNode is null) return false;

            this.RightNode.Compile(request);

            if (this.Token.Text == "=")
            {
                this.LeftNode.Compile(new Request.RequestParserTreeCompile ( request.Compiler, "set" ));

                return true;
            }

            if (this.Reference is null) return false;
            if (this.Reference.Deklaration is null) return false;
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
            if (this.LeftNode is null) return false;
            if (t.AccessDefinition == null) return false;
            if (t.AccessDefinition.Kind != IdentifierKind.Copy) return false;
            if (t.Statement is null) return false;

            this.LeftNode.Compile(new Request.RequestParserTreeCompile (compiler, mode));

            t.Statement.Compile(new Request.RequestParserTreeCompile(compiler, "default"));

            return true;
        }

        private bool FunctionsCall(Compiler.Compiler compiler, string mode)
        {
            if (this.LeftNode is null) return false;
            if (this.Reference is null) return false;

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