using System;

namespace Yama.Assembler.ARMT32
{
    public class ConstFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "Const";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            uint firstFragment = ( request.Command >> 16 ) & 0xFFFF;

            uint secondFragment = ( request.Command ) & 0xFFFF;

            byte[] tmp = BitConverter.GetBytes ( secondFragment );
            request.Result.Add ( tmp[0] );
            request.Result.Add ( tmp[1] );

            tmp = BitConverter.GetBytes ( firstFragment );
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