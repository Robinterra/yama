using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexEnumEntryDeklaration : IParent
    {

        #region get/set

        public List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        private ValidUses thisUses;

        public ValidUses ThisUses
        {
            get
            {
                if (this.thisUses != null) return this.thisUses;

                this.thisUses = new ValidUses(this.ParentUsesSet);

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }

        public SyntaxToken Value
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public IndexEnumEntryDeklaration (  )
        {
            this.References = new List<IndexVariabelnReference>();
        }

        #endregion ctor

        #region methods

        #endregion methods
    }
}