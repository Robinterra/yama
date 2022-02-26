using Yama.Parser;

namespace Yama.Index
{
    public class IndexNamespaceDeklaration : IParent
    {

        #region get/set

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }


        public List<IndexKlassenDeklaration> KlassenDeklarationen
        {
            get;
            set;
        }

        public List<IndexNamespaceReference> References
        {
            get;
            set;
        }

        public List<IndexNamespaceReference> Usings
        {
            get;
            set;
        }

        public List<IndexEnumDeklaration> EnumDeklarationen
        {
            get;
            set;
        }

        private ValidUses? thisUses;

        public ValidUses ThisUses
        {
            get
            {
                if (this.thisUses != null) return this.thisUses;

                this.thisUses = new ValidUses(this.ParentUsesSet);

                return this.thisUses;
            }
        }

        public List<string> OriginKeys
        {
            get;
            set;
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

        #endregion get/set

        #region ctor

        public IndexNamespaceDeklaration ( IParseTreeNode use, string name )
        {
            this.Use = use;
            this.Name = name;
            this.ParentUsesSet = new();
            this.References = new List<IndexNamespaceReference>();
            this.KlassenDeklarationen = new List<IndexKlassenDeklaration>();
            this.Usings = new List<IndexNamespaceReference>();
            this.OriginKeys = new List<string>();
            this.EnumDeklarationen = new List<IndexEnumDeklaration>();
        }

        #endregion ctor

        #region methods

        public bool IsInUse (int depth)
        {
            return true;
        }

        public bool PreMappen(ValidUses uses)
        {
            return true;
        }

        public bool Mappen(ValidUses rootValidUses)
        {
            //this.ParentUsesSet = rootValidUses;

            //this.PreviusMappen();

            //this.KlassenMappen(this.KlassenDeklarationen, this.ThisUses);

            return true;
        }


        #endregion methods
    }
}