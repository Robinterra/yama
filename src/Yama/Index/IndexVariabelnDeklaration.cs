using Yama.Parser;

namespace Yama.Index
{
    public class IndexVariabelnDeklaration : IParent
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Name
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IndexVariabelnReference> References
        {
            get;
            set;
        }

        // -----------------------------------------------

        public IParseTreeNode Use
        {
            get;
            set;
        }

        // -----------------------------------------------

        public IndexVariabelnReference Type
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ValidUses ThisUses
        {
            get
            {
                return this.ParentUsesSet;
            }
        }

        // -----------------------------------------------

        public bool IsMapped
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ValidUses ParentUsesSet
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ValidUses? BaseUsesSet
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ValidUses? SetUsesSet
        {
            get;
            set;
        }

        // -----------------------------------------------

        public GenericCall? GenericDeklaration
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool IsNullable
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool IsBorrowing
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool IsMutable
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public IndexVariabelnDeklaration ( IParseTreeNode use, string name, IndexVariabelnReference varType )
        {
            this.Use = use;
            this.Name = name;
            this.Type = varType;
            this.ParentUsesSet = new();
            this.References = new List<IndexVariabelnReference>();
            this.IsMutable = true;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool PreMappen(ValidUses uses)
        {
            return true;
        }

        // -----------------------------------------------

        public bool Mappen(ValidUses uses)
        {
            this.ParentUsesSet = uses;
            if (uses.GetIndex is null) throw new NullReferenceException();

            VariableNameing nameing = uses.GetIndex.Nameing;

            if (this.Name == "this") return this.VariableIsThisKeyword (nameing);

            if (this.Name == "base") return this.VariableIsBaseKeyword (nameing);

            if (this.Name == "invalue") return this.VariableIsInValueKeyword (uses, nameing);

            this.Type.Mappen(uses);
            if ( this.Type.Deklaration is IndexKlassenDeklaration kd ) return this.TypeIsKlassenDeklaration ( kd, uses.GetIndex );

            return true;
        }

        private bool VariableIsThisKeyword (VariableNameing nameing)
        {
            IParent? parent = this.ParentUsesSet.Deklarationen.FirstOrDefault ( t => t.Name == nameing.This);

            if (parent is not IndexVariabelnDeklaration dek) return false;

            this.Type = dek.Type;
            this.Use = dek.Use;

            return true;
        }

        private bool VariableIsBaseKeyword (VariableNameing nameing)
        {
            if (this.BaseUsesSet is null) return false;

            IParent? parent = this.BaseUsesSet.Deklarationen.FirstOrDefault ( t => t.Name == nameing.Base );

            if (parent is not IndexVariabelnDeklaration dek) return false;

            this.Type = dek.Type;
            this.Use = dek.Use;

            return true;
        }

        private bool VariableIsInValueKeyword ( ValidUses uses, VariableNameing nameing)
        {
            this.SetUsesSet = uses;

            IParent? parent = this.SetUsesSet.Deklarationen.FirstOrDefault ( t => t.Name == nameing.InValue );
            if (parent is not IndexVariabelnDeklaration dek) return false;

            this.Type = dek.Type;
            this.Use = dek.Use;

            return true;
        }

        private bool TypeIsKlassenDeklaration ( IndexKlassenDeklaration kd, Index getIndex)
        {
            if ( kd.MemberModifier != ClassMemberModifiers.None )
            {
                if (this.IsBorrowing) getIndex.AddError(new IndexError(this.Use, "Borrowing is only possilbe on varaibles by reference"));

                return true;
            }

            this.IsNullable = true;

            return true;
        }

        // -----------------------------------------------

        public bool IsInUse (int depth)
        {
            if (depth > 10) return true;
            if (this.Name == "main") return true;

            depth += 1;

            foreach (IndexVariabelnReference reference in this.References)
            {
                if (reference.IsOwnerInUse(depth)) return true;
            }

            return false;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}