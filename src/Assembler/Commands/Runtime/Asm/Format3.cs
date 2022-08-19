using System;

namespace Yama.Assembler.Runtime
{
    public class Format3 : IFormat
    {
        public string Name
        {
            get
            {
                return "Format3";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            uint firstFragment = ( request.Command << 24 ) & 0xFF000000;
            firstFragment |= ( request.Condition << 20 ) & 0x00F00000;
            firstFragment |= ( request.RegisterDestionation << 16 ) & 0x000F0000;
            firstFragment |= ( request.Immediate ) & 0x0000FFFF;

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