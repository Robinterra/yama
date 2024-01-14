using System.Linq;

namespace Yama.ProjectConfig
{
    public class Package
    {

        // -----------------------------------------------

        private string? name;

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string? Name
        {
            get
            {
                if ( !string.IsNullOrEmpty ( this.name ) ) return this.name;
                if (this.GitRepository == null) return null;

                string[] splits = this.GitRepository.Split ( "/" );
                string? result = splits.LastOrDefault ();

                if ( string.IsNullOrEmpty ( result ) ) return null;

                this.name = result.Replace ( ".git", string.Empty );

                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        // -----------------------------------------------

        public string? GitRepository
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string? GitBranch
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string? LocalPath
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool GitAutomaticUpdate
        {
            get;
            set;
        }

        // -----------------------------------------------

        public PackageType Type
        {
            get
            {
                if ( !string.IsNullOrEmpty ( this.GitRepository ) ) return PackageType.Git;
                if ( !string.IsNullOrEmpty ( this.LocalPath ) ) return PackageType.Local;

                return PackageType.None;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public enum PackageType
        {
            None,  
            Local,
            Git
        }

    }
}