using System.Linq;

namespace Yama.ProjectConfig
{
    public class Package
    {

        // -----------------------------------------------

        private string name;

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Name
        {
            get
            {
                if ( !string.IsNullOrEmpty ( this.name ) ) return this.name;

                string[] splits = this.GitRepository.Split ( "/" );
                string result = splits.LastOrDefault ();

                if ( string.IsNullOrEmpty ( result ) ) return null;

                this.name = result.Replace ( ".git", string.Empty );

                return this.name;
            }
        }

        // -----------------------------------------------

        public string GitRepository
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string GitBranch
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

    }
}