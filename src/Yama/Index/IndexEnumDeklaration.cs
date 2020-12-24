using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexEnumDeklaration : IParent
    {

        #region get/set

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

        public List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        public List<IndexEnumEntryDeklaration> Entries
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

        public ValidUses ParentUsesSet { get;
        set; }

        private bool IsMapped
        {
            get;
            set;
        }

        
        public bool IsInUse (int depth)
        {
            if (depth > 10) return true;
            if (this.Name == "main") return true;

            depth += 1;

            foreach (IndexVariabelnReference reference in this.References)
            {
                if (reference.IsOwnerInUse(depth)) return true;
            }

            return false;
        }

        #endregion get/set

        #region ctor

        public IndexEnumDeklaration (  )
        {
            this.Entries = new List<IndexEnumEntryDeklaration>();
            this.References = new List<IndexVariabelnReference>();
        }

        #endregion ctor

        #region methods

        #endregion methods
    }
}