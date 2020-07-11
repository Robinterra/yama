using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class TextParser : IParseTreeNode, IPriority
    {

        #region get/set

        public SyntaxToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new List<IParseTreeNode> (  );
            }
        }

        public int Prio
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public TextParser (  )
        {

        }

        public TextParser ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Text ) return null;

            TextParser node = new TextParser { Token = token };

            token.Node = node;

            return node;
        }
    }
}