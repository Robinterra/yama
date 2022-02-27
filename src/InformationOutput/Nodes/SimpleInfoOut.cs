using Yama.Lexer;

namespace Yama.InformationOutput.Nodes
{

    public class SimpleInfoOut : IOutputNode
    {

        #region get/set

        public string Msg
        {
            get;
        }

        #endregion get/set

        #region ctor

        public SimpleInfoOut(string msg)
        {
            this.Msg = msg;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            o.Info.Write(this.Msg);

            return true;
        }

        #endregion methods

    }

}