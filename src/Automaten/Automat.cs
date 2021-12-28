using System;
using System.Collections.Generic;

namespace LearnCsStuf.Automaten
{
    public class Automat
    {
        // -----------------------------------------------

        private Zustand? current;

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public List<Zustand> Zustände
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Zustand Current
        {
            get
            {
                if (this.current is null) return this.Start;

                return this.current;
            }
            set
            {
                this.current = value;
            }
        }

        // -----------------------------------------------

        public Zustand Start
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<Token> Alphabet
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<Relation> Relationen
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<Zustand> Finales
        {
            get;
            set;
        }

        // -----------------------------------------------

        /*public Kellerspeicher Stack
        {
            get;
            set;
        }*/

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Automat ( List<Zustand> Q, Zustand s, List<Token> Alphabet, List<Zustand> F, List<Relation> relationen )
        {
            this.Zustände = Q;
            this.Start = s;
            this.Alphabet = Alphabet;
            this.Relationen = relationen;
            this.Finales = F;

            this.Reset (  );

            this.MakeMapping (  );
        }


        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        private bool MakeMapping (  )
        {
            foreach ( Relation relation in this.Relationen )
            {
                this.FindeZustand ( relation );
            }

            return true;
        }

        private bool FindeZustand ( Relation relation )
        {
            if (relation.Von.Relationen == null) relation.Von.Relationen = new List<Relation>();

            relation.Von.Relationen.Add(relation);

            return true;
        }

        public bool Execute ( List<Token> tokens )
        {
            foreach ( Token token in tokens )
            {
                Status status = this.Next ( token );

                if (status == Status.Failed) return false;
                if (status == Status.Final) return true;
            }

            return false;
        }

        public Status Next ( Token token )
        {
            Zustand? zustand = this.Current.Check ( token, this );

            if ( zustand == null ) return Status.Failed;

            this.Current = zustand;

            if ( this.CheckEnde (  ) ) return Status.Final;

            return Status.None;
        }

        private bool CheckEnde()
        {
            foreach ( Zustand ende in this.Finales )
            {
                if ( ende.Equals ( this.Current ) ) return true;
            }

            return false;
        }

        public bool Reset (  )
        {
            this.Current = this.Start;

            return true;
        }

        #endregion methods
    }

    public enum Status
    {
        None,
        Final,
        Failed
    }
}