using Yama.Parser;

namespace Yama.Index
{
    public class IndexVariabelnReference : IIndexReference, IParent
    {

        #region get/set

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

        public IParent? Deklaration
        {
            get;
            set;
        }

        public IParent? Owner
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

        public bool IsPointIdentifier
        {
            get;
            set;
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

        public List<IMethode>? OverloadMethods
        {
            get;
            set;
        }

        private IndexVariabelnReference? parentCall;

        public IndexVariabelnReference? ParentCall
        {
            get
            {
                return parentCall;
            }
            set
            {
                if (parentCall != null) parentCall.ParentCall = value;
                else this.parentCall = value;
            }
        }

        public string AssemblyName
        {
            get
            {
                if (this.Deklaration is null) return "null";

                if (this.Deklaration is IndexMethodDeklaration t) return t.AssemblyName;
                if (this.Deklaration is IndexPropertyDeklaration pd) return pd.AssemblyName;
                //if (this.Deklaration is IndexPropertyGetSetDeklaration pgsd) return pgsd.AssemblyName;

                return this.Deklaration.Name;
            }
        }

        public bool IsOperator
        {
            get;
            set;
        }

        public GenericCall? GenericDeklaration
        {
            get;
            set;
        }

        public GenericCall? ClassGenericDefinition
        {
            get;
            set;
        }

        public IndexVariabelnReference? RefCombination
        {
            get;
            set;
        }

        public IndexVariabelnReference? ChildUse
        {
            get;
            set;
        }

        private bool isMethodCalled;
        public bool IsMethodCalled
        {
            get
            {
                return this.isMethodCalled;
            }
            set
            {
                this.isMethodCalled = value;
                if (this.ParentCall is null) return;
                this.ParentCall.IsMethodCalled = value;
            }
        }

        private bool? isownerinUser;

        #endregion get/set

        #region ctor

        public IndexVariabelnReference ( IParseTreeNode use, string name )
        {
            this.Use = use;
            this.Name = name;
            this.ParentUsesSet = new();
            this.VariabelnReferences = new List<IndexVariabelnReference>();
        }

        #endregion ctor

        #region methods

        public bool IsOwnerInUse(int depth)
        {
            if (this.isownerinUser is not null) return (bool)this.isownerinUser;
            if (this.Owner == null) return (bool)(this.isownerinUser = false);

            return (bool)(this.isownerinUser = this.Owner.IsInUse(depth));
        }

        public bool IsInUse (int depth)
        {
            if (depth > 10) return true;

            return this.IsOwnerInUse(depth + 1);
        }

        public bool Mappen(IndexVariabelnReference parentCall)
        {
            this.Owner = parentCall.Owner;

            if ( this.ChildUse != null ) parentCall = this.GetChildUse ( this.ChildUse );
            if ( this.IsMapped ) return true;

            this.IsMapped = true;

            this.ParentUsesSet = parentCall.ParentUsesSet;

            if (parentCall.Deklaration is IndexVariabelnDeklaration vd) return this.VariableDeklarationMappen (parentCall, vd);

            if (parentCall.Deklaration is IndexKlassenDeklaration kd) return this.ClassMappen(kd, parentCall);

            if (parentCall.Deklaration is IndexEnumDeklaration ed) return this.EnumMappen(ed, parentCall);

            if (parentCall.Deklaration is IndexPropertyDeklaration pd) return this.PropertyMappen(pd, parentCall);

            if (parentCall.Deklaration is IndexPropertyGetSetDeklaration pgsd) return this.GetSetMappen(pgsd, parentCall);

            if (parentCall.Deklaration is IndexMethodDeklaration md) return this.MethodMappen(md, parentCall);

            if (parentCall.ParentUsesSet.GetIndex is null) return false;
            return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / regular");
        }

        private IndexVariabelnReference GetChildUse ( IndexVariabelnReference childUse )
        {
            if ( childUse.ParentCall == null ) return childUse;
            if ( this.Equals ( childUse.ParentCall ) ) return childUse;

            return this.GetChildUse ( childUse.ParentCall );
        }

        private bool MethodMappen(IndexMethodDeklaration md, IndexVariabelnReference parentCall)
        {
            IParent? dek = null;

            if (this.IsOperator && md.ReturnValue.Deklaration is IndexKlassenDeklaration klasse)
            {
                dek = this.GetStaticFound(klasse);
            }

            if (dek != null)
            {
                this.Deklaration = dek;

                if (this.ParentCall != null) this.ParentCall.Mappen(this);

                return true;
            }

            if (md.Klasse is null) return false;
            if (parentCall.Deklaration is null) return false;

            if (parentCall.Name == parentCall.Deklaration.Name) dek = this.GetStaticFound(md.Klasse);
            else dek = this.GetKlassenFound(md.Klasse, parentCall);

            if (dek == null)
            {
                if (parentCall.ParentUsesSet.GetIndex is null) return false;

                return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / method");
            }

            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool GetSetMappen(IndexPropertyGetSetDeklaration pgsd, IndexVariabelnReference parentCall)
        {
            //if (parentCall.Name == parentCall.Deklaration.Name) dek = this.GetStaticFound(pd.Type.Deklaration as IndexKlassenDeklaration);
            //else 

            IParent? dek = this.GetKlassenFound(pgsd.ReturnValue.Deklaration as IndexKlassenDeklaration, parentCall);
            if (dek is null)
            {
                if (parentCall.ParentUsesSet.GetIndex is null) return false;

                return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / prop dek");
            }

            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool PropertyMappen(IndexPropertyDeklaration pd, IndexVariabelnReference parentCall)
        {
            //if (parentCall.Name == parentCall.Deklaration.Name) dek = this.GetStaticFound(pd.Type.Deklaration as IndexKlassenDeklaration);
            //else 

            IParent? dek = this.GetKlassenFound(pd.Type.Deklaration as IndexKlassenDeklaration, parentCall);

            if (dek is null)
            {
                if (parentCall.ParentUsesSet.GetIndex is null) return false;

                return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / property dek");
            }
            this.Deklaration = dek;

            this.ClassGenericDefinition = parentCall.GenericDeklaration;
            this.GenericDeklaration = pd.GenericDeklaration;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool EnumMappen(IndexEnumDeklaration ed, IndexVariabelnReference parentCall)
        {
            IParent? dek = this.GetEnumFound(ed);

            if (dek is null)
            {
                if (parentCall.ParentUsesSet.GetIndex is null) return false;

                return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / enum dek");
            }
            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool ClassMappen(IndexKlassenDeklaration kd, IndexVariabelnReference parentCall)
        {
            IParent? dek = this.GetStaticFound(kd);

            if (dek is null)
            {
                if (parentCall.ParentUsesSet.GetIndex is null) return false;

                return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / call dek");
            }
            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool VariableDeklarationMappen(IndexVariabelnReference parentCall, IndexVariabelnDeklaration vd)
        {
            if (vd.Type.Deklaration is not IndexKlassenDeklaration kdd)
            {
                if (parentCall.ParentUsesSet.GetIndex is null) return false;

                return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no support type definition");
            }

            IParent? dek = this.GetKlassenFound(kdd, parentCall);
            if (dek is null)
            {
                if (parentCall.ParentUsesSet.GetIndex is null) return false;

                return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / variable dek");
            }
            this.Deklaration = dek;

            this.ClassGenericDefinition = kdd.GenericDeklaration;
            this.GenericDeklaration = vd.GenericDeklaration;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private IParent? GetKlassenFound(IndexKlassenDeklaration? kd, IndexVariabelnReference parentCall)
        {
            if (kd is null) return null;
            kd = this.GenericClass(kd, parentCall);

            IndexMethodDeklaration? md = kd.DeCtors.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }

            IMethode? imd = kd.Methods.FirstOrDefault(t=>t.Name == this.Name);
            if (imd != null) { this.MethodenDeklaration(imd, kd.Methods); imd.References.Add(this); return imd; }

            md = kd.Operators.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }

            IndexPropertyDeklaration? pd = kd.IndexProperties.FirstOrDefault(t=>t.Name == this.Name);
            if (pd != null) if (pd.Zusatz != MethodeType.Static) { pd.References.Add(this); return pd; }

            return null;
        }

        private IndexKlassenDeklaration GenericClass(IndexKlassenDeklaration kd, IndexVariabelnReference parentCall)
        {
            if (parentCall.RefCombination == null) return kd;
            if (parentCall.RefCombination.GenericDeklaration == null) return kd;
            if (parentCall.RefCombination.ClassGenericDefinition == null) return kd;

            int index = parentCall.RefCombination.ClassGenericDefinition.GenericIdentifiers.FindIndex(t=>t.Text == kd.Name);
            if (index == -1) return kd;

            IndexVariabelnReference genRef = parentCall.RefCombination.GenericDeklaration.References[index];
            if (genRef.Deklaration is not IndexKlassenDeklaration kdg) return kd;

            return kdg;
        }

        private IParent? GetStaticFound(IndexKlassenDeklaration kd)
        {
            if (kd is null) return null;

            IMethode? md = kd.StaticMethods.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { this.MethodenDeklaration(md, kd.StaticMethods); md.References.Add(this); return md; }

            md = kd.Ctors.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }

            md = kd.Operators.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }

            IndexPropertyDeklaration? pd = kd.IndexStaticProperties.FirstOrDefault(t=>t.Name == this.Name);
            if (pd == null) return null;

            pd.References.Add(this);

            return pd;
        }

        private IParent? GetEnumFound(IndexEnumDeklaration kd)
        {
            if (kd is null) return null;

            IndexEnumEntryDeklaration? md = kd.Entries.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }

            return null;
        }

        public bool PreMappen(ValidUses uses)
        {
            return true;
        }

        public bool Mappen(ValidUses uses)
        {
            if ( this.IsMapped ) return true;
            if ( this.ChildUse != null ) return this.Mappen ( this.ChildUse );
            if (uses.GetIndex is null) return false;

            this.Owner = uses.GetIndex.CurrentMethode;

            this.ParentUsesSet = uses;

            this.Deklaration = this.ParentUsesSet.Deklarationen.FirstOrDefault(t=>t.Name == this.Name);

            if (this.Deklaration == null) return uses.GetIndex.CreateError(this.Use, string.Format("no defintion in index found {0}", this.Name));

            if (this.Deklaration is IndexVariabelnDeklaration vd) this.VariableDeklarationMappen(vd);
            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            foreach (IndexVariabelnReference t in this.VariabelnReferences)
            {
                t.Owner = uses.GetIndex.CurrentMethode;
                if (t.Deklaration == null) t.Deklaration = this.Deklaration.ThisUses.Deklarationen.FirstOrDefault(u=>u.Name == t.Name);
                if (t.Deklaration == null) return uses.GetIndex.CreateError(t.Use, string.Format("no defintion in index found {0}", t.Name));

                if (t.Deklaration is IndexVektorDeklaration ivdu) { ivdu.References.Add(this); continue; }
                if (t.Deklaration is IndexPropertyGetSetDeklaration pgsdu) { pgsdu.References.Add(this); continue; }
                if (t.Deklaration is IndexMethodDeklaration mdu) { mdu.References.Add(this); continue; }
            }

            if (this.Deklaration is IndexKlassenDeklaration kd) return kd.AddReference(this);
            if (this.Deklaration is IndexMethodDeklaration md) { md.References.Add(this); return true; }
            if (this.Deklaration is IndexPropertyDeklaration pd) { pd.References.Add(this); return true; }
            if (this.Deklaration is IndexVariabelnDeklaration) return true;
            if (this.Deklaration is IndexEnumDeklaration ed) { ed.References.Add(this); return true; }
            if (this.Deklaration is IndexEnumEntryDeklaration eed) { eed.References.Add(this); return true; }
            if (this.Deklaration is IndexVektorDeklaration ivd) { ivd.References.Add(this); return true; }
            if (this.Deklaration is IndexPropertyGetSetDeklaration pgsd) { pgsd.References.Add(this); return true; }
            if (this.Deklaration is IndexDelegateDeklaration idd) return this.CreateDelegateDeklaration(idd);

            return uses.GetIndex.CreateError(this.Use, "no support type definition");
        }

        private bool CreateDelegateDeklaration(IndexDelegateDeklaration idd)
        {
            IndexDelegateDeklaration newDelegate = new IndexDelegateDeklaration(idd.Name, idd.ThisUses);
            newDelegate.GenericDeklaration = this.GenericDeklaration;
            this.Deklaration = newDelegate;
            newDelegate.References.Add(this);

            return true;
        }

        private bool VariableDeklarationMappen(IndexVariabelnDeklaration vd)
        {
            if (vd.Type.Deklaration is IndexDelegateDeklaration idd)
            {
                this.GenericDeklaration = idd.GenericDeklaration;
                this.ClassGenericDefinition = idd.GenericDeklaration;

                vd.References.Add(this);

                return true;
            }

            if (vd.Type.Deklaration is not IndexKlassenDeklaration kdd)
            {
                if (this.ParentUsesSet.GetIndex is null) return false;

                 return this.ParentUsesSet.GetIndex.CreateError(this.Use, "no support type definition");
            }

            this.GenericDeklaration = vd.GenericDeklaration;
            this.ClassGenericDefinition = kdd.GenericDeklaration;

            vd.References.Add(this);

            return true;
        }

        private bool MethodenDeklaration(IMethode mud, List<IMethode> deklarationen)
        {
            List<IMethode> parents = deklarationen.Where(t=>t.Name == mud.Name).ToList();
            if (parents.Count <= 1) return true;

            this.OverloadMethods = parents;

            return true;
        }

        /*private bool MethodenDeklaration(IndexMethodDeklaration mud, List<IParent> deklarationen)
        {
            List<IParent> parents = deklarationen.Where(t=>t.Name == mud.Name).ToList();
            if (parents.Count <= 1) return true;



            return true;
        }*/

        #endregion methods

    }
}