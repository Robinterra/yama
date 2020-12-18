using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class CmpImediateCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x2F;

        public bool Execute(Runtime runtime)
        {

            long cmpresult = (long)runtime.Register[runtime.A] - (long)runtime.C;

            runtime.Carry = cmpresult < 0;
            runtime.Zero = cmpresult == 0;

            return true;
        }

    }
}