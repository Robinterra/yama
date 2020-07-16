using Yama.Parser;

namespace Yama.Index
{
    public class IndexVariabelnReference : IIndexReference
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

        public IndexVariabelnDeklaration Deklaration
        {
            get;
            set;
        }


    }
}