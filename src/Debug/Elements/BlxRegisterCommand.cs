using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class BlxRegisterCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x31;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[14] = runtime.Register[15];
            runtime.Register[15] = runtime.Register[runtime.A];

            return true;
        }

    }
}