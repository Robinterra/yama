using LearnCsStuf.Basic;

namespace Yama.Index
{
    public class IndexKlassenReference
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