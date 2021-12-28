namespace Yama.ProjectConfig.Nodes
{
    public class RequestDeserialize
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public Project Project
        {
            get;
        }

        // -----------------------------------------------

        public Package? Package
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public RequestDeserialize(Project project)
        {
            this.Project = project;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

    }
}