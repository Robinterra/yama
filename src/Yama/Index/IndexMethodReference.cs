using Yama.Parser;

namespace Yama.Index
{
    public class IndexMethodReference : IIndexReference
    {

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IndexMethodDeklaration Deklaration
        {
            get;
            set;
        }

    }
}