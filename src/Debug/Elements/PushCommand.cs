using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class PushCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x40;

        public bool Execute(Runtime runtime)
        {

            for (int i = 0; i < 16; i++)
            {
                if (((runtime.C >> i) & (uint)0x1) == 0) continue;

                byte[] daten = System.BitConverter.GetBytes(runtime.Register[i]);
                uint adresse = runtime.Register[13];

                runtime.Memory[adresse] = daten[0];
                runtime.Memory[adresse + 1] = daten[1];
                runtime.Memory[adresse + 2] = daten[2];
                runtime.Memory[adresse + 3] = daten[3];

                runtime.Register[13] -= 4;
            }

            return true;
        }

    }
}