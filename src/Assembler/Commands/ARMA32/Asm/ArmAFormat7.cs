using System;

namespace Yama.Assembler.ARMA32
{
    public class ArmAFormat7 : IFormat
    {
        private IAssemblerDefinition definition;

        public string Name
        {
            get
            {
                return "Format7";
            }
        }

        public ArmAFormat7(IAssemblerDefinition assemblerDefinition)
        {
            this.definition = assemblerDefinition;
        }


        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            uint condition = definition.GetCondition(request.Condition);

            uint firstFragment = ( condition << 28 ) & 0xF0000000;
            firstFragment |= ( request.Command << 20 ) & 0x0FF00000;
            firstFragment |= 0x000D0000;
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