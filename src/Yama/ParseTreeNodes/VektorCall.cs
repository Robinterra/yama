using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System;
using System.Linq;

namespace Yama.Parser
{
    public class VektorCall : IParseTreeNode, IIndexNode, ICompileNode, IContainer, IParentNode
    {

        #region vars

        private IdentifierToken? ende;

        private ParserLayer expressionLayer;

        #endregion vars

        #region get/set

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

        public VektorCall ( int prio, ParserLayer expressionLayer )
        {
            this.expressionLayer = expressionLayer;
            this.ParametersNodes = new();
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
        }

        public VektorCall ( IdentifierKind begin, IdentifierKind end, int prio, ParserLayer expressionLayer )
            : this ( prio, expressionLayer )
        {
            this.BeginZeichen = begin;
            this.EndeZeichen = end;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != this.BeginZeichen ) return null;

            IdentifierToken? steuerToken = request.Parser.FindEndToken ( request.Token, this.EndeZeichen, this.BeginZeichen );
            if ( steuerToken is null ) return null;

            VektorCall node = new VektorCall ( this.Prio, this.expressionLayer );

            request.Parser.ActivateLayer(this.expressionLayer);

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, steuerToken.Position, true );

            request.Parser.VorherigesLayer();

            if (nodes is null) return null;
            node.ParametersNodes.AddRange(nodes);

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            node.ende = steuerToken;
            node.AllTokens.Add(steuerToken);

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.LeftNode is not IIndexNode leftNode) return request.Index.CreateError(this);

            foreach (IParseTreeNode node in this.ParametersNodes)
            {
                if (node is not IIndexNode indexNode) continue;

                indexNode.Indezieren(request);
            }

            leftNode.Indezieren(request);

            return true;
        }

        private SSAVariableMap? GetParameterVariableMap()
        {
            if (this.LeftNode is not PointIdentifier pi) return null;
            if (pi.RightNode is not ReferenceCall rc) return null;
            if (rc.Reference is null) return null;

            if (rc.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsd) return this.GetParameterVariableMap(pgsd.ReturnValue.Deklaration, pgsd.Use.BorrowingToken is not null, pgsd.ReturnValue, pgsd.Use.SetStatement is null);
            if (rc.Reference.Deklaration is IndexVektorDeklaration pvd) return this.GetParameterVariableMap(pvd.ReturnValue.Deklaration, pvd.Use.BorrowingToken is not null, pvd.ReturnValue, pvd.Use.SetStatement is null);

            return null;
        }

        private SSAVariableMap? GetParameterVariableMap(IParent? deklaration, bool isBorrowing, IndexVariabelnReference varref, bool isNotMutable)
        {
            if (deklaration is not IndexKlassenDeklaration dk) return null;

            SSAVariableMap.VariableType kind = SSAVariableMap.VariableType.StackValue;
            IndexVariabelnDeklaration vardek = new IndexVariabelnDeklaration(this, dk.Name, varref);
            if (dk.MemberModifier == ClassMemberModifiers.None)
            {
                kind = isBorrowing ? SSAVariableMap.VariableType.BorrowingReference : SSAVariableMap.VariableType.OwnerReference;

                vardek.IsReference = true;
            }

            SSAVariableMap map = new SSAVariableMap(dk.Name, kind, vardek);
            map.MutableState = isNotMutable ? SSAVariableMap.VariableMutableState.NotMutable : SSAVariableMap.VariableMutableState.Mutable;

            return map;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.LeftNode is not ICompileNode leftNode) return false;

            List<IParseTreeNode> copylist = this.ParametersNodes;//.ToArray().ToList();
            copylist.Reverse();
            IParseTreeNode? dek = null;

            int parasCount = 0;

            if (request.Mode == "set")
            {
                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.ParameterType = this.GetParameterVariableMap();
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            foreach (IParseTreeNode par in copylist )
            {
                dek = par;
                if (dek is not ICompileNode compileNode) continue;

                compileNode.Compile(new RequestParserTreeCompile (request.Compiler, "default"));

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            string modeCall = "vektorcall";
            if (request.Mode == "set") modeCall = "setvektorcall";

            if (request.Compiler.ContainerMgmt.CurrentMethod is null) return false;
            CompileContainer currentMethod = request.Compiler.ContainerMgmt.CurrentMethod;

            bool nullError = currentMethod.NullCallsCanProduceErrors;
            currentMethod.NullCallsCanProduceErrors = true;

            leftNode.Compile(new RequestParserTreeCompile(request.Compiler, modeCall));

            currentMethod.NullCallsCanProduceErrors = nullError;

            if (this.LeftNode is not PointIdentifier op) return false;

            if (op.IsANonStatic) parasCount++;

            this.FunctionExecute.Compile(request.Compiler, null, "default");

            return true;
        }

        #endregion methods
    }
}