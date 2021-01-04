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

        public List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public MethodeType Zusatz
        {
            get;
            set;
        }

        public IndexVariabelnReference Type
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

                IndexVariabelnDeklaration dekValue = new IndexVariabelnDeklaration();
                dekValue.Name = "value";
                dekValue.Type = this.Type;
                dekValue.Use = this.Use;
                dekValue.ParentUsesSet = this.thisUses;

                this.Value = dekValue;

                IndexVariabelnDeklaration dekinValue = new IndexVariabelnDeklaration();
                dekinValue.Name = "invalue";
                dekinValue.Type = this.Type;
                dekinValue.Use = this.Use;
                dekinValue.ParentUsesSet = this.thisUses;

                this.InValue = dekinValue;

                this.thisUses = new ValidUses(this.ParentUsesSet);
                this.thisUses.Deklarationen = new List<IParent> { this, this.InValue, this.Value };

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }
        public IndexKlassenDeklaration Klasse { get; set; }
        public bool IsMapped
        {
            get;
            set;
        }

        public IndexPropertyDeklaration (  )
        {
            this.References = new List<IndexVariabelnReference>();
        }

        public bool PreMappen(ValidUses uses)
        {
            if (this.IsMapped) return false;

            this.ParentUsesSet = uses;

            this.Type.Mappen(uses);

            return true;
        }

        public bool Mappen()
        {
            if (this.IsMapped) return false;

            //this.SetContainer.Mappen(this.ThisUses);
            //this.GetContainer.Mappen(this.ThisUses);

            return this.IsMapped = true;
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
    }
}