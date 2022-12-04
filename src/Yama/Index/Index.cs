using System;
using System.Collections.Generic;
using System.IO;
using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
using Yama.Parser;

namespace Yama.Index
{
    public class Index
    {
        private IndexKlassenDeklaration? IntKlasse;

        #region get/set

        public IEnumerable<IIndexNode> Roots
        {
            get;
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

        public VariableNameing Nameing
        {
            get;
        }

        public string StartNamespace
        {
            get;
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

        public MethodeDeclarationNode? MainFunction
        {
            get;
            set;
        }

        public List<string> AllUseFiles
        {
            get;
        }

        public List<IndexEnumDeklaration> RegisterEnums
        {
            get;
            set;
        }

        public IParent? CurrentMethode
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

        public Index ( IEnumerable<IIndexNode> rootIndexNodes, string startNamespace, List<string> allFilesInUse)
        {
            this.Nameing = new VariableNameing();
            this.RootValidUses = new ValidUses(this);
            this.Roots = rootIndexNodes;
            this.StartNamespace = startNamespace;
            this.Register = new List<IndexKlassenDeklaration>();
            this.Errors = new List<IndexError>();
            this.Namespaces = new Dictionary<string, IndexNamespaceDeklaration>();
            this.ZuCompilenNodes = new List<IParseTreeNode>();
            this.RegisterEnums = new List<IndexEnumDeklaration>();
            this.IndexTypeSafeties = new List<IIndexTypeSafety>();
            this.AllUseFiles = allFilesInUse;
        }

        #endregion ctor

        #region methods

        private bool Indezieren()
        {
            RequestParserTreeIndezieren request = new RequestParserTreeIndezieren(this, null);

            foreach (IIndexNode node in this.Roots)
            {
                node.Indezieren(request);
            }

            if (this.MainFunction == null) return false;//this.CreateError("No main method found!");

            if (this.Errors.Count != 0) return false;

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
            RequestTypeSafety request = new RequestTypeSafety(this);

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

            if (!this.Namespaces.ContainsKey(this.StartNamespace)) return this.AddError(new (new IndexCritError($"no start '{this.StartNamespace}' namespace found")));

            this.MakeRegisterFromValidsNamespaces(this.Namespaces[this.StartNamespace], aviableNamespaces);

            foreach (KeyValuePair<string, IndexNamespaceDeklaration> nameSpace in aviableNamespaces)
            {
                this.AllUseFiles.AddRange(nameSpace.Value.OriginKeys);

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
            if (reference.IsPointIdentifier && reference.ParentCall != null) return this.GetTypeName(reference.ParentCall, reference);

            if (reference.Deklaration is IndexKlassenDeklaration t) return t.Name;
            if (reference.Deklaration is IndexVariabelnDeklaration vd) return vd.Type.Name;
            if (reference.Deklaration is IndexPropertyGetSetDeklaration pgsd) return pgsd.ReturnValue.Name;
            if (reference.Deklaration is IndexVektorDeklaration ved) return ved.ReturnValue.Name;
            if (reference.Deklaration is IndexMethodDeklaration md) return md.ReturnValue.Name;
            if (reference.Deklaration is IndexPropertyDeklaration pd) return pd.Type.Name;
            if (reference.Deklaration is IndexEnumEntryDeklaration) return "int";

            return string.Empty;
        }

        public IParent? GetIndexType(IndexVariabelnReference reference)
        {
            if (reference.IsPointIdentifier && reference.ParentCall != null) return this.GetIndexType(reference.ParentCall, reference);

            if (reference.Deklaration is IndexKlassenDeklaration t) return t;
            if (reference.Deklaration is IndexVariabelnDeklaration vd) return vd.Type.Deklaration;
            if (reference.Deklaration is IndexPropertyGetSetDeklaration pgsd) return pgsd.ReturnValue.Deklaration;
            if (reference.Deklaration is IndexVektorDeklaration ved) return ved.ReturnValue.Deklaration;
            if (reference.Deklaration is IndexPropertyDeklaration pd) return pd.Type.Deklaration;
            if (reference.Deklaration is IndexEnumEntryDeklaration) return this.IntKlasse;
            if (reference.Deklaration is IndexMethodDeklaration md)
            {
                if (reference.IsMethodCalled) return md.ReturnValue.Deklaration;

                return md;
            }
            if (reference.Deklaration is IndexDelegateDeklaration idd)
            {
                if (reference.IsMethodCalled) return idd.ReturnValue.Deklaration;

                return idd;
            }

            return reference.Deklaration;
        }

        public IParent? GetIndexType(IndexVariabelnReference reference, IndexVariabelnReference parent)
        {
            IParent? typeName = this.GetIndexType(reference);
            if (typeName is IndexDelegateDeklaration idd)
            {
                if (reference.IsMethodCalled) return idd.ReturnValue.Deklaration;

                return idd;
            }

            if (parent.ClassGenericDefinition is null) return typeName;
            if (parent.GenericDeklaration is null) return typeName;
            if (typeName is null) return typeName;

            if (typeName.Name != parent.ClassGenericDefinition.Token.Text) return typeName;
            if (parent.GenericDeklaration.Reference is null) return null;

            return parent.GenericDeklaration.Reference.Deklaration;
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

            this.RootValidUses.Add(new IndexDelegateDeklaration("Func", this.RootValidUses));

            foreach (IndexEnumDeklaration klasse in this.RegisterEnums)
            {
                this.RootValidUses.Add(klasse);
            }

            foreach (IndexKlassenDeklaration klasse in this.Register)
            {
                if (klasse.Name == "int") this.IntKlasse = klasse;

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

        public bool AddError(IndexError error)
        {
            this.Errors.Add(error);

            return true;
        }

        public bool CreateError(IParseTreeNode node, string msg = "The call is not allowed here")
        {
            this.Errors.Add(new (node, msg));

            return false;
        }

        public bool SetMainFunction(MethodeDeclarationNode funktionsDeklaration)
        {
            if (this.MainFunction != null) return this.CreateError(funktionsDeklaration, "One Main Method is already exist");

            this.MainFunction = funktionsDeklaration;

            return true;
        }

        public bool ExistTypeInheritanceHistory ( string leftName, IndexVariabelnReference? reference )
        {
            if ( reference is null ) return false;

            if (reference.Deklaration is IndexVariabelnDeklaration vd) return this.ExistTypeInheritanceHistory ( leftName, vd.Type );
            if (reference.Deklaration is IndexPropertyGetSetDeklaration pgsd) return this.ExistTypeInheritanceHistory ( leftName, pgsd.ReturnValue );
            if ( reference.Deklaration is IndexVektorDeklaration ved ) return this.ExistTypeInheritanceHistory ( leftName, ved.ReturnValue );
            if ( reference.Deklaration is IndexMethodDeklaration md ) return this.ExistTypeInheritanceHistory ( leftName, md.ReturnValue );
            if ( reference.Deklaration is IndexPropertyDeklaration pd ) return this.ExistTypeInheritanceHistory ( leftName, pd.Type );
            if ( !(reference.Deklaration is IndexKlassenDeklaration t) ) return false;

            if ( t.Name == leftName ) return true;
            if ( this.ExistTypeInheritanceHistory ( leftName, t.InheritanceBase ) ) return true;

            return false;
        }



        #endregion methods

    }

    public enum AccessModify
    {
        Public,
        Private
    }
}