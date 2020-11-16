using System;

namespace Yama.Assembler.ARMT32
{
    public class T3ImmediateFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "T3Immediate";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 3) return false;

            uint firstFragment = ( request.Command << 4 ) & 0xFFF0;
            firstFragment |= ( request.Arguments[1] ) & 0x000F;

            uint secondFragment = ( request.Arguments[0] << 8 ) & 0x0F00;
            secondFragment |= ( request.Arguments[2] << 4 ) & 0x7000;
            secondFragment |= ( request.Arguments[2] ) & 0x00FF;

            byte[] tmp = BitConverter.GetBytes ( firstFragment );
            request.Result.Add ( tmp[0] );
            request.Result.Add ( tmp[1] );

            tmp = BitConverter.GetBytes ( secondFragment );
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