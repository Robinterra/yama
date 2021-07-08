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

        public Index Index
        {
            get;
            set;
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

            if (functionRef.OverloadMethods != null) return this.OverrideMethodsDeklaration(functionRef, imd);

            this.Deklaration = imd;

            return true;
        }

        private bool OverrideMethodsDeklaration(IndexVariabelnReference functionRef, IndexMethodDeklaration firstDek)
        {
            foreach (IMethode methode in functionRef.OverloadMethods)
            {
                if (!(methode is IndexMethodDeklaration imd)) continue;

                this.Deklaration = imd;
                if (imd.Parameters.Count != this.ParametersCount) continue;
                if (!this.OverrideMethodTypeCheck()) continue;

                firstDek.References.Remove(functionRef);
                imd.References.Add(functionRef);
                functionRef.Deklaration = imd;

                return true;
            }

            return this.Index.CreateError(functionRef.Use, "for this function exist multible overloadings and no one is the chosen one");
        }

        private bool OverrideMethodTypeCheck()
        {
            int count = 0;
            for (int i = 0; i < this.Deklaration.Parameters.Count; i++)
            {
                IndexVariabelnDeklaration dek = this.Deklaration.Parameters[i];
                if (dek.Name == "this") continue;

                if (this.Index.GetTypeName(this.Parameters[count]) != dek.Type.Name) return false;

                count = count + 1;
            }

            return true;
        }

        private IndexVariabelnReference GetParentCall(IndexVariabelnReference reference)
        {
            if (reference.ParentCall == null) return reference;

            return this.GetParentCall(reference.ParentCall);
        }

        public bool Mappen(ValidUses thisUses)
        {
            this.Index = thisUses.GetIndex;
            if (!this.FindDeklaration()) return thisUses.GetIndex.CreateError(this.Use, "methoden declaretion can not be found");

            if (this.Deklaration.Type == MethodeType.Operator) return true;

            int expectedParaCount = this.Deklaration.Parameters.Count;
            if ( this.Use is NewKey ) expectedParaCount -= 1;
            if ( expectedParaCount != this.ParametersCount) return thisUses.GetIndex.CreateError(this.CallRef.ParentCall == null ? this.Use : this.CallRef.ParentCall.Use, "parameter count is not equals with the declaration");

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