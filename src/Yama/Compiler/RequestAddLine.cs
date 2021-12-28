using System.Collections.Generic;

namespace Yama.Compiler
{
    public class RequestAddLine
    {

        #region get/set

        public string AssemblyCode
        {
            get;
            set;
        }

        public Dictionary<string, string>? Dictionaries
        {
            get;
            set;
        }

        public Dictionary<string, string>? PostReplaces
        {
            get;
            set;
        }

        public ICompileRoot Root
        {
            get;
            set;
        }

        public bool AddToList
        {
            get;
            set;
        } = true;

        #endregion get/set

        #region ctor

        public RequestAddLine(ICompileRoot rootm, string assemblyCode, bool addToList)
        {
            this.Root = rootm;
            this.AssemblyCode = assemblyCode;
            this.AddToList = addToList;
        }

        public RequestAddLine(ICompileRoot rootm, string assemblyCode, bool addToList, Dictionary<string, string> dictionary, Dictionary<string, string> postreplaces)
        {
            this.Root = rootm;
            this.AssemblyCode = assemblyCode;
            this.AddToList = addToList;
            this.PostReplaces = postreplaces;
            this.Dictionaries = dictionary;
        }

        public RequestAddLine(ICompileRoot root, string assemblyCode)
        {
            this.Root = root;
            this.AssemblyCode = assemblyCode;
        }

        public RequestAddLine(ICompileRoot root, string assemblyCode, Dictionary<string, string>? dictionary)
        {
            this.Dictionaries = dictionary;
            this.AssemblyCode = assemblyCode;
            this.Root = root;
        }

        public RequestAddLine(ICompileRoot root, string assemblyCode, Dictionary<string, string>? dictionary, Dictionary<string, string> postreplaces)
        : this(root, assemblyCode, dictionary)
        {
            this.PostReplaces = postreplaces;
        }

        #endregion ctor

    }
}