using LearnCsStuf.Basic;

namespace Yama.Index
{
    public interface IParent
    {

        string Name
        {
            get;
            set;
        }

        IParseTreeNode Use
        {
            get;
            set;
        }

        ValidUses ThisUses
        {
            get;
        }

        ValidUses ParentUsesSet
        {
            get;
            set;
        }

    }
}