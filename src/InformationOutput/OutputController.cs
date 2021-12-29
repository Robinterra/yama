namespace Yama.InformationOutput
{

    public class OutputController
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

        RequestOutput Request;

        #endregion get/set

        #region ctor

        public OutputController()
        {
            this.Info = new ConsoleWriter();
            this.Error = new ConsoleErrorWriter();
            this.Request = new RequestOutput(this.Info, this.Error);
        }

        #endregion ctor

        #region methods

        public bool Print(IOutputNode node)
        {
            return node.Print(this.Request);
        }

        public bool Print(IEnumerable<IOutputNode> nodes)
        {

            foreach (IOutputNode node in nodes)
            {
                node.Print(this.Request);
            }

            return true;
        }

        #endregion methods

    }

}