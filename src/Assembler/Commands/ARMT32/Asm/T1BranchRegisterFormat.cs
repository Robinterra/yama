using System;

namespace Yama.Assembler.ARMT32
{
    public class T1BranchRegisterFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "T1BLX";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 1) return false;

            uint firstFragment = ( request.Command << 7 ) & 0xFF80;
            firstFragment |= ( request.Arguments[0] << 3 ) & 0x0078;

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