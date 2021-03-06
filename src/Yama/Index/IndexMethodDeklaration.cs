using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexMethodDeklaration : IParent, IMethode
    {

        public IParseTreeNode Use
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

        public List<string> Tags
        {
            get;
            set;
        } = new List<string>();

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

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }

        public string KeyName
        {
            get
            {
                if (this.NameInText == "main") return "main";

                StringBuilder build = new StringBuilder();
                foreach (IndexVariabelnDeklaration dek in this.Parameters)
                {
                    if (dek.Name == "this") continue;

                    build.AppendFormat("_{0}", dek.Type.Name);
                }

                string pattern = "{0}_{1}{2}";

                return string.Format(pattern, this.NameInText, this.Parameters.Count, build.ToString());
            }
        }

        public string AssemblyName
        {
            get
            {
                if (this.NameInText == "main") return "main";

                StringBuilder build = new StringBuilder();
                foreach (IndexVariabelnDeklaration dek in this.Parameters)
                {
                    build.AppendFormat("_{0}", dek.Type.Name);
                }

                string pattern = "{0}_{1}_{2}{3}";

                return string.Format(pattern, this.Klasse.Name, this.NameInText, this.Parameters.Count, build.ToString());
            }
        }

        public string NameInText
        {
            get
            {
                if (this.Use.Token.Kind != IdentifierKind.Operator) return this.Name;


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

        public bool IsMapped
        {
            get;
            set;
        }

        public bool IsInUse (int depth)
        {
            if (depth > 20) return true;
            if (this.Name == "main") return true;
            if (this.Name == "Malloc") return true;
            if (this.Name == "Free") return true;

            depth += 1;

            foreach (IndexVariabelnReference reference in this.References)
            {
                if (reference == null) return true;

                if (reference.IsOwnerInUse(depth)) return true;
            }

            if (this.Klasse.InheritanceBase == null) return false;
            if (!(this.Klasse.InheritanceBase.Deklaration is IndexKlassenDeklaration dek)) return false;
            IMethode parentMethods = dek.Methods.FirstOrDefault(u=>u.KeyName == this.KeyName);
            if (parentMethods == null) return false;
            if (!(parentMethods.Use is MethodeDeclarationNode t)) return false;
            if (t.Equals(this)) return false;

            return t.CanCompile(depth);
        }

        public IndexMethodDeklaration (  )
        {
            this.References = new List<IndexVariabelnReference>();
            this.Parameters = new List<IndexVariabelnDeklaration>();
        }

        public bool Mappen()
        {
            if (this.IsMapped) return false;

            this.ThisUses.GetIndex.CurrentMethode = this;

            this.Container.Mappen(this.ThisUses);

            return this.IsMapped = true;
        }

        public bool PreMappen(ValidUses uses)
        {
            if (this.IsMapped) return false;

            uses.GetIndex.CurrentMethode = this;

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
        PropertyStatic,
        PropertyGetSet,
        PropertyStaticGetSet,
        VektorMethode,
        VektorStatic
    }
}