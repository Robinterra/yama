namespace Yama.Index
{
    public class RequestTypeSafety
    {

        #region get/set

        public Index Index
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public RequestTypeSafety(Index index)
        {
            this.Index = index;
        }

        #endregion ctor

    }
}