using System.Diagnostics;
using Yama.Lexer;

namespace Yama.InformationOutput.Nodes
{

    public class ParseFileEnde : IOutputNode
    {

        #region get/set

        public Stopwatch Stopwatch
        {
            get;
        }

        public bool IsFailed
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ParseFileEnde(Stopwatch stopwatch, bool isfailed = false)
        {
            this.Stopwatch = stopwatch;
            this.IsFailed = isfailed;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {

            string printMessage = $"{this.Stopwatch.ElapsedMilliseconds} ms";

            if (!IsFailed) o.Info.Write("DONE ", foreColor: ConsoleColor.Green);
            else o.Info.Write("FAILED ", foreColor: ConsoleColor.Red);

            o.Info.Write(printMessage, newLine: true, foreColor: ConsoleColor.Yellow);

            return true;
        }

        #endregion methods

    }

}