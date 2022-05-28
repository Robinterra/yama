using System.Diagnostics;
using Yama.Lexer;

namespace Yama.InformationOutput.Nodes
{

    public class MessaureTimeOutput : IOutputNode, IDisposable
    {

        #region get/set

        public Stopwatch Stopwatch
        {
            get;
        }

        public bool IsOK
        {
            get;
            set;
        }

        public string? Message
        {
            get;
        }

        public IOutputNode? OutputNode
        {
            get;
        }

        public OutputController OutputController
        {
            get;
        }

        public bool IsPrintingEnabled
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public MessaureTimeOutput(string msg, bool isPrintinEnabled, OutputController outputController)
        {
            this.Message = msg;
            this.IsOK = true;
            this.OutputController = outputController;
            this.IsPrintingEnabled = isPrintinEnabled;
            this.Stopwatch = new Stopwatch();
            this.Stopwatch.Start();
        }

        public MessaureTimeOutput(IOutputNode msg, bool isPrintinEnabled, OutputController outputController)
        {
            this.OutputNode = msg;
            this.IsOK = true;
            this.OutputController = outputController;
            this.IsPrintingEnabled = isPrintinEnabled;
            this.Stopwatch = new Stopwatch();
            this.Stopwatch.Start();
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            if (!this.IsPrintingEnabled) return true;

            if (this.Message is not null) o.Info.Write(this.Message);
            if (this.OutputNode is not null) this.OutputNode.Print(o);

            string printMessage = $"{this.Stopwatch.ElapsedMilliseconds} ms";

            if (this.IsOK) o.Info.Write("DONE ", foreColor: ConsoleColor.Green);
            else o.Info.Write("FAILED ", foreColor: ConsoleColor.Red);

            o.Info.Write(printMessage, newLine: true, foreColor: ConsoleColor.Yellow);

            return this.IsOK;
        }

        public void Dispose()
        {
            this.Stopwatch.Stop();

            this.OutputController.Print(this);
        }

        #endregion methods

    }

}