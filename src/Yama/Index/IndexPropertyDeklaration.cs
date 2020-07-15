using System.Collections.Generic;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexPropertyDeklaration : IParent
    {
        #region get/set

        public IndexKlassenDeklaration Parent
        {
            get;
            set;
        }

        public List<IndexPropertyReference> References
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public AccessModify AccessModify
        {
            get;
            set;
        }

        public IndexContainer GetContainer
        {
            get;
            set;
        }

        public IndexContainer SetContainer
        {
            get;
            set;
        }
        public IParseTreeNode Use { get; set; }

        private IndexVariabelnDeklaration Value
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
                this.thisUses.Deklarationen = new List<IParent> { this, this.Value };

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }

        #endregion get/set
    }
}