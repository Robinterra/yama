using Yama.Parser;

namespace Yama.Assembler
{
    public class RequestIdentify
    {

        #region get/set

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
        public bool IsData
        {
            get;
            set;
        }

        public int Size
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public RequestIdentify ( IParseTreeNode node, Assembler assembler )
        {
            this.Size = -1;
            this.Node = node;
            this.Assembler = assembler;
        }

        #endregion ctor
    }
}