using Yama.Assembler.ARMA32;
using Yama.Assembler.Runtime;

namespace Yama.Assembler.Definitions
{

    public class ArmA32Definition : IAssemblerDefinition
    {
        public string Name
        {
            get
            {
                return "arm-a32-linux";
            }
        }

        public Assembler SetupDefinition(Assembler assembler)
        {
            assembler.DataAddition = 4;
            assembler.Definition = new AssemblerDefinition();
            assembler.Definition.ProgramCounterIncress = 4;
            assembler.Definition.CommandEntitySize = 4;

            this.T3ImmediateDefinitionen ( assembler.Definition );
            this.T3RegisterDefinitionen ( assembler.Definition );
            //this.T3asrDefinition ( assembler.Definition );
            //this.T1RegisterDefinition ( assembler.Definition );
            //this.BranchesDefinitionen ( assembler.Definition );
            this.GenerateRegister ( assembler.Definition );

            return assembler;
        }

        private void T3RegisterDefinitionen(AssemblerDefinition definition)
        {
            ArmAFormat1 format1 = new ArmAFormat1();
            definition.Formats.Add(format1);

            definition.Commands.Add(new Command3Register("add", format1, 0x08, 4));
            definition.Commands.Add(new Command3Register("and", format1, 0x00, 4));
        }

        private void T3ImmediateDefinitionen(AssemblerDefinition definition)
        {
            ArmAFormat3 format3 = new ArmAFormat3();
            definition.Formats.Add(format3);

            definition.Commands.Add(new Command1Imediate("mov", format3, 0x3A, 4, 0xFFF));
            definition.Commands.Add(new Command1Imediate("cmp", format3, 0x35, 4, 0xFFF, true));
        }

        private bool GenerateRegister(AssemblerDefinition definition)
        {
            for (uint i = 0; i <= 15; i++)
            {
                definition.Registers.Add(new Register(string.Format("r{0}", i), i));
            }

            definition.Registers.Add(new Register("sp", 13));
            definition.Registers.Add(new Register("lr", 14));
            definition.Registers.Add(new Register("pc", 15));

            return true;
        }

        // -----------------------------------------------
    }
}