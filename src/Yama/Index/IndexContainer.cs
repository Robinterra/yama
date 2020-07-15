using System.Collections.Generic;
using System.Linq;

namespace Yama.Index
{
    public class IndexContainer
    {

        public List<IndexContainer> Containers
        {
            get;
            set;
        }

        public List<IndexVariabelnDeklaration> VariabelnDeklarations
        {
            get;
            set;
        }

        public List<IndexMethodReference> MethodReferences
        {
            get;
            set;
        }

        public List<IndexVariabelnReference> VariabelnReferences
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
                this.thisUses.Deklarationen = this.VariabelnDeklarations.OfType<IParent>().ToList<IParent>();

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }
    }
}