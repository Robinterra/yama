using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexVariabelnDeklaration : IParent
    {
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

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public IndexVariabelnReference Type
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

        public ValidUses ParentUsesSet { get; set; }
        public ValidUses BaseUsesSet { get; internal set; }

        public ValidUses SetUsesSet { get; internal set; }

        public IndexVariabelnDeklaration (  )
        {
            this.References = new List<IndexVariabelnReference>();
            
        }

        public bool Mappen(ValidUses uses)
        {
            this.ParentUsesSet = uses;

            if (this.Name == "this")
            {
                IParent parent = this.ParentUsesSet.Deklarationen.FirstOrDefault(t=>t.Name == "this");

                if (!(parent is IndexVariabelnDeklaration dek)) return false;

                this.Type = dek.Type;
                this.Use = dek.Use;

                return true;
            }

            if (this.Name == "base")
            {
                IParent parent = this.BaseUsesSet.Deklarationen.FirstOrDefault(t=>t.Name == "base");

                if (!(parent is IndexVariabelnDeklaration dek)) return false;

                this.Type = dek.Type;
                this.Use = dek.Use;

                return true;
            }

            if (this.Name == "invalue")
            {
                this.SetUsesSet = uses;

                IParent parent = this.SetUsesSet.Deklarationen.FirstOrDefault(t=>t.Name == "invalue");

                if (!(parent is IndexVariabelnDeklaration dek)) return false;

                this.Type = dek.Type;
                this.Use = dek.Use;

                return true;
            }

            this.Type.Mappen(uses);

            return true;
        }

        
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
    }
}