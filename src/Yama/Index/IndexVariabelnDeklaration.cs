using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexVariabelnDeklaration : IParent
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Name
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        // -----------------------------------------------

        public IParseTreeNode Use
        {
            get;
            set;
        }

        // -----------------------------------------------

        public IndexVariabelnReference Type
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ValidUses ThisUses
        {
            get
            {
                return this.ParentUsesSet;
            }
        }

        // -----------------------------------------------

        public bool IsMapped
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ValidUses BaseUsesSet
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ValidUses SetUsesSet
        {
            get;
            set;
        }

        // -----------------------------------------------

        public SSAVariableMap SSAMap
        {
            get;
            set;
        }

        // -----------------------------------------------

        public GenericCall GenericDeklaration
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public IndexVariabelnDeklaration (  )
        {
            this.References = new List<IndexVariabelnReference>();
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool PreMappen(ValidUses uses)
        {
            return true;
        }

        // -----------------------------------------------

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

        // -----------------------------------------------

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

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}