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

        public VektorCall ( int prio )
        {
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

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != this.BeginZeichen ) return null;

            IdentifierToken left = parser.Peek ( token, -1 );

            if ( left.Kind == IdentifierKind.Operator ) return null;
            if ( left.Kind == IdentifierKind.NumberToken ) return null;
            if ( left.Kind == IdentifierKind.OpenBracket ) return null;
            if ( left.Kind == IdentifierKind.BeginContainer ) return null;
            if ( left.Kind == IdentifierKind.OpenSquareBracket ) return null;
            if ( left.Kind == IdentifierKind.EndOfCommand ) return null;
            if ( left.Kind == IdentifierKind.Comma ) return null;

            IdentifierToken steuerToken = parser.FindEndToken ( token, this.EndeZeichen, this.BeginZeichen );

            if ( steuerToken == null ) return null;

            VektorCall node = new VektorCall ( this.Prio );

            node.LeftNode = parser.ParseCleanToken ( left );

            node.ParametersNodes = parser.ParseCleanTokens ( token.Position + 1, steuerToken.Position, true );

            node.Token = token;
            token.Node = node;

            if (node.LeftNode == null) return null;

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

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            foreach (IParseTreeNode node in this.ParametersNodes)
            {
                node.Indezieren(index, container);
            }

            this.LeftNode.Indezieren(index, parent);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            List<IParseTreeNode> copylist = this.ParametersNodes.ToArray().ToList();
            copylist.Reverse();
            IParseTreeNode dek = null;

            int parasCount = 0;

            if (mode == "set")
            {
                CompileMovResult movResultRight = new CompileMovResult();

                movResultRight.Compile(compiler, null, "default");

                parasCount++;
            }

            foreach (IParseTreeNode par in copylist )
            {
                dek = par;
                if (par is EnumartionExpression b) dek = b.ExpressionParent;
                if (dek == null) continue;

                dek.Compile(compiler, "default");

                CompileMovResult movResultRight = new CompileMovResult();

                movResultRight.Compile(compiler, null, "default");

                parasCount++;
            }

            string modeCall = "vektorcall";
            if (mode == "set") modeCall = "setvektorcall";
            this.LeftNode.Compile(compiler, modeCall);
            if (!(this.LeftNode is OperatorPoint op)) return false;

            parasCount++;

            for (int i = 0; i < parasCount; i++)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.Compile(compiler, null);
            }

            this.FunctionExecute.Compile(compiler, null, "default");

            return true;
        }

        #endregion methods
    }
}