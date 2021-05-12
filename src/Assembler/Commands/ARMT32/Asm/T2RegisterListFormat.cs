using System;

namespace Yama.Assembler.ARMT32
{
    public class T2RegisterListFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "T2RegisterList";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 1) return false;

            uint firstFragment = ( request.Command ) & 0x0FFF;
            firstFragment |= 0xE000;

            uint secondFragment = ( request.Arguments[0] ) & 0xFFFF;

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