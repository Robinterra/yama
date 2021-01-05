using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexNamespaceReference : IIndexReference, IParent
    {

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

        public IndexNamespaceDeklaration Deklaration
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

        public IndexNamespaceReference (  )
        {

        }

        public bool PreMappen(ValidUses uses) {return true;}
        public bool IsInUse (int depth)
        {
            return true;
        }

    }
}