using System.Collections.Generic;

namespace Yama.Parser
{
    public class ParserLayer
    {
        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Name
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IParseTreeNode> ParserMembers
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public ParserLayer(string name)
        {
            this.Name = name;
            this.ParserMembers = new List<IParseTreeNode>();
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------
    }
}