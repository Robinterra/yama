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

        public bool IsClone
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ConfigPackageRefreshStart(Package package, bool isclone = false)
        {
            this.Package = package;
            this.IsClone = isclone;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            string typeName = IsClone ? "Clone" : "Refresh";
            string printMessage = $"{typeName} '{Package.Name}' Package ";

            o.Info.Write(printMessage);

            return true;
        }

        #endregion methods

    }

}