using Yama.Parser;

namespace Yama.Index
{
    public class IndexParameterType : IIndexTypeSafety
    {

        #region get/set

        public IndexVariabelnDeklaration ParaDeclaration
        {
            get;
            set;
        }

        public IndexVariabelnReference TypeUse
        {
            get;
            set;
        }

        public int Position
        {
            get;
            set;
        }

        public IndexMethodDeklaration Methode
        {
            get;
            set;
        }

        public IndexVariabelnReference MethodCall
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public IndexParameterType ( IndexVariabelnDeklaration paradeclartion, IndexVariabelnReference typeUse, int pos, IndexMethodDeklaration dek, IndexVariabelnReference methodCall )
        {
            this.ParaDeclaration = paradeclartion;
            this.TypeUse = typeUse;
            this.Position = pos;
            this.Methode = dek;
            this.MethodCall = methodCall;
        }

        #endregion ctor

        #region methods

        public bool CheckExecute(RequestTypeSafety request)
        {
            if (this.ParaDeclaration.Type.Deklaration is IndexKlassenDeklaration ikd)
            {
                if (ikd.IsGeneric && this.MethodCall.GenericDeklaration is not null)
                {
                    if (this.MethodCall.GenericDeklaration.Token.Text == request.Index.GetTypeName(this.TypeUse)) return true;
                }
            }

            if (this.TypeUse.Use is NullKey) return true;
            if (this.ParaDeclaration.Type.Name  == "Any") return true;
            if (this.ParaDeclaration.Type.Name == request.Index.GetTypeName(this.TypeUse)) return true;
            if ( request.Index.ExistTypeInheritanceHistory ( this.ParaDeclaration.Type.Name, this.TypeUse ) ) return true;

            IParseTreeNode errorNode = this.MethodCall.ParentCall == null ? this.MethodCall.Use : this.MethodCall.ParentCall.Use;

            request.Index.CreateError(errorNode, string.Format("Parameter has not correct type, expectet: {0}, currently: {1}, Position: {2}", this.ParaDeclaration.Type.Name, request.Index.GetTypeName(this.TypeUse), this.Position));

            return false;
        }

        #endregion methods

    }
}