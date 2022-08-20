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
            ArmAFormat1 format1 = new ArmAFormat1(this);
            definition.Formats.Add(format1);

            definition.Commands.Add(new Command3Register("adc", format1, 0x0A, 4));
            definition.Commands.Add(new Command3Register("add", format1, 0x08, 4));
            definition.Commands.Add(new Command3Register("and", format1, 0x00, 4));
        }

        private void T3ImmediateDefinitionen(AssemblerDefinition definition)
        {
            ArmAFormat3 format3 = new ArmAFormat3(this);
            definition.Formats.Add(format3);

            definition.Commands.Add(new Command1Imediate("mov", format3, 0x3A, 4, 0xFFF));
            definition.Commands.Add(new Command1Imediate("cmp", format3, 0x35, 4, 0xFFF, true));

            definition.Commands.Add(new Command2Register1Imediate("add", format3, 0x28, 4));
            definition.Commands.Add(new Command2Register1Imediate("and", format3, 0x20, 4));
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

        public uint GetCondition(ConditionMode condition)
        {
            if (condition == ConditionMode.Always) return 0xe;
            if (condition == ConditionMode.Equal) return 0x0;
            if (condition == ConditionMode.NotEqual) return 0x1;
            if (condition == ConditionMode.UnsignedGreaterThanOrEqual) return 0x2;
            if (condition == ConditionMode.UnsignedLessThan) return 0x3;
            //if (condition == ConditionMode.UnsignedLessThan) return 0x4; Nagative
            //if (condition == ConditionMode.UnsignedLessThan) return 0x5; Positive or zero
            //if (condition == ConditionMode.UnsignedLessThan) return 0x6; overflow
            //if (condition == ConditionMode.UnsignedLessThan) return 0x7; no overflow
            if (condition == ConditionMode.UnsignedGreaterThan) return 0x8;
            if (condition == ConditionMode.UnsignedLessThanOrEqual) return 0x9;
            if (condition == ConditionMode.SignedGreaterThanOrEqual) return 0xa;
            if (condition == ConditionMode.SignedLessThan) return 0xb;
            if (condition == ConditionMode.SignedGreaterThan) return 0xc;
            if (condition == ConditionMode.SignedLessThanOrEqual) return 0xd;

            return 0;
        }

        // -----------------------------------------------
    }
}