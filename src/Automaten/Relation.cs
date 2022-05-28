using System.Collections.Generic;

namespace Yama.Automaten
{
    public interface Relation
    {

        #region get/set

        Zustand Von
        {
            get;
            set;
        }

        Zustand Zu
        {
            get;
            set;
        }

        Token Bedingung
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        bool Check ( Token token, Automat automat );

        #endregion methods

    }
}