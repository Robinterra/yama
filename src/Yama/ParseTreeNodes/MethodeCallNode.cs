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

            callRef.IsMethodCalled = true;
            methodReference.CallRef = callRef;
            this.Reference = methodReference;

            container.MethodReferences.Add(methodReference);

            return true;
        }

        private SSAVariableMap? GetParameterVariableMap(bool isBorrowing, IndexVariabelnDeklaration vardek, bool isNullable)
        {
            if (vardek.Type.Deklaration is not IndexKlassenDeklaration dk) return null;

            SSAVariableMap.VariableType kind = SSAVariableMap.VariableType.StackValue;
            if (dk.MemberModifier == ClassMemberModifiers.None)
            {
                kind = isBorrowing ? SSAVariableMap.VariableType.BorrowingReference : SSAVariableMap.VariableType.OwnerReference;

                vardek.IsReference = true;
                vardek.IsNullable = isNullable;
            }

            SSAVariableMap map = new SSAVariableMap(dk.Name, kind, vardek);
            map.MutableState = SSAVariableMap.VariableMutableState.Mutable;

            return map;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.LeftNode is not ICompileNode leftNode) return false;
            if (this.Reference is null) return request.Compiler.AddError("error reference is null", this);
            if (this.Reference.DeklarationDelegate is not null) return this.Compile(request, this.Reference, this.Reference.DeklarationDelegate, leftNode);
            if (this.Reference.Deklaration is null) return request.Compiler.AddError("error deklaratrion is null", this);
            IndexMethodDeklaration methodDeklaration = this.Reference.Deklaration;

            List<IParseTreeNode> copylist = this.ParametersNodes.ToArray().ToList();
            copylist.Reverse();

            //if (this.CompileCopy(copylist, request)) return true;

            int parasCount = 0;

            int lengthOfParameters = this.StructResult(request, methodDeklaration);
            if (lengthOfParameters == -2) return false;
            if (lengthOfParameters == -1) return request.Compiler.AddError("the method is returning a struct, please assigment the return value to a variable", this);

            foreach (IParseTreeNode par in copylist )
            {
                if (par is not ICompileNode compileNode) continue;

                int q = lengthOfParameters - parasCount - 1;
                if (q < 0) return request.Compiler.AddError("this method call is not allowed, save the result in a variable", this);
                IndexVariabelnDeklaration varDek = methodDeklaration.Parameters[q];

                bool isBorrowing = false;
                bool isNullable = false;
                if (varDek.Use is VariabelDeklaration vd)
                {
                    isBorrowing = vd.BorrowingToken is not null;
                    isNullable = vd.NullableToken is not null;
                }

                compileNode.Compile(request);

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.ParameterType = this.GetParameterVariableMap(isBorrowing, varDek, isNullable);
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            if (request.Compiler.ContainerMgmt.CurrentMethod is null) return false;
            CompileContainer currentMethod = request.Compiler.ContainerMgmt.CurrentMethod;

            bool nullError = currentMethod.NullCallsCanProduceErrors;
            currentMethod.NullCallsCanProduceErrors = true;

            leftNode.Compile(new RequestParserTreeCompile(request.Compiler, "methode"));

            currentMethod.NullCallsCanProduceErrors = nullError;

            if (this.LeftNode is not PointIdentifier op) return false;

            if (op.IsANonStatic) parasCount++;

            /*for (int i = 0; i < parasCount; i++)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.Compile(compiler, null);
            }*/

            this.FunctionExecute.Compile(request.Compiler, this, request.Mode);

            return true;
        }

        private bool Compile(RequestParserTreeCompile request, IndexMethodReference reference, IndexDelegateDeklaration deklarationDelegate, ICompileNode leftNode)
        {
            List<IParseTreeNode> copylist = this.ParametersNodes.ToArray().ToList();
            copylist.Reverse();

            //if (this.CompileCopy(copylist, request)) return true;

            int parasCount = 0;

            int lengthOfParameters = this.StructResult(request, deklarationDelegate);
            if (lengthOfParameters == -2) return false;
            if (lengthOfParameters == -1) return request.Compiler.AddError("the method is returning a struct, please assigment the return value to a variable", this);

            foreach (IParseTreeNode par in copylist )
            {
                if (par is not ICompileNode compileNode) continue;

                IndexVariabelnDeklaration varDek = deklarationDelegate.Parameters[lengthOfParameters - parasCount - 1];

                bool isBorrowing = varDek.IsBorrowing;
                bool isNullable = varDek.IsNullable;

                compileNode.Compile(request);

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.ParameterType = this.GetParameterVariableMap(isBorrowing, varDek, isNullable);
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            if (request.Compiler.ContainerMgmt.CurrentMethod is null) return false;
            CompileContainer currentMethod = request.Compiler.ContainerMgmt.CurrentMethod;

            bool nullError = currentMethod.NullCallsCanProduceErrors;
            currentMethod.NullCallsCanProduceErrors = true;

            leftNode.Compile(new RequestParserTreeCompile(request.Compiler, "methode"));

            currentMethod.NullCallsCanProduceErrors = nullError;

            //if (this.LeftNode is not PointIdentifier op) return false;

            //if (op.IsANonStatic) parasCount++;

            /*for (int i = 0; i < parasCount; i++)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.Compile(compiler, null);
            }*/

            this.FunctionExecute.Compile(request.Compiler, this, request.Mode);

            return true;
        }

        private int StructResult(RequestParserTreeCompile request, IndexDelegateDeklaration methodDeklaration)
        {
            int res = methodDeklaration.Parameters.Count;

            if (methodDeklaration.ReturnValue.Deklaration is IndexKlassenDeklaration ikd)
            {
                if (ikd.MemberModifier == ClassMemberModifiers.Struct) res = -1;
            }

            if (request.StructLeftNode is null) return res;
            if (res != -1)
            {
                request.Compiler.AddError("The method is not given a struct back", this);
                return -2;
            }

            res = methodDeklaration.Parameters.Count;

            CompileReferenceCall call = new CompileReferenceCall();
            call.GetVariableCompile(request.Compiler, request.StructLeftNode, this);

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.ParameterType = this.GetParameterVariableMap(false, request.StructLeftNode, false);
            compilePushResult.Compile(request.Compiler, null, "default");

            if (!methodDeklaration.Parameters.Any(t=>t.Name == "return")) return res;

            return res - 1;
        }

        private int StructResult(RequestParserTreeCompile request, IndexMethodDeklaration methodDeklaration)
        {
            int res = methodDeklaration.Parameters.Count;

            if (methodDeklaration.ReturnValue.Deklaration is IndexKlassenDeklaration ikd)
            {
                if (ikd.MemberModifier == ClassMemberModifiers.Struct) res = -1;
            }

            if (request.StructLeftNode is null) return res;
            if (res != -1)
            {
                request.Compiler.AddError("The method is not given a struct back", this);
                return -2;
            }

            res = methodDeklaration.Parameters.Count;

            CompileReferenceCall call = new CompileReferenceCall();
            call.GetVariableCompile(request.Compiler, request.StructLeftNode, this);

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.ParameterType = this.GetParameterVariableMap(false, request.StructLeftNode, false);
            compilePushResult.Compile(request.Compiler, null, "default");

            if (!methodDeklaration.Parameters.Any(t=>t.Name == "return")) return res;

            return res - 1;
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