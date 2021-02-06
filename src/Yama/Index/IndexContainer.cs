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

                this.thisUses = this.ParentUsesSet;

                foreach (IndexVariabelnDeklaration dek in this.VariabelnDeklarations)
                {
                    if (this.thisUses.Deklarationen.Exists(t=>t.Name == dek.Name)) this.thisUses.GetIndex.CreateError(dek.Use, "Die Deklaration kann nicht vorgenommen werden, eine Deklaration mit diesen Namen existiert schon");

                    this.thisUses.Add(dek);
                }

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }
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

        public bool IsMapped
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

        
        public bool IsInUse (int depth)
        {
            return true;
        }

        public bool PreMappen(ValidUses uses)
        {
            return true;
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