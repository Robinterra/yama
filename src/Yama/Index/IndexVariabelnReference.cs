using System.Collections.Generic;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexVariabelnReference : IIndexReference, IParent
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

        public IParent Deklaration
        {
            get;
            set;
        }
        public List<IndexVariabelnReference> VariabelnReferences
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
    }
}