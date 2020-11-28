using System;

namespace Yama.Assembler.ARMT32
{
    public class T1InputOutputSmallFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "T1InputOutputSmall";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 3) return false;

            uint firstFragment = ( request.Command << 11 ) & 0xF800;
            firstFragment |= ( request.Arguments[2] << 6 ) & 0x07C0;
            firstFragment |= ( request.Arguments[1] << 3 ) & 0x0038;
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