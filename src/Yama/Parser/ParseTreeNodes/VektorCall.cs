using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System;
using System.Linq;

namespace Yama.Parser
{
    public class VektorCall : IParseTreeNode, IEndExpression, IContainer
    {

        #region get/set

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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public VektorCall ( int prio )
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
        }

        public VektorCall ( IdentifierKind begin, IdentifierKind end, int prio )
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

            VektorCall node = new VektorCall ( this.Prio );

            node.LeftNode = request.Parser.ParseCleanToken ( left );

            node.ParametersNodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, steuerToken.Position, true );

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            if (node.LeftNode == null) return null;

            node.Ende = steuerToken;
            node.AllTokens.Add(steuerToken);

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
            List<IParseTreeNode> copylist = this.ParametersNodes;//.ToArray().ToList();
            copylist.Reverse();
            IParseTreeNode dek = null;

            int parasCount = 0;

            if (request.Mode == "set")
            {
                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            foreach (IParseTreeNode par in copylist )
            {
                dek = par;
                if (par is EnumartionExpression b) dek = b.ExpressionParent;
                if (dek == null) continue;

                dek.Compile(new Request.RequestParserTreeCompile (request.Compiler, "default"));

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            string modeCall = "vektorcall";
            if (request.Mode == "set") modeCall = "setvektorcall";
            this.LeftNode.Compile(new Request.RequestParserTreeCompile(request.Compiler, modeCall));
            if (!(this.LeftNode is OperatorPoint op)) return false;

            if (op.IsANonStatic) parasCount++;

            this.FunctionExecute.Compile(request.Compiler, null, "default");

            return true;
        }

        #endregion methods
    }
}