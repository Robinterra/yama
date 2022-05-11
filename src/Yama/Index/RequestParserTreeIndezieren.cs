using Yama.Index;

namespace Yama.Index
{
    public class RequestParserTreeIndezieren
    {

        #region get/set

        public Index Index
        {
            get;
            set;
        }

        public IParent? Parent
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public RequestParserTreeIndezieren(Index index, IParent? indexContainer)
        {
            this.Index = index;
            this.Parent = indexContainer;
        }

        #endregion ctor

    }
}