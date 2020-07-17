using Yama.Parser;

namespace Yama.Index
{
    public interface IIndexReference
    {

        IParseTreeNode Use
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }
    }
}