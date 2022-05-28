using Yama.Parser;

namespace Yama.Index
{
    public class IndexPropertyGetSetDeklaration : IParent, IMethode
    {

        #region get/set

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

        public IndexKlassenDeklaration? Klasse
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string KeyName
        {
            get
            {
                return this.Name;
            }
        }

        public List<string> Tags
        {
            get;
            set;
        } = new List<string>();

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

        public IndexContainer? SetContainer
        {
            get;
            set;
        }

        public IndexContainer? GetContainer
        {
            get;
            set;
        }

        private ValidUses? thisUses;
        private ValidUses? getUses;

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
                if (this.thisUses.GetIndex is null) throw new NullReferenceException();

                if (this.ReturnValue.Deklaration is null) return this.thisUses;

                IndexVariabelnReference varref = new IndexVariabelnReference (this.Use, this.ReturnValue.Deklaration.Name) { Deklaration = this.ReturnValue.Deklaration };
                IndexVariabelnDeklaration dekThisVar = new IndexVariabelnDeklaration(this.Use, this.thisUses.GetIndex.Nameing.InValue, varref);

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
                string klassenName = "null";
                if (this.Klasse is not null) klassenName = this.Klasse.Name;

                string pattern = "{0}_{1}_{2}_Get";

                return string.Format(pattern, klassenName, this.NameInText, this.Parameters.Count);
            }
        }

        public string AssemblyNameSetMethode
        {
            get
            {
                string klassenName = "null";
                if (this.Klasse is not null) klassenName = this.Klasse.Name;

                string pattern = "{0}_{1}_{2}_Set";

                return string.Format(pattern, klassenName, this.NameInText, this.Parameters.Count);
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
            set;
        }

        #endregion get/set

        #region ctor

        public IndexPropertyGetSetDeklaration ( IParseTreeNode use, string name, IndexVariabelnReference returnValue )
        {
            this.ParentUsesSet = new();
            this.Use = use;
            this.Name = name;
            this.ReturnValue = returnValue;
            this.References = new List<IndexVariabelnReference>();
            this.Parameters = new List<IndexVariabelnDeklaration>();
        }

        #endregion ctor

        #region methods

        public bool Mappen()
        {
            if (this.IsMapped) return false;
            if (this.ThisUses.GetIndex is null) return false;

            this.ThisUses.GetIndex.CurrentMethode = this;

            if (this.SetContainer is not null) this.SetContainer.Mappen(this.SetUses);
            if (this.GetContainer is not null) this.GetContainer.Mappen(this.GetUses);

            return this.IsMapped = true;
        }

        public bool PreMappen(ValidUses uses)
        {
            if (this.IsMapped) return false;
            if (uses.GetIndex is null) return false;

            uses.GetIndex.CurrentMethode = this;
            this.ParentUsesSet = uses;

            this.ReturnValue.Mappen(this.ParentUsesSet);

            foreach (IndexVariabelnDeklaration dek in this.Parameters)
            {
                dek.Mappen(this.SetUses);

                if (dek.Name == uses.GetIndex.Nameing.InValue) continue;

                dek.Mappen(this.GetUses);
            }

            return true;
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