using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class AdcRegisterCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x51;

        public bool Execute(Runtime runtime)
        {
            ulong result = (ulong)runtime.Register[runtime.B] + (ulong)runtime.Register[runtime.C] + (runtime.Carry ? (ulong)1 : 0);

            runtime.Carry = result == 0x100000000;

            runtime.Register[runtime.A] = (uint) result;

            return true;
        }

    }
}