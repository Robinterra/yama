using System;

namespace Yama.Assembler.Commands.AVR.Asm
{

    public class AvrFormatRegisterImmediate : IFormat
    {
        public string Name
        {
            get
            {
                return "FormatRegisterImmediate";
            }
        }

        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 2) return false;

            uint firstFragment = (request.Command << 10) & 0xf800;
            firstFragment |= ( request.Arguments[0] << 4 ) & 0x01f0;
            firstFragment |= ( request.Arguments[1] << 5 ) & 0x0600;
            firstFragment |= ( request.Arguments[1] ) & 0x000f;

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