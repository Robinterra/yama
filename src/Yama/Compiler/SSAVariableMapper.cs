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
        private SSACompileLine? reference;
        private LastValue value;

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
            get
            {
                return this.reference;
            }
            set
            {
                if (value is not null)
                {
                    this.First.TryToClean = false;
                    this.TryToClean = false;
                }
                this.reference = value;
            }
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
            get
            {
                return this.value;
            }
            set
            {
                this.First.TryToClean = false;
                this.TryToClean = false;

                this.value = value;
            }
        }

        public List<IndexVariabelnReference> Calls
        {
            get
            {
                return this.Deklaration.References;
            }
        }

        public List<SSACompileArgument> ArgumentsCalls
        {
            get;
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

        public SSAVariableMap OrgMap
        {
            get;
        }

        public SSAVariableMap First
        {
            get;
        }

        public bool TryToClean
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public SSAVariableMap(string key, VariableType type, IndexVariabelnDeklaration dek)
        {
            this.First = this;
            this.OrgMap = this;
            this.Key = key;
            this.Deklaration = dek;
            this.Kind = type;
            this.MutableState = VariableMutableState.NotMutable;
            this.ArgumentsCalls = new List<SSACompileArgument>();
        }

        public SSAVariableMap(IndexVariabelnDeklaration dek)
        {
            this.First = this;
            this.OrgMap = this;
            this.Key = dek.Name;
            this.Deklaration = dek;
            this.Value = LastValue.NotSet;
            this.Kind = this.GetVariableKind(dek);
            if (!dek.IsMutable) this.MutableState = VariableMutableState.NotMutable;
            this.ArgumentsCalls = new List<SSACompileArgument>();
        }

        public SSAVariableMap(SSAVariableMap value)
        {
            this.First = value.First;
            //this.AllSets.AddRange(value.AllSets);
            this.OrgMap = value;
            this.Key = value.Key;
            this.Reference = value.Reference;
            this.Deklaration = value.Deklaration;
            this.Value = value.Value;
            this.Kind = value.Kind;
            this.MutableState = value.MutableState;
            this.ArgumentsCalls = value.ArgumentsCalls;
            this.TryToClean = value.TryToClean;
        }

        #endregion ctor

        #region methods

        public bool AddArg(SSACompileArgument arg)
        {
            this.TryToClean = false;
            this.First.TryToClean = false;

            this.ArgumentsCalls.Add(arg);

            return true;
        }


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
            NotNull,
            NeverCall

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