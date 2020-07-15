using System.Collections.Generic;
using System.Linq;
using LearnCsStuf.Basic;

namespace Yama.Index
{
    public class IndexMethodDeklaration : IParent
    {

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public List<IndexMethodReference> References
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public MethodeType Type
        {
            get;
            set;
        }

        public AccessModify AccessModify
        {
            get;
            set;
        }

        public IndexKlassenReference ReturnValue
        {
            get;
            set;
        }

        public List<IndexVariabelnDeklaration> Parameters
        {
            get;
            set;
        }

        public IndexContainer Container
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
                this.thisUses.Deklarationen = this.Parameters.OfType<IParent>().ToList<IParent>();

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }

    }

    public enum MethodeType
    {
        Ctor,
        Operator,
        DeCtor,
        Methode,
        Static,
        Explicit,
        Implicit
    }
}