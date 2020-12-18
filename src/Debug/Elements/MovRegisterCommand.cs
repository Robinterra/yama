using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class MovRegisterCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x5E;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[runtime.A] = runtime.Register[runtime.B];

            return true;
        }

    }
}