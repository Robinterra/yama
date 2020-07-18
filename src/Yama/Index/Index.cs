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

        public List<IndexKlassenDeklaration> Register
        {
            get;
            set;
        }

        public List<IndexError> Errors
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
        }

        private bool Indezieren()
        {
            foreach (IParseTreeNode node in this.Roots)
            {
                node.Indezieren(this, null);
            }

            if (this.Errors.Count != 0) return false;

            this.RootValidUses = new ValidUses();

            foreach (IndexKlassenDeklaration klasse in this.Register)
            {
                this.RootValidUses.Deklarationen.Add(klasse);
            }

            return true;
        }

        public bool CreateIndex()
        {
            if (!this.Indezieren()) return false;

            return this.Mappen();
        }

        private bool Mappen()
        {
            foreach (IndexKlassenDeklaration klasse in this.Register)
            {
                klasse.Mappen(this.RootValidUses);
            }

            return true;
        }

        public bool CreateError(IParseTreeNode node)
        {
            if (node == null) return false;

            IndexError error = new IndexError();
            error.Use = node;
            this.Errors.Add(error);

            return false;
        }

        #endregion ctor

    }

    public enum AccessModify
    {
        Public,
        Private
    }
}