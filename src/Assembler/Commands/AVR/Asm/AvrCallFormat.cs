using System;

namespace Yama.Assembler.Commands.AVR.Asm
{

    public class AvrCallFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "CallFormat";
            }
        }

        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 2) return false;

            uint firstFragment = (request.Command << 25) & 0xfe000000;
            firstFragment |= ( request.Arguments[0] << 4 ) & 0x01f00000;
            firstFragment |= ( request.Arguments[0] ) & 0x0001ffff;
            firstFragment |= (request.Arguments[1] << 17) & 0x000e0000;

            byte[] tmp = BitConverter.GetBytes ( firstFragment );
            request.Result.Add ( tmp[0] );
            request.Result.Add ( tmp[1] );
            request.Result.Add ( tmp[2] );
            request.Result.Add ( tmp[3] );

            return true;
        }

        public bool DisAssemble(RequestDisAssembleFormat request)
        {
            return true;
        }
    }

}