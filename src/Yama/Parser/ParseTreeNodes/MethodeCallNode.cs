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

        #region get/set

        public IndexMethodReference Reference
        {
            get;
            set;
        }

        public IParseTreeNode LeftNode
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

                result.Add ( this.LeftNode );
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
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public MethodeCallNode ( int prio )
        {
            this.Prio = prio;
        }

        public MethodeCallNode ( IdentifierKind begin, IdentifierKind end, int prio )
            : this ( prio )
        {
            this.BeginZeichen = begin;
            this.EndeZeichen = end;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != this.BeginZeichen ) return null;

            IdentifierToken left = request.Parser.Peek ( request.Token, -1 );

            if ( left.Kind == IdentifierKind.Operator ) return null;
            if ( left.Kind == IdentifierKind.NumberToken ) return null;
            if ( left.Kind == IdentifierKind.OpenBracket ) return null;
            if ( left.Kind == IdentifierKind.BeginContainer ) return null;
            if ( left.Kind == IdentifierKind.OpenSquareBracket ) return null;
            if ( left.Kind == IdentifierKind.EndOfCommand ) return null;
            if ( left.Kind == IdentifierKind.Comma ) return null;

            IdentifierToken steuerToken = request.Parser.FindEndToken ( request.Token, this.EndeZeichen, this.BeginZeichen );

            if ( steuerToken == null ) return null;

            MethodeCallNode node = new MethodeCallNode ( this.Prio );

            node.LeftNode = request.Parser.ParseCleanToken ( left );

            node.ParametersNodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, steuerToken.Position, true );

            node.Token = request.Token;

            if (node.LeftNode == null) return null;

            node.Token.Node = node;

            node.LeftNode.Token.ParentNode = node;
            node.Ende = steuerToken;

            steuerToken.ParentNode = node;
            steuerToken.Node = node;

            foreach ( IParseTreeNode n in node.ParametersNodes )
            {
                n.Token.ParentNode = node;
            }

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            foreach (IParseTreeNode node in this.ParametersNodes)
            {
                node.Indezieren(request);
            }

            this.LeftNode.Indezieren(request);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            List<IParseTreeNode> copylist = this.ParametersNodes.ToArray().ToList();
            copylist.Reverse();
            IParseTreeNode dek = null;

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
            if (!(this.LeftNode is OperatorPoint op)) return false;

            if (op.IsANonStatic) parasCount++;

            /*for (int i = 0; i < parasCount; i++)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.Compile(compiler, null);
            }*/

            this.FunctionExecute.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        #endregion methods
    }
}