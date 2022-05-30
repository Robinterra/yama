using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class CmpRegisterCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x5F;

        public bool Execute(Runtime runtime)
        {

            long cmpresult = (long)runtime.Register[runtime.A] - (long)runtime.Register[runtime.B];

            runtime.StatusRegister.Carry = cmpresult < 0;
            runtime.StatusRegister.Zero = cmpresult == 0;

            return true;
        }

    }
}