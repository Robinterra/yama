using System;

namespace Yama.Assembler.ARMT32
{
    public class T1SmallRegisterFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "T1Register";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 2) return false;

            uint firstFragment = ( request.Command << 8 ) & 0xFF00;
            firstFragment |= ( request.Arguments[0] << 4 ) & 0x0080;
            firstFragment |= ( request.Arguments[1] << 3 ) & 0x0078;
            firstFragment |= ( request.Arguments[0] ) & 0x0007;

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