using Yama.Index;

namespace Yama.Parser.Request
{
    public class RequestParserTreeIndezieren
    {

        #region get/set

        public Index.Index Index
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

        public RequestParserTreeIndezieren(Index.Index index, IParent? indexContainer)
        {
            this.Index = index;
            this.Parent = indexContainer;
        }

        #endregion ctor

    }
}