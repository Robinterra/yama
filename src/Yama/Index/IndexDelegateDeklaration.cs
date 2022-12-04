using Yama.Parser;

namespace Yama.Index
{

    public class IndexDelegateDeklaration : IParent
    {

        #region get/set

        public string Name
        {
            get;
            set;
        }

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public ValidUses ThisUses
        {
            get;
        }

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }

        public bool IsMapped
        {
            get;
            set;
        }

        public List<IndexVariabelnReference> References
        {
            get;
        }

        public GenericCall? GenericDeklaration
        {
            get;
            set;
        }

        public bool IsNullable
        {
            get
            {
                if (this.GenericDeklaration is null) return true;

                return false;
            }
        }

        public List<IndexVariabelnDeklaration> Parameters
        {
            get;
        }


        public IndexVariabelnReference ReturnValue
        {
            get
            {
                return this.GenericDeklaration!.Reference!;
            }
        }

        #endregion get/set

        #region ctor

        public IndexDelegateDeklaration(string name, ValidUses uses)
        {
            this.Name = name;
            this.Use = new ParserError();
            this.ThisUses = uses;
            this.ParentUsesSet = uses;
            this.References = new List<IndexVariabelnReference>();
            this.Parameters = new List<IndexVariabelnDeklaration>();
            //this.ReturnValue = returnValue;
        }

        #endregion ctor

        public bool IsInUse(int depth)
        {
            return true;
        }

        public bool PreMappen(ValidUses uses)
        {
            return true;
        }
    }

}