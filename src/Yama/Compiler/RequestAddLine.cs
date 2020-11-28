using System.Collections.Generic;

namespace Yama.Compiler
{
    public class RequestAddLine
    {

        public string AssemblyCode
        {
            get;
            set;
        }

        public Dictionary<string, string> Dictionaries
        {
            get;
            set;
        }

        public Dictionary<string, string> PostReplaces
        {
            get;
            set;
        }

        public ICompileRoot Root
        {
            get;
            set;
        }

        public RequestAddLine(ICompileRoot root, string assemblyCode)
        {
            this.Root = root;
            this.AssemblyCode = assemblyCode;
        }

        public RequestAddLine(ICompileRoot root, string assemblyCode, Dictionary<string, string> dictionary)
        {
            this.Dictionaries = dictionary;
            this.AssemblyCode = assemblyCode;
            this.Root = root;
        }

        public RequestAddLine(ICompileRoot root, string assemblyCode, Dictionary<string, string> dictionary, Dictionary<string, string> postreplaces)
        : this(root, assemblyCode, dictionary)
        {
            this.PostReplaces = postreplaces;
        }
    }
}