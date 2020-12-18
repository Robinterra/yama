using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class MulRegisterCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x54;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[runtime.A] = runtime.Register[runtime.B] * runtime.Register[runtime.C];

            return true;
        }

    }
}