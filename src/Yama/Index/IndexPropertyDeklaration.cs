using System;
using System.Collections.Generic;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexPropertyDeklaration : IParent
    {
        #region get/set

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

        public string AssemblyName
        {
            get
            {
                string klassenName = "null";
                if (this.Klasse is not null) klassenName = this.Klasse.Name;

                string pattern = "{0}_{1}_StaticProperty";

                return string.Format(pattern, klassenName, this.Name);
            }
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

        public IndexContainer? SetContainer
        {
            get;
            set;
        }

        IParseTreeNode IParent.Use
        {
            get
            {
                return this.Use;
            }
            set
            {
                this.Use = (value as PropertyDeklaration)!;
            }
        }

        public PropertyDeklaration Use
        {
            get;
            set;
        }

        private IndexVariabelnDeklaration? Value
        {
            get;
            set;
        }

        private IndexVariabelnDeklaration? InValue
        {
            get;
            set;
        }

        private ValidUses? thisUses;

        public ValidUses ThisUses
        {
            get
            {
                if (this.thisUses != null) return this.thisUses;

                IndexVariabelnDeklaration dekValue = new IndexVariabelnDeklaration(this.Use, "value", this.Type);
                dekValue.ParentUsesSet = this.ParentUsesSet;

                this.Value = dekValue;

                IndexVariabelnDeklaration dekinValue = new IndexVariabelnDeklaration(this.Use, "invalue", this.Type);
                dekinValue.ParentUsesSet = this.ParentUsesSet;

                this.InValue = dekinValue;

                this.thisUses = new ValidUses(this.ParentUsesSet);
                this.thisUses.Deklarationen = new List<IParent> { this, this.InValue, this.Value };

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }

        public IndexKlassenDeklaration? Klasse
        {
            get;
            set;
        }

        public bool IsMapped
        {
            get;
            set;
        }

        public int Position
        {
            get
            {
                int result = 0;
                if (this.Klasse is null) return -1;
                if (this.Klasse.IsMethodsReferenceMode) result++;

                foreach (IndexPropertyDeklaration property in this.Klasse.IndexProperties)
                {
                    result += 1;
                    if (property != this) continue;
                    result -= 1;

                    return result;
                }

                return -1;
            }
        }

        public GenericCall? GenericDeklaration
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public IndexPropertyDeklaration ( PropertyDeklaration use, string name, MethodeType methodeType, IndexVariabelnReference varTyp )
        {
            this.ParentUsesSet = new();
            this.Type = varTyp;
            this.Zusatz = methodeType;
            this.Use = use;
            this.Name = name;
            this.GetContainer = new IndexContainer(use, "contianer");
            this.References = new List<IndexVariabelnReference>();
        }

        #endregion ctor

        #region methods

        public bool PreMappen(ValidUses uses)
        {
            if (this.IsMapped) return false;

            this.ParentUsesSet = uses;

            this.Type.GenericDeklaration = this.GenericDeklaration;
            this.Type.Mappen(uses);

            return true;
        }

        public bool Mappen()
        {
            if (this.IsMapped) return false;

            //this.SetContainer.Mappen(this.ThisUses);
            this.GetContainer.Mappen(this.ThisUses);

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

        #endregion methods

    }
}