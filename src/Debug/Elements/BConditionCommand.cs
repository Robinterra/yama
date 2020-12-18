using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class BConditionCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x32;

        public bool Execute(Runtime runtime)
        {
            bool ispositive = (runtime.C & 0x8000) == 0;

            if (!ispositive) runtime.C = runtime.C | 0xFFFF0000;

            runtime.C = runtime.C << 2;

            runtime.Register[15] += runtime.C;

            return true;
        }

    }
}