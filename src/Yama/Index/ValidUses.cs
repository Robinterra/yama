namespace Yama.Index
{
    public class ValidUses
    {

        #region get/set

        private List<IParent> deklarationen;
        public List<IParent> Deklarationen
        {
            get
            {
                List<IParent> result = new List<IParent>();

                if (this.Parent != null) result.AddRange(this.Parent.Deklarationen);
                result.AddRange(this.deklarationen);

                return result;
            }
            set
            {
                this.deklarationen = value;
            }
        }

        public ValidUses? Parent
        {
            get;
        }

        private Index? index;
        public Index? GetIndex
        {
            get
            {
                if (this.index != null) return this.index;

                if (this.Parent == null) return null;

                return this.Parent.GetIndex;
            }
        }

        #endregion get/set

        #region ctor

        public ValidUses()
        {
            this.deklarationen = new List<IParent>();
        }

        public ValidUses(Index index) : this()
        {
            this.index = index;
            /*this.klassen = new List<IndexKlassenDeklaration>();
            this.variabeln = new List<IndexVariabelnDeklaration>();
            this.properties = new List<IndexPropertyDeklaration>();
            this.methods = new List<IndexMethodDeklaration>();*/
        }

        public ValidUses(ValidUses parent) : this()
        {
            this.Parent = parent;
        }

        #endregion ctor

        #region methods

        public bool Add(IParent parent)
        {
            this.deklarationen.Add(parent);

            return true;
        }

        #endregion methods

    }
}