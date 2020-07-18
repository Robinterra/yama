using System;
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

        private IndexVariabelnReference Value
        {
            get;
            set;
        }

        private IndexVariabelnDeklaration InValue
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
                this.thisUses.Deklarationen = new List<IParent> { this, this.InValue };

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }

        public IndexPropertyDeklaration (  )
        {
            this.References = new List<IndexPropertyReference>();
        }

        public bool Mappen(ValidUses uses)
        {
            this.ParentUsesSet = uses;

            this.InValue.Mappen(uses);
            this.Value.Mappen(uses);

            this.SetContainer.Mappen(this.ThisUses);
            this.GetContainer.Mappen(this.ThisUses);

            return true;
        }

        #endregion get/set
    }
}