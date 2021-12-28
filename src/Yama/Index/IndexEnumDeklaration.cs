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
        }

        public List<IndexEnumEntryDeklaration> Entries
        {
            get;
        }

        private ValidUses? thisUses;

        public ValidUses ThisUses
        {
            get
            {
                if (this.thisUses is not null) return this.thisUses;

                this.thisUses = new ValidUses(this.ParentUsesSet);

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }

        public bool IsMapped
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public IndexEnumDeklaration ( IParseTreeNode use, string name )
        {
            this.Use = use;
            this.Name = name;
            this.ParentUsesSet = new();
            this.Entries = new List<IndexEnumEntryDeklaration>();
            this.References = new List<IndexVariabelnReference>();
        }

        #endregion ctor

        #region methods

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

        public bool PreMappen(ValidUses uses)
        {
            return true;
        }

        #endregion methods
    }
}