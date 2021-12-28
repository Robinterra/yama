using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexContainer : IParent
    {

        #region get/set

        public List<IndexContainer> Containers
        {
            get;
        }

        public List<IndexVariabelnDeklaration> VariabelnDeklarations
        {
            get;
        }

        public List<IndexMethodReference> MethodReferences
        {
            get;
        }

        public List<IndexVariabelnReference> VariabelnReferences
        {
            get;
        }

        public bool FunktionContainer
        {
            get;
            set;
        }

        private ValidUses? thisUses;

        public ValidUses ThisUses
        {
            get
            {
                if (this.thisUses is not null) return this.thisUses;

                this.thisUses = new ValidUses(this.ParentUsesSet);

                foreach (IndexVariabelnDeklaration dek in this.VariabelnDeklarations)
                {
                    if (this.thisUses.Deklarationen.Exists(t=>t.Name == dek.Name)) this.thisUses.GetIndex?.CreateError(dek.Use, "Die Deklaration kann nicht vorgenommen werden, eine Deklaration mit diesen Namen existiert schon");

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

        #endregion get/set

        #region ctor

        public IndexContainer(IParseTreeNode use, string name)
        {
            this.ParentUsesSet = new();
            this.Use = use;
            this.Name = name;
            this.VariabelnReferences = new List<IndexVariabelnReference>();
            this.VariabelnDeklarations = new List<IndexVariabelnDeklaration>();
            this.MethodReferences = new List<IndexMethodReference>();
            this.Containers = new List<IndexContainer>();
        }

        #endregion ctor

        #region methods

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

            foreach (IndexMethodReference methodReference in this.MethodReferences)
            {
                methodReference.Mappen(this.ThisUses);
            }

            foreach (IndexContainer container in this.Containers)
            {
                container.Mappen(this.ThisUses);
            }

            return true;
        }

        #endregion methods

    }
}