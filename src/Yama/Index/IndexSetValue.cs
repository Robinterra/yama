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

        private string GetTypeName(IndexVariabelnReference reference)
        {
            if (reference == null) return string.Empty;
            if (reference.ParentCall != null) return this.GetTypeName(reference.ParentCall);

            if (reference.Deklaration is IndexKlassenDeklaration t) return t.Name;
            if (reference.Deklaration is IndexVariabelnDeklaration vd) return vd.Type.Name;
            if (reference.Deklaration is IndexPropertyGetSetDeklaration pgsd) return pgsd.ReturnValue.Name;
            if (reference.Deklaration is IndexVektorDeklaration ved) return ved.ReturnValue.Name;
            if (reference.Deklaration is IndexMethodDeklaration md) return md.ReturnValue.Name;
            if (reference.Deklaration is IndexPropertyDeklaration pd) return pd.Type.Name;

            return string.Empty;
        }

        public bool CheckExecute(RequestTypeSafety request)
        {
            if (this.LeftRef == null) return true;

            if (this.GetTypeName(this.LeftRef) == this.GetTypeName(this.RightRef)) return true;

            request.Index.CreateError(this.LeftRef == null ? this.RightRef.Use : this.LeftRef.Use, string.Format("Set Value has not correct type, expectet: {0}, currently: {1}", this.GetTypeName(this.LeftRef), this.GetTypeName(this.RightRef)));

            return false;
        }

        #endregion methods

    }
}