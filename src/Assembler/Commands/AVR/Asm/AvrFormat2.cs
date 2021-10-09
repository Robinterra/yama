using System;

namespace Yama.Assembler.Commands.AVR.Asm
{

    public class AvrFormat2 : IFormat
    {
        public string Name
        {
            get
            {
                return "Format2";
            }
        }

        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 2) return false;

            uint firstFragment = (request.Command << 10) & 0xfC00;
            firstFragment |= ( request.Arguments[0] << 3 ) & 0x03F8;
            firstFragment |= ( request.Arguments[1] ) & 0x0007;

            byte[] tmp = BitConverter.GetBytes ( firstFragment );
            request.Result.Add ( tmp[0] );
            request.Result.Add ( tmp[1] );

            return true;
        }

        public bool DisAssemble(RequestDisAssembleFormat request)
        {
            return true;
        }
    }

}