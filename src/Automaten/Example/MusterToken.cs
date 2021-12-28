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

        #region ctor

        public MusterToken(byte[] daten)
        {
            this.Daten = daten;
        }

        #endregion ctor

        public static implicit operator MusterToken ( string daten )
        {
            return new MusterToken(Encoding.UTF8.GetBytes ( daten ));
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