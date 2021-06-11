using System.Collections.Generic;
using System.IO;

namespace Yama.ProjectConfig
{
    public class Project
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public List<DirectoryInfo> SourcePaths
        {
            get;
            set;
        }

        // -----------------------------------------------

        public FileInfo OutputFile
        {
            get;
            set;
        }

        // -----------------------------------------------

        public FileInfo IROutputFile
        {
            get;
            set;
        }

        // -----------------------------------------------

        public FileInfo AssemblerOutputFile
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Optimize Optimize
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<DirectoryInfo> ExtensionsPaths
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string TargetPlattform
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string StartNamespace
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int Skip
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<string> Defines
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<Package> Packages
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Project (  )
        {
            this.SourcePaths = new List<DirectoryInfo>();
            this.ExtensionsPaths = new List<DirectoryInfo>();
            this.Defines = new List<string>();
            this.Packages = new List<Package>();
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

    }
}