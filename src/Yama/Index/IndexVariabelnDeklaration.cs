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

            this.Type.Mappen(uses);

            return true;
        }
    }
}