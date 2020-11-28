using System.Collections.Generic;
using System.IO;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler
{
    public class RequestAssemble
    {
        public FileInfo InputFile
        {
            get;
            set;
        }

        public List<ICompileRoot> Roots
        {
            get;
            set;
        }

        public Stream Stream
        {
            get;
            set;
        }

        public bool WithMapper
        {
            get;
            set;
        }
        public bool IsSkipper
        {
            get;
            set;
        }
    }
}