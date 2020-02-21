using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator1ChildRight : IParseTreeNode, IPriority
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

        public List<string> ValidOperators
        {
            get;
        }

        public List<SyntaxKind> ValidChilds
        {
            get;
        }

        public List<SyntaxKind> Ausnahmen
        {
            get;
        }

        #endregion get/set

        #region ctor

        public Operator1ChildRight ( int prio )
        {
            this.Prio = prio;
        }

        public Operator1ChildRight ( List<string> validOperators, int prio, List<SyntaxKind> validChilds, List<SyntaxKind> ausnahmen )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
            this.ValidChilds = validChilds;
            this.Ausnahmen = ausnahmen;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode SwapChild ( IPriority node )
        {
            IParseTreeNode result = this.ChildNode;

            if ( result is IPriority t && t.Prio < node.Prio ) return t.SwapChild ( node );

            this.ChildNode = (IParseTreeNode)node;
            this.ChildNode.Token.ParentNode = this;

            return result;
        }

        private bool CheckHashValidChild ( SyntaxToken token )
        {
            if (token == null) return false;

            foreach ( SyntaxKind op in this.ValidChilds )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }

        private bool CheckAusnahmen ( SyntaxToken token )
        {
            if (token == null) return false;

            foreach ( SyntaxKind op in this.Ausnahmen )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }
        private bool CheckHashValidOperator ( SyntaxToken token )
        {
            foreach ( string op in this.ValidOperators )
            {
                if ( op == token.Text ) return true;
            }

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Operator ) return null;
            if ( !this.CheckHashValidOperator ( token ) ) return null;

            SyntaxToken lexerLeft = parser.Peek ( token, -1 );

            if ( this.CheckHashValidChild ( lexerLeft ) && !this.CheckAusnahmen ( lexerLeft ) ) return null;

            SyntaxToken lexerRight = parser.Peek ( token, 1 );

            if ( !this.CheckHashValidChild ( lexerRight ) ) return null;

            Operator1ChildRight node = new Operator1ChildRight ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.ChildNode = parser.ParseCleanToken ( lexerRight );

            if ( node.ChildNode == null ) return null;

            //if ( node.ChildNode is IPriority t && t.Prio < this.Prio ) node.ChildNode = t.SwapChild ( node );

            node.ChildNode.Token.ParentNode = node;

            return node;
        }

        #endregion methods
    }
}