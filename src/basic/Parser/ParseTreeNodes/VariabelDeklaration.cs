using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class VariabelDeklaration : IParseTreeNode, IPriority
    {

        #region get/set

        public IParseTreeNode ChildNode
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

                result.Add ( this.ChildNode );

                return result;
            }
        }

        public List<SyntaxKind> Ausnahmen
        {
            get;
        }

        #endregion get/set

        #region ctor

        public VariabelDeklaration ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidChild ( SyntaxToken token )
        {
            if (token == null) return false;

            if (token.Kind == SyntaxKind.Word) return true;

            return false;
        }

        /*private bool CheckAusnahmen ( SyntaxToken token )
        {
            if (token == null) return false;

            foreach ( SyntaxKind op in this.Ausnahmen )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }*/
        private bool CheckHashValidOperator ( SyntaxToken token )
        {
            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Char) return true;
            if (token.Kind == SyntaxKind.Byte) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;
            if (token.Kind == SyntaxKind.Float32Bit) return true;

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Operator ) return null;
            if ( !this.CheckHashValidOperator ( token ) ) return null;

            SyntaxToken lexerLeft = parser.Peek ( token, -1 );

            if ( this.CheckHashValidChild ( lexerLeft ) /*&& !this.CheckAusnahmen ( lexerLeft )*/ ) return null;

            SyntaxToken lexerRight = parser.Peek ( token, 1 );

            if ( !this.CheckHashValidChild ( lexerRight ) ) return null;

            Operator1ChildRight node = new Operator1ChildRight ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.ChildNode = parser.ParseCleanToken ( lexerRight );

            if ( node.ChildNode == null ) return null;

            node.ChildNode.Token.ParentNode = node;

            return node;
        }

        #endregion methods
    }
}