using Yama.ProjectConfig;

namespace Yama.InformationOutput.Nodes
{

    public class ConfigPackageRefreshStart : IOutputNode
    {

        #region get/set

        public Package Package
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ConfigPackageRefreshStart(Package package)
        {
            this.Package = package;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            string printMessage = $"Refresh '{Package.Name}' Package ";

            o.Info.Write(printMessage);

            return true;
        }

        #endregion methods

    }

}