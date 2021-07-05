using System;
using System.Collections.Generic;
using System.Linq;
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

        public IParent Deklaration
        {
            get;
            set;
        }

        public IParent Owner
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

        public bool IsMapped
        {
            get;
            set;
        }

        public List<IMethode> OverloadMethods
        {
            get;
            set;
        }

        public bool IsOwnerInUse(int depth)
        {
            if (this.Owner == null) return false;

            return this.Owner.IsInUse(depth);
        }

        public bool IsInUse (int depth)
        {
            if (depth > 10) return true;

            return this.IsOwnerInUse(depth + 1);
        }

        private IndexVariabelnReference parentCall;

        public IndexVariabelnReference ParentCall
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

        public GenericCall GenericDeklaration
        {
            get;
            set;
        }

        public GenericCall ClassGenericDefinition
        {
            get;
            set;
        }

        public IndexVariabelnReference RefCombination
        {
            get;
            set;
        }

        public IndexVariabelnReference ChildUse
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public IndexVariabelnReference (  )
        {
            this.VariabelnReferences = new List<IndexVariabelnReference>();
        }

        #endregion ctor

        #region methods

        public bool Mappen(IndexVariabelnReference parentCall)
        {
            if ( this.ChildUse != null && !parentCall.Equals(this.ChildUse) ) return this.Mappen ( this.ChildUse );
            if ( this.IsMapped ) return true;

            this.IsMapped = true;

            this.ParentUsesSet = parentCall.ParentUsesSet;

            if (parentCall.Deklaration is IndexVariabelnDeklaration vd) return this.VariableDeklarationMappen (parentCall, vd);

            if (parentCall.Deklaration is IndexKlassenDeklaration kd) return this.ClassMappen(kd, parentCall);

            if (parentCall.Deklaration is IndexEnumDeklaration ed) return this.EnumMappen(ed, parentCall);

            if (parentCall.Deklaration is IndexPropertyDeklaration pd) return this.PropertyMappen(pd, parentCall);

            if (parentCall.Deklaration is IndexPropertyGetSetDeklaration pgsd) return this.GetSetMappen(pgsd, parentCall);

            if (parentCall.Deklaration is IndexMethodDeklaration md) return this.MethodMappen(md, parentCall);

            return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / regular");
        }

        private bool MethodMappen(IndexMethodDeklaration md, IndexVariabelnReference parentCall)
        {
            IParent dek = null;

            if (this.IsOperator) dek = this.GetStaticFound((IndexKlassenDeklaration)md.ReturnValue.Deklaration);

            if (dek != null)
            {
                this.Deklaration = dek;

                if (this.ParentCall != null) this.ParentCall.Mappen(this);

                return true;
            }

            if (parentCall.Name == parentCall.Deklaration.Name) dek = this.GetStaticFound(md.Klasse);
            else dek = this.GetKlassenFound(md.Klasse, parentCall);

            if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / method");

            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool GetSetMappen(IndexPropertyGetSetDeklaration pgsd, IndexVariabelnReference parentCall)
        {
            IParent dek = null;
            //if (parentCall.Name == parentCall.Deklaration.Name) dek = this.GetStaticFound(pd.Type.Deklaration as IndexKlassenDeklaration);
            //else 

            dek = this.GetKlassenFound(pgsd.ReturnValue.Deklaration as IndexKlassenDeklaration, parentCall);

            if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / prop dek");

            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool PropertyMappen(IndexPropertyDeklaration pd, IndexVariabelnReference parentCall)
        {
            IParent dek = null;
            //if (parentCall.Name == parentCall.Deklaration.Name) dek = this.GetStaticFound(pd.Type.Deklaration as IndexKlassenDeklaration);
            //else 

            dek = this.GetKlassenFound(pd.Type.Deklaration as IndexKlassenDeklaration, parentCall);

            if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / property dek");
            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool EnumMappen(IndexEnumDeklaration ed, IndexVariabelnReference parentCall)
        {
            IParent dek = this.GetEnumFound(ed);

            if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / enum dek");
            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool ClassMappen(IndexKlassenDeklaration kd, IndexVariabelnReference parentCall)
        {
            IParent dek = this.GetStaticFound(kd);

            if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / call dek");
            this.Deklaration = dek;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private bool VariableDeklarationMappen(IndexVariabelnReference parentCall, IndexVariabelnDeklaration vd)
        {
            if (!(vd.Type.Deklaration is IndexKlassenDeklaration kdd)) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no support type definition");

            IParent dek = this.GetKlassenFound(kdd, parentCall);

            if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found / variable dek");
            this.Deklaration = dek;

            this.ClassGenericDefinition = kdd.GenericDeklaration;
            this.GenericDeklaration = vd.GenericDeklaration;

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            return true;
        }

        private IParent GetKlassenFound(IndexKlassenDeklaration kd, IndexVariabelnReference parentCall)
        {
            if (kd == null) return null;
            kd = this.GenericClass(kd, parentCall);

            IndexMethodDeklaration md = kd.DeCtors.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            IMethode imd = kd.Methods.FirstOrDefault(t=>t.Name == this.Name);
            if (imd != null) { this.MethodenDeklaration(imd, kd.Methods); imd.References.Add(this); return imd; }
            md = kd.Operators.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            IndexPropertyDeklaration pd = kd.IndexProperties.FirstOrDefault(t=>t.Name == this.Name);
            if (pd != null) if (pd.Zusatz != MethodeType.Static) { pd.References.Add(this); return pd; }

            return null;
        }

        private IndexKlassenDeklaration GenericClass(IndexKlassenDeklaration kd, IndexVariabelnReference parentCall)
        {
            if (parentCall.RefCombination == null) return kd;
            if (parentCall.RefCombination.GenericDeklaration == null) return kd;
            if (parentCall.RefCombination.ClassGenericDefinition == null) return kd;

            if (parentCall.RefCombination.ClassGenericDefinition.Token.Text != kd.Name) return kd;

            if (!(parentCall.RefCombination.GenericDeklaration.Reference.Deklaration is IndexKlassenDeklaration kdg)) return kd;

            return kdg;
        }

        private IParent GetStaticFound(IndexKlassenDeklaration kd)
        {
            if (kd == null) return null;

            IMethode md = kd.StaticMethods.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { this.MethodenDeklaration(md, kd.StaticMethods); md.References.Add(this); return md; }
            md = kd.Ctors.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            md = kd.Operators.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            IndexPropertyDeklaration pd = kd.IndexStaticProperties.FirstOrDefault(t=>t.Name == this.Name);
            if (pd == null) return null;

            pd.References.Add(this);

            return pd;
        }

        private IParent GetEnumFound(IndexEnumDeklaration kd)
        {
            if (kd == null) return null;

            IndexEnumEntryDeklaration md = kd.Entries.FirstOrDefault(t=>t.Name == this.Name);
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

            if (this.Deklaration is IndexKlassenDeklaration kd) { kd.References.Add(this); return true; }
            if (this.Deklaration is IndexMethodDeklaration md) { md.References.Add(this); return true; }
            if (this.Deklaration is IndexPropertyDeklaration pd) { pd.References.Add(this); return true; }
            if (this.Deklaration is IndexVariabelnDeklaration) return true;
            if (this.Deklaration is IndexEnumDeklaration ed) { ed.References.Add(this); return true; }
            if (this.Deklaration is IndexEnumEntryDeklaration eed) { eed.References.Add(this); return true; }
            if (this.Deklaration is IndexVektorDeklaration ivd) { ivd.References.Add(this); return true; }
            if (this.Deklaration is IndexPropertyGetSetDeklaration pgsd) { pgsd.References.Add(this); return true; }

            return uses.GetIndex.CreateError(this.Use, "no support type definition");
        }

        private bool VariableDeklarationMappen(IndexVariabelnDeklaration vd)
        {
            if (!(vd.Type.Deklaration is IndexKlassenDeklaration kdd)) return this.ParentUsesSet.GetIndex.CreateError(this.Use, "no support type definition");

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