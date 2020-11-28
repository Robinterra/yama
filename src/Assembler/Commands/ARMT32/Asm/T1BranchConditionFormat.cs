using System;

namespace Yama.Assembler.ARMT32
{
    public class T1BranchConditionFormat : IFormat
    {
        public string Name
        {
            get
            {
                return "T1BranchCondition";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 1) return false;

            uint firstFragment = ( request.Command << 8 ) & 0xFF00;
            firstFragment |= ( request.Arguments[0] ) & 0x00FF;

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