using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class MovImediateCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x2E;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[runtime.A] = runtime.C;

            return true;
        }

    }
}