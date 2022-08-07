using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System;
using System.Linq;

namespace Yama.Parser
{
    public class MethodeCallNode : IParseTreeNode, IIndexNode, ICompileNode, IContainer, IParentNode
    {

        #region vars

        private IdentifierToken? ende;

        #endregion vars

        #region get/set

        public IndexMethodReference? Reference
        {
            get;
            set;
        }

        public IParseTreeNode? LeftNode
        {
            get;
            set;
        }

        public CompileExecuteCall FunctionExecute
        {
            get;
            set;
        } = new CompileExecuteCall();

        public List<IParseTreeNode> ParametersNodes
        {
            get;
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

                if (this.LeftNode is not null) result.Add ( this.LeftNode );
                result.AddRange ( this.ParametersNodes );

                return result;
            }
        }

        public IdentifierKind BeginZeichen
        {
            get;
            set;
        }

        public IdentifierKind EndeZeichen
        {
            get;
            set;
        }

        private ParserLayer expressionLayer;

        public IdentifierToken Ende
        {
            get
            {
                if (this.ende is null) return this.Token;

                return this.ende;
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public MethodeCallNode ( IdentifierKind begin, IdentifierKind end, int prio, ParserLayer expressionLayer )
        {
            this.Token = new();
            this.ParametersNodes = new List<IParseTreeNode>();
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
            this.BeginZeichen = begin;
            this.EndeZeichen = end;
            this.expressionLayer = expressionLayer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != this.BeginZeichen ) return null;

            IdentifierToken? steuerToken = request.Parser.FindEndToken ( request.Token, this.EndeZeichen, this.BeginZeichen );
            if ( steuerToken is null ) return null;

            MethodeCallNode node = new MethodeCallNode ( this.BeginZeichen, this.EndeZeichen, this.Prio, this.expressionLayer );

            node.AllTokens.Add ( steuerToken );
            node.ende = steuerToken;

            request.Parser.ActivateLayer(this.expressionLayer);

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, steuerToken.Position, true );

            request.Parser.VorherigesLayer();

            if (nodes is null) return null;

            node.ParametersNodes.AddRange(nodes);

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.LeftNode is not IIndexNode leftNode) return request.Index.CreateError(this);

            IndexMethodReference methodReference = new IndexMethodReference(this, this.Token.Text);

            foreach (IParseTreeNode node in this.ParametersNodes)
            {
                if (node is not IIndexNode indexNode) continue;

                indexNode.Indezieren(request);

                IndexVariabelnReference? reference = container.VariabelnReferences.LastOrDefault();
                if (reference is null) return request.Index.CreateError(this);

                methodReference.Parameters.Add(reference);
            }

            leftNode.Indezieren(request);

            IndexVariabelnReference? callRef = container.VariabelnReferences.LastOrDefault();
            if (callRef is null) return request.Index.CreateError(this);

            methodReference.CallRef = callRef;
            this.Reference = methodReference;

            container.MethodReferences.Add(methodReference);

            return true;
        }

        private SSAVariableMap? GetParameterVariableMap(bool isBorrowing, IndexVariabelnDeklaration vardek)
        {
            if (vardek.Type.Deklaration is not IndexKlassenDeklaration dk) return null;

            SSAVariableMap.VariableType kind = SSAVariableMap.VariableType.Primitive;
            if (dk.MemberModifier == ClassMemberModifiers.None)
            {
                kind = isBorrowing ? SSAVariableMap.VariableType.BorrowingReference : SSAVariableMap.VariableType.OwnerReference;

                vardek.IsNullable = true;
            }

            SSAVariableMap map = new SSAVariableMap(dk.Name, kind, vardek);
            map.MutableState = SSAVariableMap.VariableMutableState.Mutable;

            return map;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.LeftNode is not ICompileNode leftNode) return false;
            if (this.Reference is null) return request.Compiler.AddError("error reference is null", this);
            if (this.Reference.Deklaration is null) return request.Compiler.AddError("error deklaratrion is null", this);
            IndexMethodDeklaration methodDeklaration = this.Reference.Deklaration;

            List<IParseTreeNode> copylist = this.ParametersNodes.ToArray().ToList();
            copylist.Reverse();

            //if (this.CompileCopy(copylist, request)) return true;

            int parasCount = 0;

            foreach (IParseTreeNode par in copylist )
            {
                if (par is not ICompileNode compileNode) continue;

                IndexVariabelnDeklaration varDek = methodDeklaration.Parameters[copylist.Count - parasCount - 1];

                bool isBorrowing = false;
                if (varDek.Use is VariabelDeklaration vd) isBorrowing = vd.BorrowingToken is not null;

                compileNode.Compile(request);

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.ParameterType = this.GetParameterVariableMap(isBorrowing, varDek);
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            leftNode.Compile(new RequestParserTreeCompile(request.Compiler, "methode"));
            if (this.LeftNode is not PointIdentifier op) return false;

            if (op.IsANonStatic) parasCount++;

            /*for (int i = 0; i < parasCount; i++)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.Compile(compiler, null);
            }*/

            this.FunctionExecute.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        /*private bool CompileCopy(List<IParseTreeNode> copylist, RequestParserTreeCompile request)
        {
            if (this.Reference is null) return false;
            if (this.Reference.Deklaration is null) return false;
            if (this.Reference.Deklaration.Use is not MethodeDeclarationNode t) return false;
            if (t.Deklaration is null) return false;

            //TODO: Not in this place
            if (t.Deklaration.References.Count == 2 && t.ZusatzDefinition == null)
            {
                if (t.AccessDefinition == null) t.AccessDefinition = new IdentifierToken();

                t.AccessDefinition.Kind = IdentifierKind.Copy;
            }

            if (t.AccessDefinition == null) return false;
            if (t.AccessDefinition.Kind != IdentifierKind.Copy) return false;

            foreach ( IParseTreeNode parameter in copylist )
            {
                parameter.Compile(request);
            }

            this.LeftNode.Compile(new RequestParserTreeCompile(request.Compiler, "copy"));

            t.CompileCopy(new RequestParserTreeCompile(request.Compiler, "default"));

            return true;
        }*/

        #endregion methods

    }
}