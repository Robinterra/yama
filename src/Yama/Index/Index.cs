using System.Collections.Generic;
using LearnCsStuf.Basic;

namespace Yama.Index
{
    public class Index
    {

        #region get/set

        public List<IParseTreeNode> Roots
        {
            get;
            set;
        }

        public ValidUses RootValidUses
        {
            get;
            set;
        }

        public List<IndexKlassenDeklaration> Register
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public Index (  )
        {
            this.Roots = new List<IParseTreeNode>();
            this.Register = new List<IndexKlassenDeklaration>();
        }

        #endregion ctor

    }

    public enum AccessModify
    {
        Public,
        Private
    }
}