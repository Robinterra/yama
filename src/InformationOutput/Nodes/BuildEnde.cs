using System.Diagnostics;
using Yama.Lexer;

namespace Yama.InformationOutput.Nodes
{

    public class BuildEnde : IOutputNode
    {

        #region get/set

        public Stopwatch Stopwatch
        {
            get;
        }

        public bool IsOK
        {
            get;
        }

        #endregion get/set

        #region ctor

        public BuildEnde(Stopwatch stopwatch, bool isok)
        {
            this.Stopwatch = stopwatch;
            this.IsOK = isok;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            string printMessage = $"{this.Stopwatch.ElapsedMilliseconds} ms";

            o.Info.Write(null, true);

            if (this.IsOK) o.Info.Write("Build succeeeded ", foreColor: ConsoleColor.Green);
            else o.Info.Write("Build failed ", foreColor: ConsoleColor.Red);

            o.Info.Write(printMessage, newLine: true, foreColor: ConsoleColor.Yellow);

            o.Info.Write(null, true);

            return this.IsOK;
        }

        #endregion methods

    }

}