using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class SubImediateCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x12;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[runtime.A] = (~runtime.C) + 1 + runtime.Register[runtime.B];

            return true;
        }

    }
}