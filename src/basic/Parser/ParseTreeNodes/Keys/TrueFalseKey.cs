using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class TrueFalseKey : IParseTreeNode, IPriority
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
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                return result;
            }
        }
        public int Prio
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public TrueFalseKey (  )
        {

        }

        public TrueFalseKey ( int prio )
        {
            this.Prio = prio;
        }
        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            bool isok = token.Kind == SyntaxKind.True;

            if ( !isok ) isok = token.Kind == SyntaxKind.False;

            if ( !isok ) return null;

            TrueFalseKey result = new TrueFalseKey (  );

            token.Node = result;

            result.Token = token;

            return result;
        }

        #endregion methods

    }
}