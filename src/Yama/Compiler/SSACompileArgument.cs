using System.Collections.Generic;

namespace Yama.Compiler
{

    public class SSACompileArgument
    {

        public SSACompileLine Reference
        {
            get;
            set;
        }

        public int VarConst
        {
            get;
            set;
        }

        public string Order
        {
            get;
            set;
        }

        public SSACompileArgumentMode Mode
        {
            get;
            set;
        }
        public ICompileRoot Root { get; internal set; }

        public SSACompileArgument()
        {
            
        }

        public SSACompileArgument(SSACompileLine line)
        {
            this.Position = line;
            this.Mode = SSACompileArgumentMode.Position;
            this.Root = line.Compile;
        }

        public string GetText(Compiler compiler)
        {
            if (this.Mode == SSACompileArgumentMode.Position)
            {
                SSACompileLine line = compiler.GetLine(this.Position);

                return string.Format("({0})", line.Line);
            }
            if (this.Mode == SSACompileArgumentMode.Order) return this.Order;

            return string.Format("#{0}", this.VarConst);
        }
    }

    public enum SSACompileArgumentMode
    {
        None,
        Const,
        Reference,
        Order
    }
}