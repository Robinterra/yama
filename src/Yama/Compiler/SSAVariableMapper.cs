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

        public List<SSACompileLine> AllSets
        {
            get;
            set;
        } = new List<SSACompileLine>();

        //Die Letzte Reference, bei dem die Variable gesetzt wurde
        public SSACompileLine Reference
        {
            get;
            set;
        }

        public bool IsChecked
        {
            get;
            set;
        }

        public List<ICompileRoot> Calls
        {
            get;
            set;
        } = new List<ICompileRoot>();

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

        #endregion get/set

        #region ctor

        public SSAVariableMap()
        {

        }

        public SSAVariableMap(SSAVariableMap value)
        {
            this.Key = value.Key;
            this.Reference = value.Reference;
            this.Calls = new List<ICompileRoot>();
            this.Deklaration = value.Deklaration;
            this.IsChecked = value.IsChecked;
        }

        #endregion ctor
    }

}