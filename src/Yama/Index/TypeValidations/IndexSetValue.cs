namespace Yama.Index
{
    public class IndexSetValue : IIndexTypeSafety
    {

        #region get/set

        public IndexVariabelnReference LeftRef
        {
            get;
            set;
        }

        public IndexVariabelnReference RightRef
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public IndexSetValue ( IndexVariabelnReference leftRef, IndexVariabelnReference rightRef )
        {
            this.LeftRef = leftRef;
            this.RightRef = rightRef;
        }

        #endregion ctor

        #region methods

        public bool CheckExecute(RequestTypeSafety request)
        {
            if (this.LeftRef == null) return true;

            string leftName = request.Index.GetTypeName(this.LeftRef);
            string rightName = request.Index.GetTypeName(this.RightRef);

            request.Index.GetTypeName(this.RightRef);

            if (leftName == rightName) return true;

            request.Index.CreateError(this.LeftRef == null ? this.RightRef.Use : this.LeftRef.Use, string.Format("Set Value has not correct type, expectet: {0}, currently: {1}", leftName, rightName));

            return false;
        }

        #endregion methods

    }
}