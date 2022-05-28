using System.Collections.Generic;

namespace Yama.Automaten
{
    public interface Zustand
    {

        #region get/set

        List<Relation>? Relationen
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        Zustand? Check ( Token token, Automat automat );

        #endregion methods

    }
}