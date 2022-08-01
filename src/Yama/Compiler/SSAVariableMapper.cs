using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{
    public class SSAVariableMap
    {
        #region get/set

        public string Key
        {
            get;
            set;
        }

        public List<SSACompileLine> LoopBranchReferencesForPhis
        {
            get;
        } = new List<SSACompileLine>();

        public List<SSACompileLine> AllSets
        {
            get;
            set;
        } = new List<SSACompileLine>();

        //Die Letzte Reference, bei dem die Variable gesetzt wurde
        public SSACompileLine? Reference
        {
            get;
            set;
        }

        public bool IsNullable
        {
            get
            {
                return this.Deklaration.IsNullable;
            }
        }

        public LastValue Value
        {
            get;
            set;
        }

        public List<IndexVariabelnReference> Calls
        {
            get
            {
                return this.Deklaration.References;
            }
        }

        public bool IsUsed
        {
            get
            {
                foreach (ICompileRoot root in this.Calls)
                {
                    if (root.IsUsed) return true;
                }

                return false;
            }
        }

        public IndexVariabelnDeklaration Deklaration
        {
            get;
            set;
        }

        public bool PhiOnlyValueChecking
        {
            get;
            set;
        }

        public VariableType Kind
        {
            get;
            set;
        }

        public VariableMutableState MutableState
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public SSAVariableMap(IndexVariabelnDeklaration dek)
        {
            this.Key = dek.Name;
            this.Deklaration = dek;
            this.Value = LastValue.NotSet;
            this.Kind = this.GetVariableKind(dek);
            if (!dek.IsMutable) this.MutableState = VariableMutableState.NotMutable;
        }

        public SSAVariableMap(SSAVariableMap value)
        {
            //this.AllSets.AddRange(value.AllSets);
            this.Key = value.Key;
            this.Reference = value.Reference;
            this.Deklaration = value.Deklaration;
            this.Value = value.Value;
            this.Kind = value.Kind;
            this.MutableState = value.MutableState;
        }

        #endregion ctor

        #region methods

        private VariableType GetVariableKind(IndexVariabelnDeklaration dek)
        {
            if (!dek.IsNullable) return VariableType.Primitive;
            if (dek.IsBorrowing) return VariableType.BorrowingReference;

            return VariableType.OwnerReference;
        }

        #endregion methods

        public enum LastValue
        {

            NotSet,
            Const,
            Unknown,
            Null,
            NotNull

        }

        public enum VariableType
        {
            Primitive, //ByValue
            OwnerReference, //ByReference
            BorrowingReference //ByReference
        }

        public enum VariableMutableState
        {
            Mutable,
            NotMutable
        }

    }

}