using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
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
            private set;
        }

        public List<IndexMethodDeklaration> Operators
        {
            get;
            private set;
        }

        public List<IndexMethodDeklaration> Ctors
        {
            get;
            private set;
        }

        public List<IndexPropertyDeklaration> IndexProperties
        {
            get;
            private set;
        }

        public List<IndexPropertyDeklaration> IndexStaticProperties
        {
            get;
            private set;
        }

        public List<IndexMethodDeklaration> DeCtors
        {
            get;
            private set;
        }

        public List<IMethode> Methods
        {
            get;
            private set;
        }

        public List<IMethode> StaticMethods
        {
            get;
            private set;
        }

        //public List<IndexVektorDeklaration> VektorDeclaration { get; set; }

        private ValidUses? thisUses;

        public ValidUses ThisUses
        {
            get
            {
                if (this.thisUses != null) return this.thisUses;

                this.thisUses = new ValidUses(this.ParentUsesSet);

                IndexVariabelnReference varref = new IndexVariabelnReference (this.Use, this.Name) { Deklaration = this };
                IndexVariabelnDeklaration dekThisVar = new IndexVariabelnDeklaration(this.Use, "this", varref);
                dekThisVar.ParentUsesSet = this.thisUses;

                this.References.Add(dekThisVar.Type);

                List<IParent> dekList = new List<IParent> { dekThisVar };

                this.CreateGenericForThisUses(dekList);

                this.thisUses.Deklarationen = dekList;

                if (this.InheritanceBase == null) return this.thisUses;
                if (this.InheritanceBase.Deklaration is not IndexKlassenDeklaration dek) return this.thisUses;

                varref = new IndexVariabelnReference (dek.Use, dek.Name) { Deklaration = dek };
                IndexVariabelnDeklaration dekbaseVar = new IndexVariabelnDeklaration(this.Use, "base", varref);
                dekbaseVar.ParentUsesSet = dek.ThisUses;
                dekbaseVar.BaseUsesSet = this.thisUses;
                dekList.Add(dekbaseVar);
                this.BaseVar = dekbaseVar;

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }

        public bool IsGeneric
        {
            get;
            set;
        }

        public bool IsMethodsReferenceMode
        {
            get
            {
                if (this.InheritanceChilds.Count != 0) return true;

                return this.InheritanceBase != null;
            }
        }

        public int GetNonStaticPropCount
        {
            get
            {
                int count = 0;

                if (this.IsMethodsReferenceMode) count += 1;

                foreach (IndexPropertyDeklaration dek in this.IndexProperties)
                {
                    if (dek.Zusatz == MethodeType.Property) count++;
                }

                return count;
            }
        }

        public IndexVariabelnReference? InheritanceBase
        {
            get;
            set;
        }

        public List<IndexKlassenDeklaration> InheritanceChilds
        {
            get;
        }

        public bool IsMapped
        {
            get;
            set;
        }

        public IndexVariabelnDeklaration? BaseVar
        {
            get;
            set;
        }

        public CompileData? DataRef
        {
            get;
            set;
        }

        public CompileData? ReflectionData
        {
            get;
            set;
        }

        public GenericCall? GenericDeklaration
        {
            get;
            set;
        }

        public ClassMemberModifiers MemberModifier
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public IndexKlassenDeklaration ( IParseTreeNode use, string name )
        {
            this.Use = use;
            this.Name = name;
            this.ParentUsesSet = new();
            this.References = new List<IndexVariabelnReference>();
            this.Ctors = new List<IndexMethodDeklaration>();
            this.DeCtors = new List<IndexMethodDeklaration>();
            this.Methods = new List<IMethode>();
            this.Operators = new List<IndexMethodDeklaration>();
            this.IndexProperties = new List<IndexPropertyDeklaration>();
            this.StaticMethods = new List<IMethode>();
            this.IndexStaticProperties = new List<IndexPropertyDeklaration>();
            this.InheritanceChilds = new List<IndexKlassenDeklaration>();
        }

        #endregion ctor

        #region methods

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

        private bool CreateGenericForThisUses(List<IParent> dekList)
        {
            if (this.GenericDeklaration == null) return false;
            if (this.thisUses is null) return false;//TODO: fehler erfassen

            IndexKlassenDeklaration dekThisVar = new (this.GenericDeklaration, this.GenericDeklaration.Token.Text);
            dekThisVar.ParentUsesSet = this.thisUses;
            dekThisVar.IsGeneric = true;
            dekThisVar.MemberModifier = ClassMemberModifiers.Primitive;

            dekList.Add(dekThisVar);

            return true;
        }

        public bool PreMappen(ValidUses uses)
        {
            this.ParentUsesSet = uses;

            if (this.InheritanceBase == null) return true;

            this.InheritanceBase.Mappen(this.ParentUsesSet);

            if (!(this.InheritanceBase.Deklaration is IndexKlassenDeklaration dek)) return true;

            dek.InheritanceChilds.Add(this);

            //this.Methods.AddRange(dek.Methods.Where(t=>!this.Methods.Any(q=>q.Name == t.Name)));
            this.StaticMethods.AddRange(dek.StaticMethods.Where(t=>!this.StaticMethods.Any(q=>q.Name == t.Name)));
            this.Operators.AddRange(dek.Operators.Where(t=>!this.Operators.Any(q=>q.Name == t.Name)));
            this.Ctors.AddRange(dek.Ctors.Where(t=>this.Ctors.Count == 0));
            //this.DeCtors.AddRange(dek.DeCtors.Where(t=>this.DeCtors.Count == 0));

            List<IndexPropertyDeklaration> deks = dek.IndexProperties.Where(t=>true).ToList();

            deks.AddRange(this.IndexProperties.Where(t=>!dek.IndexProperties.Any(q=>q.Name == t.Name)));

            List<IMethode> sortMethods = new List<IMethode>();
            foreach (IMethode met in dek.Methods)
            {
                IMethode? setmet = this.Methods.FirstOrDefault(q=>q.KeyName == met.KeyName);
                if (setmet is null) setmet = met;

                sortMethods.Add(setmet);
            }

            sortMethods.AddRange(this.Methods.Where(t=>!sortMethods.Any(q=>q.KeyName == t.KeyName)));

            this.IndexProperties = deks;
            this.Methods = sortMethods;

            return true;
        }

        public bool PreviusMappen(ValidUses rootValidUses)
        {
            this.ParentUsesSet = rootValidUses;

            this.PreviusMethodeMappen(this.Methods, this.ThisUses);
            this.PreviusMethodeMappen(this.StaticMethods, this.ParentUsesSet);
            this.PreviusMethodeMappen(this.Operators, this.ParentUsesSet);
            this.PreviusMethodeMappen(this.Ctors, this.ThisUses);
            //this.PreviusMethodeMappen(this.DeCtors, this.ThisUses);
            this.PreviusPropertyMappen(this.IndexProperties, this.ThisUses);
            this.PreviusPropertyMappen(this.IndexStaticProperties, this.ParentUsesSet);

            return true;
        }

        public bool Mappen()
        {
            if (this.IsMapped) return true;
            if (this.InheritanceBase != null)
            {
                if (this.InheritanceBase.Deklaration == null)
                {
                    if (this.ThisUses.GetIndex is null) return false;
                    return this.ThisUses.GetIndex.CreateError(this.Use, string.Format("Inheritance '{0}' Base Class is not found", this.InheritanceBase.Name));
                }
                if (!((IndexKlassenDeklaration)this.InheritanceBase.Deklaration).IsMapped) return false;
            }

            //this.PreviusMappen();

            this.MethodeMappen(this.Methods, this.ThisUses);
            this.MethodeMappen(this.StaticMethods, this.ParentUsesSet);
            this.MethodeMappen(this.Operators, this.ParentUsesSet);
            this.MethodeMappen(this.Ctors, this.ThisUses);
            //this.MethodeMappen(this.DeCtors, this.ThisUses);
            this.PropertyMappen(this.IndexProperties, this.ThisUses);
            this.PropertyMappen(this.IndexStaticProperties, this.ParentUsesSet);

            return this.IsMapped = true;
        }

        private bool PropertyMappen(List<IndexPropertyDeklaration> indexProperties, ValidUses thisUses)
        {
            foreach (IndexPropertyDeklaration deklaration in indexProperties)
            {
                if (deklaration.IsMapped) continue;

                deklaration.Klasse = this;

                deklaration.Mappen();
            }

            return true;
        }

        private bool PreviusPropertyMappen(List<IndexPropertyDeklaration> indexProperties, ValidUses thisUses)
        {
            foreach (IndexPropertyDeklaration deklaration in indexProperties)
            {
                if (deklaration.IsMapped) continue;

                deklaration.Klasse = this;

                deklaration.PreMappen(thisUses);
            }

            return true;
        }

        private bool PreviusMethodeMappen(List<IndexMethodDeklaration> methods, ValidUses uses)
        {
            foreach (IndexMethodDeklaration deklaration in methods)
            {
                if (deklaration.IsMapped) continue;

                deklaration.Klasse = this;

                deklaration.PreMappen(uses);
            }

            return true;
        }

        private bool PreviusMethodeMappen(List<IMethode> methods, ValidUses uses)
        {
            foreach (IMethode deklaration in methods)
            {
                if (deklaration.IsMapped) continue;

                deklaration.Klasse = this;

                deklaration.PreMappen(uses);
            }

            return true;
        }

        private bool MethodeMappen(List<IndexMethodDeklaration> methods, ValidUses uses)
        {
            foreach (IndexMethodDeklaration deklaration in methods)
            {
                if (deklaration.IsMapped) continue;

                deklaration.Klasse = this;

                deklaration.Mappen();
            }

            return true;
        }

        private bool MethodeMappen(List<IMethode> methods, ValidUses uses)
        {
            foreach (IMethode deklaration in methods)
            {
                if (deklaration.IsMapped) continue;

                deklaration.Klasse = this;

                deklaration.Mappen();
            }

            return true;
        }

        public bool AddReference(IndexVariabelnReference indexVariabelnReference)
        {
            this.References.Add(indexVariabelnReference);
            if (indexVariabelnReference.Use is not TypeofKey tk) return true;

            foreach (IndexMethodDeklaration imd in this.Ctors)
            {
                imd.References.Add(indexVariabelnReference);
            }

            return true;
        }

        #endregion methods
    }

    public enum ClassMemberModifiers
    {

        None,
        Static,
        Primitive,
        Struct

    }
}