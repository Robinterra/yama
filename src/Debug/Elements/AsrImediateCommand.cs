using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class ASRImediateCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x19;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[runtime.A] = runtime.Register[runtime.B] >> (int)runtime.C;

            return true;
        }

    }
}