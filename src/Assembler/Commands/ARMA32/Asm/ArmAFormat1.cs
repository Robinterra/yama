using System;

namespace Yama.Assembler.ARMA32
{
    public class ArmAFormat1 : IFormat
    {
        public string Name
        {
            get
            {
                return "Format1";
            }
        }

        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 4) return false;

            uint firstFragment = ( request.Arguments[0] << 28 ) & 0xF0000000;
            firstFragment |= ( request.Command << 20 ) & 0x0FF00000;
            firstFragment |= ( request.Arguments[2] << 16 ) & 0x000F0000;
            firstFragment |= ( request.Arguments[1] << 12) & 0x0000F000;
            firstFragment |= ( request.Arguments[3] ) & 0x0000000F;

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