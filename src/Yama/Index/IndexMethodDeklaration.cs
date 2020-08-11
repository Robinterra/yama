using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexMethodDeklaration : IParent
    {

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public IndexKlassenDeklaration Parent
        {
            get;
            set;
        }

        public List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        public IndexKlassenDeklaration Klasse
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

        public IndexVariabelnReference ReturnValue
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

                return this.thisUses;
            }
        }

        public ValidUses ParentUsesSet { get;
        set; }
        public string AssemblyName
        {
            get
            {
                if (this.NameInText == "main") return "main";

                string pattern = "{0}_{1}_{2}";

                return string.Format(pattern, this.Klasse.Name, this.NameInText, this.Parameters.Count);
            }
        }

        public string NameInText
        {
            get
            {
                if (this.Use.Token.Kind != SyntaxKind.Operator) return this.Name;


                if ("!" == this.Name) return "Achtung";
                if ("==" == this.Name) return "Equal";
                if ("+" == this.Name) return "Addition";
                if ("++" == this.Name) return "Incrementation";
                if ("<" == this.Name) return "LessThen";
                if ("<=" == this.Name) return "LessThenEquals";
                if (">" == this.Name) return "GreaterThen";
                if (">=" == this.Name) return "GreaterThenEquals";
                if ("-" == this.Name) return "Subtraktion";
                if ("--" == this.Name) return "Decrementation";
                if ("*" == this.Name) return "Star";
                if ("/" == this.Name) return "Dvide";
                if ("&&" == this.Name) return "AndLogicBool";
                if ("&" == this.Name) return "AndLogicBinary";
                if ("||" == this.Name) return "OrLogicBool";
                if ("|" == this.Name) return "OrLogicBinary";
                if ("^" == this.Name) return "XorLogicBinary";
                if ("~" == this.Name) return "DeCtor";

                return "UnknownSonderzeichen";
            }
        }

        public bool IsMapped { get; private set; }

        public IndexMethodDeklaration (  )
        {
            this.References = new List<IndexVariabelnReference>();
            this.Parameters = new List<IndexVariabelnDeklaration>();
        }

        public bool Mappen()
        {
            if (this.IsMapped) return false;

            this.Container.Mappen(this.ThisUses);

            return this.IsMapped = true;
        }

        public bool PreMappen(ValidUses uses)
        {
            if (this.IsMapped) return false;

            this.ParentUsesSet = uses;

            foreach (IndexVariabelnDeklaration dek in this.Parameters)
            {
                dek.Mappen(this.ThisUses);
                //this.ThisUses.Add(dek);
            }

            this.ReturnValue.Mappen(this.ThisUses);

            return true;
        }
    }

    public enum MethodeType
    {
        Ctor,
        Operator,
        DeCtor,
        Methode,
        Static,
        Explicit,
        Implicit,
        Property,
        PropertyStatic
    }
}