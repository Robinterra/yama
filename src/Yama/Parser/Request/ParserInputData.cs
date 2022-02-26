namespace Yama.Parser
{

    public class ParserInputData : IParserInputData
    {

        #region get/set

        public string Name
        {
            get;
        }

        public Stream InputStream
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ParserInputData(string name, Stream inputStream)
        {
            this.Name = name;
            this.InputStream = inputStream;
        }

        #endregion ctor

    }

}