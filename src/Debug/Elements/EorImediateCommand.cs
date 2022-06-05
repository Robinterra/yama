using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class EorImediateCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x17;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[runtime.A] = runtime.Register[runtime.B] ^ runtime.C;

            return true;
        }

    }
}