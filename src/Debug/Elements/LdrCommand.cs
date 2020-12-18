using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class LdrCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x1B;

        public bool Execute(Runtime runtime)
        {
            uint adresse = runtime.Register[runtime.B] + (runtime.C << 2);

            runtime.Register[runtime.A] = System.BitConverter.ToUInt32(runtime.Memory, (int)adresse);

            return true;
        }

    }
}