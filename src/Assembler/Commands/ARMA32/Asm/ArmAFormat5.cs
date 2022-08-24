using System;

namespace Yama.Assembler.ARMA32
{
    public class ArmAFormat5 : IFormat
    {
        private IAssemblerDefinition definition;

        public string Name
        {
            get
            {
                return "Format5";
            }
        }

        public ArmAFormat5(IAssemblerDefinition assemblerDefinition)
        {
            this.definition = assemblerDefinition;
        }


        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            uint condition = definition.GetCondition(request.Condition);

            uint firstFragment = ( condition << 28 ) & 0xF0000000;
            firstFragment |= ( request.Command << 20 ) & 0x0FF00000;
            firstFragment |= ( request.RegisterInputRight << 16 ) & 0x00000F00;
            firstFragment |= ( request.RegisterDestionation << 12) & 0x0000F000;
            firstFragment |= ( request.Stype << 7) & 0x000000F0;
            firstFragment |= ( request.RegisterInputLeft ) & 0x0000000F;

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