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

        public IdentifierToken Value
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

        public IndexEnumEntryDeklaration ( IParseTreeNode use, string name, IdentifierToken value )
        {
            this.Value = value;
            this.ParentUsesSet = new();
            this.Name = name;
            this.Use = use;
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