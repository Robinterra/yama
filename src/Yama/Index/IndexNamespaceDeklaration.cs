using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexNamespaceDeklaration : IParent
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


        public List<IndexKlassenDeklaration> KlassenDeklarationen
        {
            get;
            set;
        }

        public List<IndexNamespaceReference> References
        {
            get;
            set;
        }

        public List<IndexNamespaceReference> Usings
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

                //this.References.Add(dekThisVar.Type);

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

        public IndexNamespaceDeklaration (  )
        {
            this.References = new List<IndexNamespaceReference>();
            this.KlassenDeklarationen = new List<IndexKlassenDeklaration>();
            this.Usings = new List<IndexNamespaceReference>();
        }

        private bool PreviusMappen()
        {
            //this.PreviusUsingsMappen(this.Usings, this.ThisUses);

            return true;
        }

        public bool Mappen(ValidUses rootValidUses)
        {
            //this.ParentUsesSet = rootValidUses;

            //this.PreviusMappen();

            //this.KlassenMappen(this.KlassenDeklarationen, this.ThisUses);

            return true;
        }

        #endregion ctor
    }
}