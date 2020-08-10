using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexKlassenDeklaration : IParent
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

        public List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> Operators
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> Ctors
        {
            get;
            set;
        }

        public List<IndexPropertyDeklaration> IndexProperties
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> DeCtors
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> Methods
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> StaticMethods
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

                IndexVariabelnDeklaration dekThisVar = new IndexVariabelnDeklaration();
                dekThisVar.Name = "this";
                dekThisVar.Type = new IndexVariabelnReference { Deklaration = this, Name = this.Name, Use = this.Use };
                dekThisVar.Use = this.Use;
                dekThisVar.ParentUsesSet = this.thisUses;

                this.References.Add(dekThisVar.Type);

                List<IParent> dekList = new List<IParent> { dekThisVar };

                this.thisUses.Deklarationen = dekList;

                if (this.InheritanceBase == null) return this.thisUses;
                if (!(this.InheritanceBase.Deklaration is IndexKlassenDeklaration dek)) return this.thisUses;

                IndexVariabelnDeklaration dekbaseVar = new IndexVariabelnDeklaration();
                dekbaseVar.Name = "base";
                dekbaseVar.Type = new IndexVariabelnReference { Deklaration = dek, Name = dek.Name, Use = dek.Use };
                dekbaseVar.Use = this.Use;
                dekbaseVar.ParentUsesSet = dek.ThisUses;
                dekbaseVar.BaseUsesSet = this.thisUses;
                dekList.Add(dekbaseVar);
                this.BaseVar = dekbaseVar;

                this.Methods.AddRange(dek.Methods.Where(t=>!this.Methods.Any(q=>q.Name == t.Name)));
                this.Operators.AddRange(dek.Operators.Where(t=>!this.Operators.Any(q=>q.Name == t.Name)));
                this.Ctors.AddRange(dek.Ctors.Where(t=>this.Ctors.Count == 0));
                this.DeCtors.AddRange(dek.DeCtors.Where(t=>this.DeCtors.Count == 0));

                List<IndexPropertyDeklaration> deks = dek.IndexProperties.Where(t=>true).ToList();

                deks.AddRange(this.IndexProperties.Where(t=>!dek.IndexProperties.Any(q=>q.Name == t.Name)));

                this.IndexProperties = deks;

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }
        public int GetNonStaticPropCount
        {
            get
            {
                int count = 0;

                foreach (IndexPropertyDeklaration dek in this.IndexProperties)
                {
                    if (dek.Zusatz == MethodeType.Property) count++;
                }

                return count;
            }
        }

        public IndexVariabelnReference InheritanceBase { get; internal set; }

        private bool IsMapped
        {
            get;
            set;
        }
        public IndexVariabelnDeklaration BaseVar { get; private set; }

        #endregion get/set

        #region ctor

        public IndexKlassenDeklaration (  )
        {
            this.References = new List<IndexVariabelnReference>();
            this.Ctors = new List<IndexMethodDeklaration>();
            this.DeCtors = new List<IndexMethodDeklaration>();
            this.Methods = new List<IndexMethodDeklaration>();
            this.Operators = new List<IndexMethodDeklaration>();
            this.IndexProperties = new List<IndexPropertyDeklaration>();
            this.StaticMethods = new List<IndexMethodDeklaration>();
        }

        #endregion ctor

        #region methods

        public bool PreMappen(ValidUses uses)
        {
            this.ParentUsesSet = uses;

            if (this.InheritanceBase == null) return true;

            this.InheritanceBase.Mappen(this.ParentUsesSet);

            return true;
        }

        private bool PreviusMappen()
        {
            this.PreviusMethodeMappen(this.Methods, this.ThisUses);
            this.PreviusMethodeMappen(this.StaticMethods, this.ParentUsesSet);
            this.PreviusMethodeMappen(this.Operators, this.ParentUsesSet);
            this.PreviusMethodeMappen(this.Ctors, this.ThisUses);
            this.PreviusMethodeMappen(this.DeCtors, this.ThisUses);

            this.PreviusPropertyMappen(this.IndexProperties, this.ThisUses);

            return true;
        }

        public bool Mappen(ValidUses rootValidUses)
        {
            if (this.IsMapped) return true;
            if (this.InheritanceBase != null)
                if (!((IndexKlassenDeklaration)this.InheritanceBase.Deklaration).IsMapped) return false;

            this.ParentUsesSet = rootValidUses;

            this.PreviusMappen();

            this.MethodeMappen(this.Methods, this.ThisUses);
            this.MethodeMappen(this.StaticMethods, this.ParentUsesSet);
            this.MethodeMappen(this.Operators, this.ParentUsesSet);
            this.MethodeMappen(this.Ctors, this.ThisUses);
            this.MethodeMappen(this.DeCtors, this.ThisUses);

            this.PropertyMappen(this.IndexProperties, this.ThisUses);

            return this.IsMapped = true;
        }

        private bool PropertyMappen(List<IndexPropertyDeklaration> indexProperties, ValidUses thisUses)
        {
            foreach (IndexPropertyDeklaration deklaration in indexProperties)
            {
                if (!deklaration.Mappen()) continue;
                deklaration.Klasse = this;
            }

            return true;
        }

        private bool PreviusPropertyMappen(List<IndexPropertyDeklaration> indexProperties, ValidUses thisUses)
        {
            foreach (IndexPropertyDeklaration deklaration in indexProperties)
            {
                if (!deklaration.PreMappen(thisUses)) continue;
                deklaration.Klasse = this;
            }

            return true;
        }

        private bool PreviusMethodeMappen(List<IndexMethodDeklaration> methods, ValidUses uses)
        {
            foreach (IndexMethodDeklaration deklaration in methods)
            {
                if (!deklaration.PreMappen(thisUses)) continue;
                deklaration.Klasse = this;
            }

            return true;
        }

        private bool MethodeMappen(List<IndexMethodDeklaration> methods, ValidUses uses)
        {
            foreach (IndexMethodDeklaration deklaration in methods)
            {
                if (!deklaration.Mappen()) continue;
                deklaration.Klasse = this;
            }

            return true;
        }

        #endregion methods
    }
}