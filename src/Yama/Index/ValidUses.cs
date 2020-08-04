using System.Collections.Generic;

namespace Yama.Index
{
    public class ValidUses
    {

        /*private List<IndexKlassenDeklaration> klassen;

        public List<IndexKlassenDeklaration> Klassen
        {
            get
            {
                List<IndexKlassenDeklaration> result = new List<IndexKlassenDeklaration>();

                result.AddRange(this.klassen);
                if (this.Parent != null) result.AddRange(this.Parent.Klassen);

                return result;
            }
            set
            {
                this.klassen = value;
            }
        }

        private List<IndexPropertyDeklaration> properties;

        public List<IndexPropertyDeklaration> Properties
        {
            get
            {
                List<IndexPropertyDeklaration> result = new List<IndexPropertyDeklaration>();

                result.AddRange(this.properties);
                if (this.Parent != null) result.AddRange(this.Parent.Properties);

                return result;
            }

            set
            {
                this.properties = value;
            }
        }

        private List<IndexMethodDeklaration> methods;
        public List<IndexMethodDeklaration> Methods
        {
            get
            {
                List<IndexMethodDeklaration> result = new List<IndexMethodDeklaration>();

                result.AddRange(this.methods);
                if (this.Parent != null) result.AddRange(this.Parent.methods);

                return result;
            }
            set
            {
                this.methods = value;
            }
        }

        private List<IndexVariabelnDeklaration> variabeln;
        public List<IndexVariabelnDeklaration> Variabeln
        {
            get
            {
                List<IndexVariabelnDeklaration> result = new List<IndexVariabelnDeklaration>();

                result.AddRange(this.variabeln);
                if (this.Parent != null) result.AddRange(this.Parent.Variabeln);

                return result;
            }
            set
            {
                this.variabeln = value;
            }
        }*/

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

        public ValidUses Parent { get; }
        private Index index;
        public Index GetIndex
        {
            get
            {
                if (this.index != null) return this.index;

                if (this.Parent == null) return null;

                return this.Parent.GetIndex;
            }
        }

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

        public bool Add(IParent parent)
        {
            this.deklarationen.Add(parent);

            return true;
        }
    }
}