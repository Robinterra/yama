using System;

namespace Yama.Assembler.Commands.AVR.Asm
{

    public class AvrCmdFormat: IFormat
    {
        public string Name
        {
            get
            {
                return "CmdFormat";
            }
        }

        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 0) return false;

            uint firstFragment = (request.Command) & 0xffff;

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