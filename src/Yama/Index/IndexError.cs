using Yama.Parser;

namespace Yama.Index
{
    public class IndexError
    {

        public IParseTreeNode Use
        {
            get;
            set;
        }
        public string Msg { get; internal set; }
    }
}