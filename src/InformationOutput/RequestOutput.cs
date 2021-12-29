namespace Yama.InformationOutput
{

    public class RequestOutput
    {

        #region get/set

        public IOutputWriter Info
        {
            get;
        }

        public IOutputWriter Error
        {
            get;
        }

        #endregion get/set

        #region ctor

        public RequestOutput(IOutputWriter info, IOutputWriter error)
        {
            this.Info = info;
            this.Error = error;
        }

        #endregion ctor

    }

}