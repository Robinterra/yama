using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class Operator2Childs : IParseTreeNode, IIndexNode, ICompileNode, IPriority, IParentNode, IContainer
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

        public IdentifierToken Ende
        {
            get
            {
                if (this.RightNode is IContainer con) return con.Ende;
                if (this.RightNode is null) return this.Token;

                return this.RightNode.Token;
            }
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

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return null;

            node.RightNode = request.Parser.ParseCleanToken ( token );

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.LeftNode is not IIndexNode leftNode) return request.Index.CreateError(this);
            if (this.RightNode is not IIndexNode rightNode) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, this.Token.Text);
            reference.IsOperator = true;
            int anzahl = container.VariabelnReferences.Count;

            leftNode.Indezieren(request);
            IndexVariabelnReference? varref = container.VariabelnReferences.LastOrDefault();
            if (anzahl == container.VariabelnReferences.Count) varref = null;

            rightNode.Indezieren(request);
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

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.RightNode is not ICompileNode rightNode) return false;
            if (this.LeftNode is not ICompileNode leftNode) return false;

            rightNode.Compile(request);

            if (this.Token.Text == "=")
            {
                leftNode.Compile(new RequestParserTreeCompile ( request.Compiler, "set" ));

                return true;
            }

            if (this.Reference is null) return false;
            if (this.Reference.Deklaration is null) return false;
            if (this.Reference.Deklaration.Use is MethodeDeclarationNode t)
            {
                bool isok = this.CompileCopy(request.Compiler, leftNode, request.Mode, t);

                if (isok) return true;
            }

            this.FunctionsCall(request.Compiler, leftNode, request.Mode);

            return true;
        }

        private bool CompileCopy(Compiler.Compiler compiler, ICompileNode leftNode, string mode, MethodeDeclarationNode t)
        {
            if (t.AccessDefinition == null) return false;
            if (t.AccessDefinition.Kind != IdentifierKind.Copy) return false;
            if (t.Statement is not ICompileNode statement) return false;

            leftNode.Compile(new RequestParserTreeCompile (compiler, mode));

            statement.Compile(new RequestParserTreeCompile(compiler, "default"));

            return true;
        }

        private bool FunctionsCall(Compiler.Compiler compiler, ICompileNode leftNode, string mode)
        {
            if (this.Reference is null) return false;

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            leftNode.Compile(new RequestParserTreeCompile(compiler, mode));

            compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            this.OperatorCall.Compile(compiler, this.Reference, "methode");

            this.FunctionExecute.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods
    }
}