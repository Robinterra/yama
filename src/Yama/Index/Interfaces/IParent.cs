using System.Collections.Generic;
using Yama.Parser;

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

        bool IsMapped
        {
            get;
            set;
        }
        bool IsInUse(int depth);

        bool PreMappen(ValidUses uses);
    }
}