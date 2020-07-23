using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexContainer : IParent
    {

        public List<IndexContainer> Containers
        {
            get;
            set;
        }

        public List<IndexVariabelnDeklaration> VariabelnDeklarations
        {
            get;
            set;
        }

        public List<IndexMethodReference> MethodReferences
        {
            get;
            set;
        }

        public List<IndexVariabelnReference> VariabelnReferences
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
                this.thisUses.Deklarationen = this.VariabelnDeklarations.OfType<IParent>().ToList<IParent>();

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }
        public string Name
        {
            get;
            set;
        }
        public IParseTreeNode Use
        {
            get;
            set;
        }

        public IndexContainer()
        {
            this.VariabelnReferences = new List<IndexVariabelnReference>();
            this.VariabelnDeklarations = new List<IndexVariabelnDeklaration>();
            this.MethodReferences = new List<IndexMethodReference>();
            this.Containers = new List<IndexContainer>();
        }

        public bool Mappen(ValidUses thisUses)
        {
            this.ParentUsesSet = thisUses;

            foreach (IndexVariabelnDeklaration deklaration in this.VariabelnDeklarations)
            {
                deklaration.Mappen(this.ThisUses);
            }

            foreach (IndexVariabelnReference reference in this.VariabelnReferences)
            {
                reference.Mappen(this.ThisUses);
            }

            foreach (IndexContainer container in this.Containers)
            {
                container.Mappen(this.ThisUses);
            }

            return true;
        }
    }
}