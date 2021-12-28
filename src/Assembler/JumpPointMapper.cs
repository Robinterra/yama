namespace Yama.Assembler
{
    public class JumpPointMapper
    {

        #region get/set

        public uint Adresse
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public JumpPointMapper(uint adresse, string key)
        {
            this.Adresse = adresse;
            this.Key = key;
        }

        #endregion ctor

    }
}