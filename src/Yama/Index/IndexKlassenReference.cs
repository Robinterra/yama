using Yama.Parser;

namespace Yama.Index
{
    public class IndexKlassenReference : IIndexReference
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

        public IndexKlassenDeklaration Deklaration
        {
            get;
            set;
        }
    }
}