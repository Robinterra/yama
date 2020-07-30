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

        public List<IndexPropertyDeklaration> IndexProperties
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

                /*List<IParent> parents = new List<IParent>();
                parents.AddRange(this.Methods.OfType<IParent>());
                parents.AddRange(this.Operators.OfType<IParent>());
                parents.AddRange(this.Ctors.OfType<IParent>());
                parents.AddRange(this.DeCtors.OfType<IParent>());
                parents.AddRange(this.IndexProperties.OfType<IParent>());*/

                this.thisUses.Deklarationen = new List<IParent> { dekThisVar };

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }

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
            this.ParentUsesSet = rootValidUses;

            this.PreviusMappen();

            this.MethodeMappen(this.Methods, this.ThisUses);
            this.MethodeMappen(this.StaticMethods, this.ParentUsesSet);
            this.MethodeMappen(this.Operators, this.ParentUsesSet);
            this.MethodeMappen(this.Ctors, this.ThisUses);
            this.MethodeMappen(this.DeCtors, this.ThisUses);

            this.PropertyMappen(this.IndexProperties, this.ThisUses);

            return true;
        }

        private bool PropertyMappen(List<IndexPropertyDeklaration> indexProperties, ValidUses thisUses)
        {
            foreach (IndexPropertyDeklaration deklaration in indexProperties)
            {
                deklaration.Mappen();
            }

            return true;
        }

        private bool PreviusPropertyMappen(List<IndexPropertyDeklaration> indexProperties, ValidUses thisUses)
        {
            foreach (IndexPropertyDeklaration deklaration in indexProperties)
            {
                deklaration.PreMappen(thisUses);
            }

            return true;
        }

        private bool PreviusMethodeMappen(List<IndexMethodDeklaration> methods, ValidUses uses)
        {
            foreach (IndexMethodDeklaration deklaration in methods)
            {
                deklaration.PreMappen(uses);
            }

            return true;
        }

        private bool MethodeMappen(List<IndexMethodDeklaration> methods, ValidUses uses)
        {
            foreach (IndexMethodDeklaration deklaration in methods)
            {
                deklaration.Mappen();
            }

            return true;
        }

        #endregion ctor
    }
}