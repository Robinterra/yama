using System.Collections.Generic;

namespace LearnCsStuf.Automaten
{
    public class MusterZustand : Zustand
    {

        #region get/set

        public List<Relation>? Relationen
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        #endregion ctor

        #region methods

        public Zustand? Check ( Token token, Automat automat )
        {
            if ( this.Relationen == null ) return null;
            if ( token == null ) return null;
            if ( automat == null ) return null;

            foreach ( Relation relation in this.Relationen )
            {
                if ( !relation.Check ( token, automat ) ) continue;

                return relation.Zu;
            }

            return null;
        }

        #endregion methods

    }
}