using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LearnCsStuf.Automaten
{
    public class MusterToken : Token
    {

        #region get/set

        public byte[] Daten
        {
            get;
            set;
        }

        #endregion get/set

        public static implicit operator MusterToken ( string daten )
        {
            return new MusterToken { Daten = Encoding.UTF8.GetBytes ( daten ) };
        }

        #region methods

        public bool CheckDaten ( Token token, Automat automat )
        {
            if ( token == null ) return false;

            return this.Daten.SequenceEqual ( token.Daten );
        }

        #endregion methods

    }
}