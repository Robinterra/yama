using System.Collections.Generic;
using System.IO;
using Yama.Parser;

namespace Yama.Assembler
{
    public class RequestAssembleCommand
    {
        public IParseTreeNode Node
        {
            get;
            set;
        }

        public Assembler Assembler
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

        public List<ICommand> Result
        {
            get;
            set;
        } = new List<ICommand>();
    }
}