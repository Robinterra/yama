using System.Collections.Generic;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexKlassenDeklaration : IParent
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

        public List<IndexKlassenReference> References
        {
            get;
            set;
        }

        public List<IndexPropertyDeklaration> IndexProperties
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> Operators
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> Ctors
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> DeCtors
        {
            get;
            set;
        }

        public List<IndexMethodDeklaration> Methods
        {
            get;
            set;
        }

        private ValidUses thisUses;

        public ValidUses ThisUses
        {
            get
            {
                if (this.thisUses != null) return this.thisUses;

                this.thisUses = new ValidUses(this.ParentUsesSet);

                IndexVariabelnDeklaration dekThisVar = new IndexVariabelnDeklaration();
                dekThisVar.Name = "this";
                dekThisVar.Type = new IndexVariabelnReference { Deklaration = this, Name = this.Name, Use = this.Use };
                dekThisVar.Use = this.Use;
                dekThisVar.ParentUsesSet = new ValidUses() { Deklarationen = new List<IParent> { this } };

                /*List<IParent> parents = new List<IParent>();
                parents.AddRange(this.Methods.OfType<IParent>());
                parents.AddRange(this.Operators.OfType<IParent>());
                parents.AddRange(this.Ctors.OfType<IParent>());
                parents.AddRange(this.DeCtors.OfType<IParent>());
                parents.AddRange(this.IndexProperties.OfType<IParent>());*/

                this.thisUses.Deklarationen = new List<IParent> { dekThisVar };

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }

        #endregion get/set

        #region ctor

        public IndexKlassenDeklaration (  )
        {
            
        }

        #endregion ctor
    }
}