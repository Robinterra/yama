using System.Diagnostics;
using Yama.Lexer;

namespace Yama.InformationOutput.Nodes
{

    public class OutputEnde : IOutputNode
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

        public OutputEnde(Stopwatch stopwatch, bool isok)
        {
            this.Stopwatch = stopwatch;
            this.IsOK = isok;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            string printMessage = $"{this.Stopwatch.ElapsedMilliseconds} ms";

            if (this.IsOK) o.Info.Write("DONE ", foreColor: ConsoleColor.Green);
            else o.Info.Write("FAILED ", foreColor: ConsoleColor.Red);

            o.Info.Write(printMessage, newLine: true, foreColor: ConsoleColor.Yellow);

            return this.IsOK;
        }

        #endregion methods

    }

}