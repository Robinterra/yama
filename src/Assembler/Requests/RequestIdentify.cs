using Yama.Parser;

namespace Yama.Assembler
{
    public class RequestIdentify
    {
        public Assembler Assembler
        {
            get;
            set;
        }
        public IParseTreeNode Node
        {
            get;
            set;
        }
    }
}