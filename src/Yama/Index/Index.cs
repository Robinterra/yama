using System;
using System.Collections.Generic;
using Yama.Parser;

namespace Yama.Index
{
    public class Index
    {

        #region get/set

        public List<IParseTreeNode> Roots
        {
            get;
            set;
        }

        public ValidUses RootValidUses
        {
            get;
            set;
        }

        public Dictionary<string, IndexNamespaceDeklaration> Namespaces
        {
            get;
            set;
        }

        public string StartNamespace
        {
            get;
            set;
        } = "Program";

        public List<IndexKlassenDeklaration> Register
        {
            get;
            set;
        }

        public List<IParseTreeNode> ZuCompilenNodes
        {
            get;
            set;
        }

        public List<IndexError> Errors
        {
            get;
            set;
        }

        public FunktionsDeklaration MainFunction
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public Index (  )
        {
            this.Roots = new List<IParseTreeNode>();
            this.Register = new List<IndexKlassenDeklaration>();
            this.Errors = new List<IndexError>();
            this.Namespaces = new Dictionary<string, IndexNamespaceDeklaration>();
            this.ZuCompilenNodes = new List<IParseTreeNode>();
        }

        private bool Indezieren()
        {
            foreach (IParseTreeNode node in this.Roots)
            {
                node.Indezieren(this, null);
            }

            if (this.MainFunction == null) this.CreateError(this.Roots[0], "Keine main function gefunden!");

            if (this.Errors.Count != 0) return false;

            this.RootValidUses = new ValidUses(this);

            return true;
        }

        public bool CreateIndex()
        {
            if (!this.Indezieren()) return false;

            this.MakeAllNamespace();

            return this.Mappen();
        }

        public IndexNamespaceDeklaration NamespaceAdd(IndexNamespaceDeklaration dek)
        {
            if (this.Namespaces.ContainsKey(dek.Name)) return this.Namespaces[dek.Name];

            this.Namespaces.Add(dek.Name, dek);

            return dek;
        }

        private bool MakeAllNamespace()
        {
            Dictionary<string, IndexNamespaceDeklaration> aviableNamespaces = new Dictionary<string, IndexNamespaceDeklaration>();

            this.MakeRegisterFromValidsNamespaces(this.Namespaces[this.StartNamespace], aviableNamespaces);

            foreach (KeyValuePair<string, IndexNamespaceDeklaration> nameSpace in aviableNamespaces)
            {
                this.Register.AddRange(nameSpace.Value.KlassenDeklarationen);
            }

            return true;
        }

        private bool MakeRegisterFromValidsNamespaces(IndexNamespaceDeklaration indexNamespaceDeklaration, Dictionary<string, IndexNamespaceDeklaration> inUse)
        {
            if (inUse.ContainsKey(indexNamespaceDeklaration.Name)) return true;

            inUse.Add(indexNamespaceDeklaration.Name, indexNamespaceDeklaration);

            foreach (IndexNamespaceReference refer in indexNamespaceDeklaration.Usings)
            {
                if (!this.Namespaces.ContainsKey(refer.Name)) return this.CreateError(refer.Use, "Der Namespace konnte nicht gefunden werden!");

                IndexNamespaceDeklaration dek = this.Namespaces[refer.Name];

                this.MakeRegisterFromValidsNamespaces(dek, inUse);
            }

            return true;
        }

        private bool Mappen()
        {

            foreach (IndexKlassenDeklaration klasse in this.Register)
            {
                this.RootValidUses.Add(klasse);
            }

            foreach (IndexKlassenDeklaration klasse in this.Register)
            {
                klasse.Mappen(this.RootValidUses);
                this.ZuCompilenNodes.Add(klasse.Use);
            }

            return this.Errors.Count == 0;
        }

        public bool CreateError(IParseTreeNode node, string msg = "Der Aufruf ist hier nicht erlaubt")
        {
            if (node == null) return false;

            IndexError error = new IndexError();
            error.Use = node;
            error.Msg = msg;
            this.Errors.Add(error);

            return false;
        }

        public bool SetMainFunction(FunktionsDeklaration funktionsDeklaration)
        {
            if (this.MainFunction != null) return this.CreateError(funktionsDeklaration, "Eine Main Funktion existiert bereits");

            this.MainFunction = funktionsDeklaration;

            return true;
        }

        #endregion ctor

    }

    public enum AccessModify
    {
        Public,
        Private
    }
}