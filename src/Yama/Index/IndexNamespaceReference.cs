using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexNamespaceReference : IIndexReference, IParent
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

        public ValidUses ThisUses
        {
            get
            {
                return this.ParentUsesSet;
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

        public IndexNamespaceReference ( IParseTreeNode use, string name )
        {
            this.Name = name;
            this.Use = use;
            this.ParentUsesSet = new();
        }

        #endregion ctor

        #region methods

        public bool PreMappen(ValidUses uses)
        {
            return true;
        }

        public bool IsInUse (int depth)
        {
            return true;
        }

        #endregion methodss

    }
}