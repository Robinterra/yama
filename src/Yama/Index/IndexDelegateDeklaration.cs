using Yama.Lexer;
using Yama.Parser;

namespace Yama.Index
{

    public class IndexDelegateDeklaration : IParent
    {

        private GenericCall? genericCall;

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
            get
            {
                return this.genericCall;
            }
            set
            {
                this.genericCall = value;
                if (value is null) return;

                bool isfirst = true;
                foreach (IndexVariabelnReference parme in value.References)
                {
                    if (isfirst)
                    {
                        isfirst = false;
                        continue;
                    }

                    IndexVariabelnDeklaration ivd = new IndexVariabelnDeklaration(value, "", parme);
                    ivd.IsBorrowing = true;
                    this.Parameters.Add(ivd);
                }
            }
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
                return this.GenericDeklaration!.References.FirstOrDefault()!;
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
            this.Parameters = new();
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