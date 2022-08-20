using System;

namespace Yama.Assembler.Runtime
{
    public class Format2 : IFormat
    {
        private IAssemblerDefinition definition;

        public string Name
        {
            get
            {
                return "Format2";
            }
        }

        public Format2(IAssemblerDefinition assemblerDefinition)
        {
            this.definition = assemblerDefinition;
        }


        // https://developer.arm.com/docs/ddi0597/h/base-instructions-alphabetic-order/adc-adcs-immediate-add-with-carry-immediate
        public bool Assemble(RequestAssembleFormat request)
        {
            uint condition = definition.GetCondition(request.Condition);

            uint firstFragment = ( request.Command << 24 ) & 0xFF000000;
            firstFragment |= ( condition << 20 ) & 0x00F00000;
            firstFragment |= ( request.RegisterDestionation << 16 ) & 0x000F0000;
            firstFragment |= ( request.RegisterInputLeft << 12) & 0x0000F000;
            firstFragment |= ( request.Immediate ) & 0x00000FFF;

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