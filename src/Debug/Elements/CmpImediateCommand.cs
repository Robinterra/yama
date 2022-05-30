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

            runtime.StatusRegister.Carry = cmpresult < 0;
            runtime.StatusRegister.Zero = cmpresult == 0;

            return true;
        }

    }
}