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

            foreach (IndexKlassenDeklaration klasse in this.Register)
            {
                this.RootValidUses.Add(klasse);
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