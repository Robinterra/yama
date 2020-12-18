using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class PopCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x41;

        public bool Execute(Runtime runtime)
        {

            for (int i = 15; i >= 0; i--)
            {
                if (((runtime.C >> i) & (uint)0x1) == 0) continue;

                runtime.Register[13] += 4;

                uint adresse = runtime.Register[13];

                runtime.Register[i] = System.BitConverter.ToUInt32(runtime.Memory, (int)adresse);
            }

            return true;
        }

    }
}