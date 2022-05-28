namespace Yama.Automaten
{
    public interface Token
    {

        #region get/set

        byte[] Daten
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        bool CheckDaten ( Token token, Automat automat );

        #endregion methods

    }
}