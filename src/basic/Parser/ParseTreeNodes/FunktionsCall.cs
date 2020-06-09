using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class FunktionsCall : IParseTreeNode, IEndExpression, IContainer
    {

        #region get/set

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

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

            FunktionsCall node = new FunktionsCall ( this.Prio );

            SyntaxToken steuerToken = parser.FindEndToken ( token, this.EndeZeichen, this.BeginZeichen );

            if ( steuerToken == null ) return null;

            SyntaxToken left = parser.Peek ( token, -1 );

            if ( left.Kind == SyntaxKind.Operator ) return null;
            if ( left.Kind == SyntaxKind.NumberToken ) return null;
            if ( left.Kind == SyntaxKind.OpenKlammer ) return null;
            if ( left.Kind == SyntaxKind.BeginContainer ) return null;
            if ( left.Kind == SyntaxKind.EckigeKlammerAuf ) return null;
            if ( left.Kind == SyntaxKind.EndOfCommand ) return null;
            if ( left.Kind == SyntaxKind.Comma ) return null;

            node.LeftNode = parser.ParseCleanToken ( left );

            node.ParametersNodes = parser.ParseCleanTokens ( token.Position + 1, steuerToken.Position, true );

            node.Token = token;
            token.Node = node;

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

        #endregion methods
    }
}