using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System;
using System.Linq;

namespace Yama.Parser
{
    public class MethodeCallNode : IParseTreeNode, IEndExpression, IContainer
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

        public MethodeCallNode ( IdentifierKind begin, IdentifierKind end, int prio )
        {
            this.Token = new();
            this.ParametersNodes = new List<IParseTreeNode>();
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
            this.BeginZeichen = begin;
            this.EndeZeichen = end;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != this.BeginZeichen ) return null;

            IdentifierToken? left = request.Parser.Peek ( request.Token, -1 );
            if (left is null) return null;
            if ( left.Kind == IdentifierKind.Operator ) return null;
            if ( left.Kind == IdentifierKind.NumberToken ) return null;
            if ( left.Kind == IdentifierKind.OpenBracket ) return null;
            if ( left.Kind == IdentifierKind.BeginContainer ) return null;
            if ( left.Kind == IdentifierKind.OpenSquareBracket ) return null;
            if ( left.Kind == IdentifierKind.EndOfCommand ) return null;
            if ( left.Kind == IdentifierKind.Comma ) return null;

            IdentifierToken? steuerToken = request.Parser.FindEndToken ( request.Token, this.EndeZeichen, this.BeginZeichen );
            if ( steuerToken is null ) return null;

            MethodeCallNode node = new MethodeCallNode ( this.BeginZeichen, this.EndeZeichen, this.Prio );

            node.AllTokens.Add ( steuerToken );
            node.ende = steuerToken;
            node.LeftNode = request.Parser.ParseCleanToken ( left );

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, steuerToken.Position, true );
            if (nodes is null) return null;

            node.ParametersNodes.AddRange(nodes);

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            if (node.LeftNode == null) return null;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.LeftNode is null) return request.Index.CreateError(this);

            IndexMethodReference methodReference = new IndexMethodReference();
            methodReference.Use = this;

            foreach (IParseTreeNode node in this.ParametersNodes)
            {
                node.Indezieren(request);

                IndexVariabelnReference? reference = container.VariabelnReferences.LastOrDefault();
                if (reference is null) return request.Index.CreateError(this);

                methodReference.Parameters.Add(reference);
            }

            this.LeftNode.Indezieren(request);

            IndexVariabelnReference? callRef = container.VariabelnReferences.LastOrDefault();
            if (callRef is null) return request.Index.CreateError(this);

            methodReference.CallRef = callRef;
            this.Reference = methodReference;

            container.MethodReferences.Add(methodReference);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.LeftNode is null) return false;

            List<IParseTreeNode> copylist = this.ParametersNodes.ToArray().ToList();
            copylist.Reverse();

            //if (this.CompileCopy(copylist, request)) return true;

            IParseTreeNode? dek = null;

            int parasCount = 0;

            foreach (IParseTreeNode par in copylist )
            {
                dek = par;
                if (par is EnumartionExpression b) dek = b.ExpressionParent;
                if (dek == null) continue;

                dek.Compile(request);

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            this.LeftNode.Compile(new Request.RequestParserTreeCompile(request.Compiler, "methode"));
            if (this.LeftNode is not OperatorPoint op) return false;

            if (op.IsANonStatic) parasCount++;

            /*for (int i = 0; i < parasCount; i++)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.Compile(compiler, null);
            }*/

            this.FunctionExecute.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        private bool CompileCopy(List<IParseTreeNode> copylist, Request.RequestParserTreeCompile request)
        {
            if (this.Reference is null) return false;
            if (this.LeftNode is null) return false;
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

            this.LeftNode.Compile(new Request.RequestParserTreeCompile(request.Compiler, "copy"));

            t.CompileCopy(new Request.RequestParserTreeCompile(request.Compiler, "default"));

            return true;
        }

        #endregion methods

    }
}