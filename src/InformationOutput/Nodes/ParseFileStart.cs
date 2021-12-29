using Yama.Lexer;

namespace Yama.InformationOutput.Nodes
{

    public class ParseFileStart : IOutputNode
    {

        #region get/set

        public FileInfo File
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ParseFileStart(FileInfo file)
        {
            this.File = file;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            string filename = this.File.FullName;

            string printMessage = $"Parse File: '{filename}' ";

            o.Info.Write(printMessage);

            return true;
        }

        #endregion methods

    }

}