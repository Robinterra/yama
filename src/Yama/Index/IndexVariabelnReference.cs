using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexVariabelnReference : IIndexReference, IParent
    {

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

                return this.Deklaration.Name;
            }
        }

        public IndexVariabelnReference (  )
        {
            this.VariabelnReferences = new List<IndexVariabelnReference>();
        }

        public bool Mappen(IndexVariabelnReference parentCall)
        {
            this.ParentUsesSet = parentCall.ParentUsesSet;

            if (parentCall.Deklaration is IndexVariabelnDeklaration vd)
            {
                if (!(vd.Type.Deklaration is IndexKlassenDeklaration kdd)) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no support type definition");

                IParent dek = this.GetKlassenFound(kdd);

                if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found");
                this.Deklaration = dek;

                if (this.ParentCall != null) this.ParentCall.Mappen(this);

                return true;
            }

            if (parentCall.Deklaration is IndexKlassenDeklaration kd)
            {
                IParent dek = this.GetStaticFound(kd);

                if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found");
                this.Deklaration = dek;

                if (this.ParentCall != null) this.ParentCall.Mappen(this);

                return true;
            }

            if (parentCall.Deklaration is IndexEnumDeklaration ed)
            {
                IParent dek = this.GetEnumFound(ed);

                if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found");
                this.Deklaration = dek;

                if (this.ParentCall != null) this.ParentCall.Mappen(this);

                return true;
            }

            if (parentCall.Deklaration is IndexPropertyDeklaration pd)
            {
                IParent dek = null;
                //if (parentCall.Name == parentCall.Deklaration.Name) dek = this.GetStaticFound(pd.Type.Deklaration as IndexKlassenDeklaration);
                //else 

                dek = this.GetKlassenFound(pd.Type.Deklaration as IndexKlassenDeklaration);

                if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found");
                this.Deklaration = dek;

                if (this.ParentCall != null) this.ParentCall.Mappen(this);

                return true;
            }

            /*if (parentCall.Deklaration is IndexMethodDeklaration md)
            {
                IParent dek = null;
                if (parentCall.Name == parentCall.Deklaration.Name) dek = this.GetStaticFound(md.Parent);
                else dek = this.GetKlassenFound(md.Parent);

                if (dek == null) return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found");
                this.Deklaration = dek;

                if (this.ParentCall != null) this.ParentCall.Mappen(this);

                return true;
            }*/

            return parentCall.ParentUsesSet.GetIndex.CreateError(this.Use, "no defintion in index found");
        }

        private IParent GetKlassenFound(IndexKlassenDeklaration kd)
        {
            if (kd == null) return null;

            IndexMethodDeklaration md = kd.DeCtors.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            md = kd.Methods.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            md = kd.Operators.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            IndexVaktorDeklaration vd = kd.VektorDeclaration.FirstOrDefault(t=>t.Name == this.Name);
            if (vd != null) return vd;
            IndexPropertyDeklaration pd = kd.IndexProperties.FirstOrDefault(t=>t.Name == this.Name);
            if (pd == null) return null;
            if (pd.Zusatz != MethodeType.Static) { pd.References.Add(this); return pd; }

            return null;
        }

        private IParent GetStaticFound(IndexKlassenDeklaration kd)
        {
            if (kd == null) return null;

            IndexMethodDeklaration md = kd.StaticMethods.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            md = kd.Ctors.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            md = kd.Operators.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }
            IndexPropertyDeklaration pd = kd.IndexProperties.FirstOrDefault(t=>t.Name == this.Name);
            if (pd == null) return null;

            if (pd.Zusatz == MethodeType.Static) { pd.References.Add(this); return pd; }

            return null;
        }

        private IParent GetEnumFound(IndexEnumDeklaration kd)
        {
            if (kd == null) return null;

            IndexEnumEntryDeklaration md = kd.Entries.FirstOrDefault(t=>t.Name == this.Name);
            if (md != null) { md.References.Add(this); return md; }

            return null;
        }

        public bool Mappen(ValidUses uses)
        {
            this.ParentUsesSet = uses;

            this.Deklaration = this.ParentUsesSet.Deklarationen.FirstOrDefault(t=>t.Name == this.Name);

            if (this.Deklaration == null) return uses.GetIndex.CreateError(this.Use, string.Format("no defintion in index found {0}", this.Name));

            if (this.ParentCall != null) this.ParentCall.Mappen(this);

            if (this.Deklaration is IndexKlassenDeklaration kd) { kd.References.Add(this); return true; }
            if (this.Deklaration is IndexMethodDeklaration md) { md.References.Add(this); return true; }
            if (this.Deklaration is IndexPropertyDeklaration pd) { pd.References.Add(this); return true; }
            if (this.Deklaration is IndexVariabelnDeklaration vd) { vd.References.Add(this); return true; }
            if (this.Deklaration is IndexEnumDeklaration ed) { ed.References.Add(this); return true; }
            if (this.Deklaration is IndexEnumEntryDeklaration eed) { eed.References.Add(this); return true; }
            if (this.Deklaration is IndexVaktorDeklaration ivd) { ivd.References.Add(this); return true; }

            return uses.GetIndex.CreateError(this.Use, "no support type definition");
        }
    }
}