using System.Collections.Generic;

namespace LearnCsStuf.Automaten
{
    public class MusterRelation : Relation
    {

        #region get/set

        public Zustand Von
        {
            get;
            set;
        }

        public Zustand Zu
        {
            get;
            set;
        }

        public Token Bedingung
        {
            get;
            set;
        }

        #endregion get/set

        public MusterRelation(Zustand von, Zustand zu, Token bedingung)
        {
            this.Von = von;
            this.Zu = zu;
            this.Bedingung = bedingung;
        }

        #region methods

        public bool Check ( Token token, Automat automat )
        {
            if (this.Bedingung.CheckDaten ( token, automat )) return true;

            return false;
        }

        #endregion methods

    }
}