using System;
using System.IO;
using System.Text;
using Yama.Index;
using Yama.Parser;

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

            ContinueMode continueMode = this.PrintUnit(line, result);
            if (continueMode == ContinueMode.Break) return true;

            if (continueMode == ContinueMode.Continue)
            {
                foreach (SSACompileArgument arg in line.Arguments)
                {
                    if (arg.Mode == SSACompileArgumentMode.Const) result.AppendFormat(" #0x{0:x}", arg.Const);
                    if (arg.Mode == SSACompileArgumentMode.Reference && arg.Reference is not null) result.AppendFormat(" {0}", arg.Reference.Order);
                    if (arg.Mode == SSACompileArgumentMode.Variable && arg.Variable is not null && arg.Variable.Reference is not null) result.AppendFormat(" {0}", arg.Variable.Reference.Order);
                    if (arg.Mode == SSACompileArgumentMode.JumpReference && arg.CompileReference is not null) result.AppendFormat(" {0}", arg.CompileReference.Line?.Order);
                }
            }

            this.Writer.WriteLine(result);

            return true;
        }

        private ContinueMode PrintUnit(SSACompileLine line, StringBuilder result)
        {
            if (this.AddReturn(line, result)) return ContinueMode.PrintDirect;
            if (this.AddFunktionsDeklaration(line, result)) return ContinueMode.Continue;
            if (this.AddFunktionsEnde(line, result)) return ContinueMode.Continue;
            if (this.AddWhileLoop(line, result)) return ContinueMode.Continue;
            if (this.AddIfExpression(line, result)) return ContinueMode.PrintDirect;
            if (this.AddForLoop(line, result)) return ContinueMode.Continue;
            if (this.AddFreeLoop(line, result)) return ContinueMode.Break;
            if (this.AddFreeIfExpression(line, result)) return ContinueMode.PrintDirect;
            if (this.AddReferenceCall(line, result)) return ContinueMode.Continue;

            string? printMode = line.Owner.Algo!.Mode == "default" ? string.Empty : line.Owner.Algo.Mode;
            string isNotUseChar = line.IsUsed ? "" : "/!\\";
            result.AppendFormat("{3}{0}: {4}{1}{2}", line.Order, line.Owner.Algo.Name, printMode, new string(' ', emptyStrings), isNotUseChar);

            return ContinueMode.Continue;
        }

        private bool AddFreeIfExpression(SSACompileLine line, StringBuilder result)
        {
            if (line.FlowTask != ProgramFlowTask.IsIfStatementEnde) return false;

            //string? printMode = line.Owner.Algo!.Mode == "default" ? string.Empty : line.Owner.Algo.Mode;
            //string isNotUseChar = line.IsUsed ? "" : "/!\\";
            //result.AppendFormat("{3}{0}: {4}{1}{2}\n", line.Order, line.Owner.Algo.Name, printMode, new string(' ', emptyStrings), isNotUseChar);

            if (emptyStrings != 0) this.emptyStrings = this.emptyStrings - 4;

            result.AppendFormat("{0}}}", new string(' ', emptyStrings));

            return true;
        }

        private bool AddReturn(SSACompileLine line, StringBuilder result)
        {
            if (line.FlowTask != ProgramFlowTask.IsReturn) return false;

            SSACompileArgument? arg = line.References.FirstOrDefault();

            result.AppendFormat("{1}{0}: return {2}", line.Order, new string(' ', emptyStrings), arg?.Reference?.Order);

            return true;
        }

        private bool AddIfExpression(SSACompileLine line, StringBuilder result)
        {
            if (!(line.Owner is CompileJumpWithCondition)) return false;
            if (!(line.Owner.Node is IfKey ifk)) return false;

            SSACompileArgument arg = line.Arguments.First();

            result.AppendFormat("{2}{0}: {1} {3}\n{2}{{", line.Order, ifk.Token.Text, new string(' ', emptyStrings), arg.Reference?.Order);

            this.emptyStrings = this.emptyStrings + 4;

            return true;
        }

        private bool AddFreeLoop(SSACompileLine line, StringBuilder result)
        {
            if (line.FlowTask != ProgramFlowTask.IsLoopEnde) return false;

            if (emptyStrings != 0) this.emptyStrings = this.emptyStrings - 4;

            return true;
        }

        private bool AddWhileLoop(SSACompileLine line, StringBuilder result)
        {
            if (!(line.Owner is CompileJumpWithCondition)) return false;
            if (!(line.Owner.Node is WhileKey ifk)) return false;

            result.AppendFormat("{2}{0}: {1}", line.Order, ifk.Token.Text, new string(' ', emptyStrings));

            this.emptyStrings = this.emptyStrings + 4;

            return true;
        }

        private bool AddForLoop(SSACompileLine line, StringBuilder result)
        {
            if (!(line.Owner is CompileJumpWithCondition)) return false;
            if (!(line.Owner.Node is ForKey ifk)) return false;

            result.AppendFormat("{2}{0}: {1}", line.Order, ifk.Token.Text, new string(' ', emptyStrings));

            this.emptyStrings = this.emptyStrings + 4;

            return true;
        }

        private bool AddReferenceCall(SSACompileLine line, StringBuilder result)
        {
            if (!(line.Owner is CompileReferenceCall rc)) return false;
            if (rc.Node == null) return false;

            string methodName = rc.Node.Token.Text;
            if (rc.Node is NewKey nk)
            {
                if (nk.Reference is null) return false;
                if (nk.Reference.Deklaration is not IndexMethodDeklaration method) return false;
                if (method.Klasse is null) return false;

                methodName = string.Format("{0}.{1}", method.Klasse.Name, methodName);
            }

            if ( rc.Node is ReferenceCall rfc ) methodName = IRCodePrintStream.GetMethodeName ( rfc, methodName );

            string isNotUseChar = line.IsUsed ? "" : "/!\\";
            result.AppendFormat("{2}{0}: {3}{1} (  )", line.Order, methodName, new string(' ', emptyStrings), isNotUseChar);

            return true;
        }

        private static string GetMethodeName ( ReferenceCall rfc, string methodName )
        {
            string? klasse = null;
            if (rfc.Reference is null) return methodName;

            if ( rfc.Reference.Deklaration is IndexMethodDeklaration md ) klasse = md.Klasse?.Name;
            if ( rfc.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsd ) klasse = pgsd.Klasse?.Name;
            if ( rfc.Reference.Deklaration is IndexPropertyDeklaration pd ) klasse = pd.Klasse?.Name;

            if ( klasse == null ) return methodName;
            return string.Format ( "{0}.{1}", klasse, methodName );
        }

        private bool AddFunktionsEnde(SSACompileLine line, StringBuilder result)
        {
            if (!(line.Owner is CompileFunktionsEnde fe)) return false;

            if (emptyStrings != 0) emptyStrings -= 4;

            result.AppendLine("}");

            return true;
        }

        private bool AddFunktionsDeklaration(SSACompileLine line, StringBuilder result)
        {
            if (line.Owner is not CompileFunktionsDeklaration fd) return false;
            if (fd.Node is not MethodeDeclarationNode md) return false;
            if (md.Deklaration is null) return false;
            if (md.Deklaration.Klasse is null) return false;

            result.AppendFormat("{1}.{0} (  )\n{{", fd.Node.Token.Text, md.Deklaration.Klasse.Name);

            emptyStrings += 4;

            return true;
        }


        #endregion methods

    }

    public enum ContinueMode
    {
        Continue,
        Break,
        PrintDirect
    }
}