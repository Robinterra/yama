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

            IParent? leftHost = request.Index.GetIndexType(this.LeftRef);
            IParent? rightHost = request.Index.GetIndexType(this.RightRef);
            if (leftHost is null) return false;
            if (rightHost is null) return false;

            if (leftHost is IndexDelegateDeklaration idd) return this.CheckDelegate(idd, rightHost, request);

            //string leftName = request.Index.GetTypeName(this.LeftRef);
            //string rightName = request.Index.GetTypeName(this.RightRef);

            if ( leftHost.Name == rightHost.Name ) return true;
            if ( request.Index.ExistTypeInheritanceHistory ( leftHost.Name, this.RightRef ) ) return true;

            request.Index.CreateError(this.LeftRef == null ? this.RightRef.Use : this.LeftRef.Use, string.Format("Set Value has not correct type, expectet: {0}, currently: {1}", leftHost.Name, rightHost.Name));
             rightHost = request.Index.GetIndexType(this.RightRef);

            return false;
        }

        private bool CheckDelegate(IndexDelegateDeklaration idd, IParent rightHost, RequestTypeSafety request)
        {
            if (rightHost is not IndexMethodDeklaration imd) return false;
            if (idd.GenericDeklaration is null) return false;

            if (idd.GenericDeklaration.Token.Text == imd.ReturnValue.Name) return true;

            request.Index.CreateError(this.LeftRef == null ? this.RightRef.Use : this.LeftRef.Use, string.Format("Set Value has not correct type, expectet: {0}, currently: {1}", idd.Name, rightHost.Name));

            return false;
        }

        #endregion methods

    }
}