using System.IO;
using System.Text;

namespace Yama.Compiler
{
    public class IRCodePrintStream
    {

        #region vars

        private int emptyStrings;

        #endregion vars

        #region get/set

        public StreamWriter Writer
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public IRCodePrintStream ( StreamWriter writer )
        {
            this.Writer = writer;
        }

        #endregion ctor

        #region methods

        public bool Add(SSACompileLine line)
        {
            if (this.Writer == null) return false;
            if (line.ReplaceLine != null) return true;

            StringBuilder result = new StringBuilder();

            if (this.AddFunktionsDeklaration(line)) return true;
            if (this.AddFunktionsEnde(line)) return true;
            if (this.AddReferenceCall(line)) return true;

            string printMode = line.Owner.Algo.Mode == "default" ? string.Empty : line.Owner.Algo.Mode;
            string isNotUseChar = line.IsUsed ? "" : "/!\\";
            result.AppendFormat("{3}{0}: {4}{1}{2}", line.Order, line.Owner.Algo.Name, printMode, new string(' ', emptyStrings), isNotUseChar);

            foreach (SSACompileArgument arg in line.Arguments)
            {
                if (arg.Mode == SSACompileArgumentMode.Const) result.AppendFormat(" #0x{0:x}", arg.Const);
                if (arg.Mode == SSACompileArgumentMode.Reference) result.AppendFormat(" {0}", arg.Reference.Order);
                if (arg.Mode == SSACompileArgumentMode.Variable) result.AppendFormat(" {0}", arg.Variable.Reference.Order);
                if (arg.Mode == SSACompileArgumentMode.JumpReference) result.AppendFormat(" {0}", arg.CompileReference.Line.Order);
            }

            this.Writer.WriteLine(result);

            return true;
        }

        private bool AddReferenceCall(SSACompileLine line)
        {
            if (!(line.Owner is CompileReferenceCall rc)) return false;
            if (rc.Node == null) return false;

            string isNotUseChar = line.IsUsed ? "" : "/!\\";
            string result = string.Format("{2}{0}: {3}{1} (  )", line.Order, rc.Node.Token.Text, new string(' ', emptyStrings), isNotUseChar);

            this.Writer.WriteLine(result);

            return true;
        }

        private bool AddFunktionsEnde(SSACompileLine line)
        {
            if (!(line.Owner is CompileFunktionsEnde fe)) return false;

            emptyStrings -= 4;

            this.Writer.WriteLine("}");
            this.Writer.WriteLine();

            return true;
        }

        private bool AddFunktionsDeklaration(SSACompileLine line)
        {
            if (!(line.Owner is CompileFunktionsDeklaration fd)) return false;

            string result = string.Format("{0} (  )\n{{", fd.Node.Token.Text);

            this.Writer.WriteLine();
            this.Writer.WriteLine(result);

            emptyStrings += 4;

            return true;
        }


        #endregion methods

    }
}