using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System;

namespace Yama.Parser
{
    public class FunktionsCall : IParseTreeNode, IEndExpression, IContainer
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

        public SyntaxToken Token
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

        public SyntaxKind BeginZeichen
        {
            get;
            set;
        }

        public SyntaxKind EndeZeichen
        {
            get;
            set;
        }
        public SyntaxToken Ende
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public FunktionsCall ( int prio )
        {
            this.Prio = prio;
        }

        public FunktionsCall ( SyntaxKind begin, SyntaxKind end, int prio )
            : this ( prio )
        {
            this.BeginZeichen = begin;
            this.EndeZeichen = end;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != this.BeginZeichen ) return null;

            SyntaxToken left = parser.Peek ( token, -1 );

            if ( left.Kind == SyntaxKind.Operator ) return null;
            if ( left.Kind == SyntaxKind.NumberToken ) return null;
            if ( left.Kind == SyntaxKind.OpenKlammer ) return null;
            if ( left.Kind == SyntaxKind.BeginContainer ) return null;
            if ( left.Kind == SyntaxKind.EckigeKlammerAuf ) return null;
            if ( left.Kind == SyntaxKind.EndOfCommand ) return null;
            if ( left.Kind == SyntaxKind.Comma ) return null;

            SyntaxToken steuerToken = parser.FindEndToken ( token, this.EndeZeichen, this.BeginZeichen );

            if ( steuerToken == null ) return null;

            FunktionsCall node = new FunktionsCall ( this.Prio );

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
            for (int i = this.ParametersNodes.Count; i > 0; i-- )
            {
                this.ParametersNodes[i - 1].Compile(compiler, mode);

                CompileMovResult movResultRight = new CompileMovResult();

                movResultRight.Compile(compiler, null, mode);
            }

            this.LeftNode.Compile(compiler, "methode");
            if (!(this.LeftNode is OperatorPoint op)) return false;

            int parasCount = this.ParametersNodes.Count;
            if (op.IsANonStatic) parasCount++;

            for (int i = 0; i < parasCount; i++)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.Compile(compiler, null);
            }

            this.FunctionExecute.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods
    }
}