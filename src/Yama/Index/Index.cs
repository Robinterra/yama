using System;
using System.Collections.Generic;
using System.IO;
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

        public MethodeDeclarationNode MainFunction
        {
            get;
            set;
        }

        public List<FileInfo> AllUseFiles
        {
            get;
            set;
        }

        public List<IndexEnumDeklaration> RegisterEnums
        {
            get;
            set;
        }

        public IParent CurrentMethode
        {
            get;
            set;
        }

        public List<IIndexTypeSafety> IndexTypeSafeties
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
            this.RegisterEnums = new List<IndexEnumDeklaration>();
            this.IndexTypeSafeties = new List<IIndexTypeSafety>();
        }

        #endregion ctor

        #region methods

        private bool Indezieren()
        {
            Parser.Request.RequestParserTreeIndezieren request = new Parser.Request.RequestParserTreeIndezieren(this, null);

            foreach (IParseTreeNode node in this.Roots)
            {
                node.Indezieren(request);
            }

            if (this.MainFunction == null) this.CreateError(this.Roots[0], "No main method found!");

            if (this.Errors.Count != 0) return false;

            this.RootValidUses = new ValidUses(this);

            return true;
        }

        public bool CreateIndex()
        {
            if (!this.Indezieren()) return false;

            this.MakeAllNamespace();

            if (!this.Mappen()) return false;

            if (!this.ExecuteTypeSafties()) return false;

            return true;
        }

        private bool ExecuteTypeSafties()
        {
            RequestTypeSafety request = new RequestTypeSafety();
            request.Index = this;

            foreach (IIndexTypeSafety indexTypeSafety in this.IndexTypeSafeties)
            {
                indexTypeSafety.CheckExecute(request);
            }

            return this.Errors.Count == 0;
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

            if (!this.Namespaces.ContainsKey(this.StartNamespace)) return this.CreateError(null, string.Format("no start '{0}' namespace found", this.StartNamespace));

            this.MakeRegisterFromValidsNamespaces(this.Namespaces[this.StartNamespace], aviableNamespaces);

            foreach (KeyValuePair<string, IndexNamespaceDeklaration> nameSpace in aviableNamespaces)
            {
                this.AllUseFiles.AddRange(nameSpace.Value.Files);

                this.RegisterEnums.AddRange(nameSpace.Value.EnumDeklarationen);

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
                if (!this.Namespaces.ContainsKey(refer.Name)) return this.CreateError(refer.Use, "The Namespace can not be found!");

                IndexNamespaceDeklaration dek = this.Namespaces[refer.Name];

                this.MakeRegisterFromValidsNamespaces(dek, inUse);
            }

            return true;
        }

        public string GetTypeName(IndexVariabelnReference reference)
        {
            if (reference == null) return string.Empty;
            if (reference.ParentCall != null) return this.GetTypeName(reference.ParentCall, reference);

            if (reference.Deklaration is IndexKlassenDeklaration t) return t.Name;
            if (reference.Deklaration is IndexVariabelnDeklaration vd) return vd.Type.Name;
            if (reference.Deklaration is IndexPropertyGetSetDeklaration pgsd) return pgsd.ReturnValue.Name;
            if (reference.Deklaration is IndexVektorDeklaration ved) return ved.ReturnValue.Name;
            if (reference.Deklaration is IndexMethodDeklaration md) return md.ReturnValue.Name;
            if (reference.Deklaration is IndexPropertyDeklaration pd) return pd.Type.Name;
            if (reference.Deklaration is IndexEnumEntryDeklaration) return "int";

            return string.Empty;
        }

        public string GetTypeName(IndexVariabelnReference reference, IndexVariabelnReference parent)
        {
            string typeName = this.GetTypeName(reference);

            if (parent.ClassGenericDefinition == null) return typeName;
            if (parent.GenericDeklaration == null) return typeName;
            if (string.IsNullOrEmpty(typeName)) return typeName;

            if (typeName != parent.ClassGenericDefinition.Token.Text) return typeName;

            return parent.GenericDeklaration.Token.Text;
        }

        private bool Mappen()
        {

            foreach (IndexEnumDeklaration klasse in this.RegisterEnums)
            {
                this.RootValidUses.Add(klasse);
            }

            foreach (IndexKlassenDeklaration klasse in this.Register)
            {
                this.RootValidUses.Add(klasse);
            }

            foreach (IndexKlassenDeklaration klasse in this.Register)
            {
                klasse.PreMappen(this.RootValidUses);
            }

            bool isnotallmapped = true;

            while (isnotallmapped)
            {
                isnotallmapped = false;

                if (this.Errors.Count != 0) return false;

                foreach (IndexKlassenDeklaration klasse in this.Register)
                {
                    klasse.PreviusMappen(this.RootValidUses);
                }

                foreach (IndexKlassenDeklaration klasse in this.Register)
                {
                    bool isok = klasse.Mappen();

                    if (!isok)
                    {
                        isnotallmapped = true;

                        continue;
                    }

                    if (this.ZuCompilenNodes.Contains(klasse.Use)) continue;

                    this.ZuCompilenNodes.Add(klasse.Use);
                }
            }

            return this.Errors.Count == 0;
        }

        public bool CreateError(IParseTreeNode node, string msg = "The call is not allowed here")
        {
            IndexError error = new IndexError();
            error.Use = node;
            error.Msg = msg;
            this.Errors.Add(error);

            return false;
        }

        public bool SetMainFunction(MethodeDeclarationNode funktionsDeklaration)
        {
            if (this.MainFunction != null) return this.CreateError(funktionsDeklaration, "One Main Method is already exist");

            this.MainFunction = funktionsDeklaration;

            return true;
        }

        #endregion methods

    }

    public enum AccessModify
    {
        Public,
        Private
    }
}