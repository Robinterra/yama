using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexVaktorDeklaration : IParent
    {

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        public IndexKlassenDeklaration Klasse
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public MethodeType Type
        {
            get;
            set;
        }

        public AccessModify AccessModify
        {
            get;
            set;
        }

        public IndexVariabelnReference ReturnValue
        {
            get;
            set;
        }

        public List<IndexVariabelnDeklaration> Parameters
        {
            get;
            set;
        }

        public IndexContainer SetContainer
        {
            get;
            set;
        }

        public IndexContainer GetContainer
        {
            get;
            set;
        }

        private ValidUses thisUses;
        private ValidUses getUses;

        public ValidUses ThisUses
        {
            get
            {
                return this.SetUses;
            }
        }

        public ValidUses SetUses
        {
            get
            {
                if (this.thisUses != null) return this.thisUses;

                this.thisUses = new ValidUses(this.ParentUsesSet);

                IndexVariabelnDeklaration dekThisVar = new IndexVariabelnDeklaration();
                dekThisVar.Name = "invalue";
                dekThisVar.Type = new IndexVariabelnReference { Deklaration = this.ReturnValue.Deklaration, Name = this.ReturnValue.Deklaration.Name, Use = this.Use };
                dekThisVar.Use = this.Use;
                dekThisVar.SetUsesSet = this.thisUses;

                this.References.Add(dekThisVar.Type);

                List<IParent> dekList = new List<IParent> { dekThisVar };

                this.thisUses.Deklarationen = dekList;

                return this.thisUses;
            }
        }

        public ValidUses GetUses
        {
            get
            {
                if (this.getUses != null) return this.getUses;

                this.getUses = new ValidUses(this.ParentUsesSet);

                return this.getUses;
            }
        }

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }
        public string AssemblyNameGetMethode
        {
            get
            {
                string pattern = "{0}_{1}_{2}_Get";

                return string.Format(pattern, this.Klasse.Name, this.NameInText, this.Parameters.Count);
            }
        }

        public string AssemblyNameSetMethode
        {
            get
            {
                string pattern = "{0}_{1}_{2}_Set";

                return string.Format(pattern, this.Klasse.Name, this.NameInText, this.Parameters.Count);
            }
        }

        public string NameInText
        {
            get
            {
                return this.Name;
            }
        }

        public bool IsMapped
        {
            get;
            private set;
        }

        public IndexVaktorDeklaration (  )
        {
            this.References = new List<IndexVariabelnReference>();
            this.Parameters = new List<IndexVariabelnDeklaration>();
        }

        public bool Mappen()
        {
            if (this.IsMapped) return false;

            this.SetContainer.Mappen(this.SetUses);
            this.GetContainer.Mappen(this.GetUses);

            return this.IsMapped = true;
        }

        public bool PreMappen(ValidUses uses)
        {
            if (this.IsMapped) return false;

            this.ParentUsesSet = uses;

            this.ReturnValue.Mappen(this.ParentUsesSet);

            foreach (IndexVariabelnDeklaration dek in this.Parameters)
            {
                dek.Mappen(this.SetUses);

                if (dek.Name == "invalue") continue;

                dek.Mappen(this.GetUses);
            }

            return true;
        }
    }
}