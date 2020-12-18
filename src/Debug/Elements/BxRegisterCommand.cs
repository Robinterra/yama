using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class BxRegisterCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x30;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[15] = runtime.Register[runtime.A];

            return true;
        }

    }
}