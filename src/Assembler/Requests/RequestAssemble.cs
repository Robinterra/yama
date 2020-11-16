using System.Collections.Generic;
using System.IO;
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
    }
}