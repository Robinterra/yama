using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class StrCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x1C;

        public bool Execute(Runtime runtime)
        {
            uint adresse = runtime.Register[runtime.B] + (runtime.C << 2);

            byte[] daten = System.BitConverter.GetBytes(runtime.Register[runtime.A]);

            runtime.Memory[adresse] = daten[0];
            runtime.Memory[adresse + 1] = daten[1];
            runtime.Memory[adresse + 2] = daten[2];
            runtime.Memory[adresse + 3] = daten[3];

            return true;
        }

    }
}