using System;
using System.Collections.Generic;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexMethodReference : IIndexReference
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

        public IndexMethodDeklaration Deklaration
        {
            get;
            set;
        }

        public List<IndexVariabelnReference> Parameters
        {
            get;
            set;
        }

        public IndexVariabelnReference CallRef
        {
            get;
            set;
        }

        public int ParametersCount
        {
            get
            {
                int result = this.Parameters.Count;

                if (this.Deklaration.Type == MethodeType.Methode) result += 1;

                return result;
            }
        }

        #endregion get/set

        #region ctor

        public IndexMethodReference (  )
        {
            this.Parameters = new List<IndexVariabelnReference> (  );
        }

        #endregion ctor

        #region methods

        private bool FindDeklaration (  )
        {
            if (this.CallRef == null) return false;
            //if (this.CallRef.ParentCall == null) return false;

            IndexVariabelnReference functionRef = this.GetParentCall(this.CallRef);
            if (functionRef == null) return false;
            if (!(functionRef.Deklaration is IndexMethodDeklaration imd)) return false;

            this.Deklaration = imd;

            return true;
        }

        private IndexVariabelnReference GetParentCall(IndexVariabelnReference reference)
        {
            if (reference.ParentCall == null) return reference;

            return this.GetParentCall(reference.ParentCall);
        }

        public bool Mappen(ValidUses thisUses)
        {
            if (!this.FindDeklaration()) return thisUses.GetIndex.CreateError(this.Use, "methoden declaretion can not be found");

            if (this.Deklaration.Type == MethodeType.Operator) return true;

            if (this.Deklaration.Parameters.Count != this.ParametersCount)
                return thisUses.GetIndex.CreateError(this.CallRef.ParentCall.Use, "parameter count is not equals with the declaration");

            int count = 0;
            for (int i = 0; i < this.Deklaration.Parameters.Count; i++)
            {
                IndexVariabelnDeklaration dek = this.Deklaration.Parameters[i];
                if (dek.Name == "this") continue;

                thisUses.GetIndex.IndexTypeSafeties.Add(new IndexParameterType(dek, this.Parameters[count], count, this.Deklaration, this.CallRef));

                count = count + 1;
            }

            return true;
        }

        #endregion methods
    }
}