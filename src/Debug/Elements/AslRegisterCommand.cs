using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class AslRegisterCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x5A;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[runtime.A] = runtime.Register[runtime.B] << (int) runtime.Register[runtime.C];

            return true;
        }

    }
}