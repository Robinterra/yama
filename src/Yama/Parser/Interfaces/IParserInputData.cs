namespace Yama.Parser
{

    public interface IParserInputData
    {

        #region get/set

        public string Name
        {
            get;
        }

        public Stream? InputStream
        {
            get;
        }

        #endregion get/set

    }

}