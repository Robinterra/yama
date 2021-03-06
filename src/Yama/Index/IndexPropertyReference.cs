using Yama.Parser;

namespace Yama.Index
{
    public class IndexPropertyReference : IIndexReference
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

        public IndexPropertyDeklaration Deklaration
        {
            get;
            set;
        }

    }
}