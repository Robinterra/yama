using System.Collections.Generic;
using LearnCsStuf.Basic;

namespace Yama.Index
{
    public class IndexVariabelnDeklaration : IParent
    {
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

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public IParent Type
        {
            get;
            set;
        }

        public ValidUses ThisUses => throw new System.NotImplementedException();

        public ValidUses ParentUsesSet { get; set; }
    }
}