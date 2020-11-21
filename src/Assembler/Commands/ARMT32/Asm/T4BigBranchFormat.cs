using System;

namespace Yama.Assembler.ARMT32
{
    public class T4BigBranchFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "T4BigBranch";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 1) return false;

            uint firstFragment = 0xF000;
            firstFragment |= ( (request.Arguments[0] & 0x80000000) == 0x80000000 ) ? (uint)0x0400 : (uint)0;
            firstFragment |= (request.Arguments[0] >> 11) & 0x03FF;

            uint secondFragment = 0x9000;
            secondFragment |= ( request.Arguments[0] ) & 0x07FF;

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