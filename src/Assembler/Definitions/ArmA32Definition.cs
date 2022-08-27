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
            assembler.DataAddition = 0;
            assembler.Definition = new AssemblerDefinition();
            assembler.Definition.ProgramCounterIncress = 4;
            assembler.Definition.CommandEntitySize = 4;
            assembler.osHeader = ProjectConfig.Project.OSHeader.LinuxArm;

            this.T3RegisterDefinitionen ( assembler.Definition );
            //this.T3asrDefinition ( assembler.Definition );
            //this.T1RegisterDefinition ( assembler.Definition );
            ArmAFormat2 format2 = this.BranchesDefinitionen ( assembler.Definition );
            this.T3ImmediateDefinitionen ( assembler.Definition, format2 );
            this.GenerateRegister ( assembler.Definition );

            return assembler;
        }

        private ArmAFormat2 BranchesDefinitionen(AssemblerDefinition definition)
        {
            ArmAFormat2 format2 = new ArmAFormat2(this);
            definition.Formats.Add(format2);

            ArmAFormat4 format4 = new ArmAFormat4(this);
            definition.Formats.Add(format4);

            definition.Commands.Add(new Command1Register("blx", format4, 0x12, 4, sonder: 3));
            definition.Commands.Add(new Command1Register("bx", format4, 0x12, 4, sonder: 1));

            definition.Commands.Add(new CommandJumpPoint("b", format2, 0xA, 4, 0xFFFFFF, ConditionMode.Always, true));
            definition.Commands.Add(new CommandJumpPoint("beq", format2, 0xA, 4, 0xFFFFFF, ConditionMode.Equal, true));
            definition.Commands.Add(new CommandJumpPoint("bne", format2, 0xA, 4, 0xFFFFFF, ConditionMode.NotEqual, true));
            definition.Commands.Add(new CommandJumpPoint("bgt", format2, 0xA, 4, 0xFFFFFF, ConditionMode.UnsignedGreaterThan, true));
            definition.Commands.Add(new CommandJumpPoint("bge", format2, 0xA, 4, 0xFFFFFF, ConditionMode.UnsignedGreaterThanOrEqual, true));
            definition.Commands.Add(new CommandJumpPoint("blt", format2, 0xA, 4, 0xFFFFFF, ConditionMode.UnsignedLessThan, true));
            definition.Commands.Add(new CommandJumpPoint("ble", format2, 0xA, 4, 0xFFFFFF, ConditionMode.UnsignedGreaterThanOrEqual, true));

            definition.Commands.Add(new CommandImediate("svc", format2, 0xf, 4, 0xfffff));

            definition.Commands.Add(new CommandData());
            definition.Commands.Add(new CommandDataList(0));

            return format2;
        }

        private void T3RegisterDefinitionen(AssemblerDefinition definition)
        {
            ArmAFormat1 format1 = new ArmAFormat1(this);
            definition.Formats.Add(format1);

            ArmAFormat5 format5 = new ArmAFormat5(this);
            definition.Formats.Add(format5);

            ArmAFormat6 format6 = new ArmAFormat6(this);
            definition.Formats.Add(format6);

            definition.Commands.Add(new Command2Register("cmp", format1, 0x15, 4, Command2Register.RegisterMode.Left_Right));

            definition.Commands.Add(new Command3Register("adc", format1, 0x0A, 4));
            definition.Commands.Add(new Command3Register("add", format1, 0x08, 4));
            definition.Commands.Add(new Command3Register("and", format1, 0x00, 4));
            definition.Commands.Add(new Command3Register("eor", format1, 0x02, 4));
            definition.Commands.Add(new Command3Register("orr", format1, 0x18, 4));
            definition.Commands.Add(new Command3Register("sub", format1, 0x04, 4));

            definition.Commands.Add(new Command3Register("mul", format6, 0x00, 4, stype: 9));

            definition.Commands.Add(new Command2Register1Imediate("lsl", format1, 0x1A, 4, true));
            definition.Commands.Add(new Command2Register1Imediate("lsr", format1, 0x1A, 4, true, stype: 2));
            definition.Commands.Add(new Command3Register("lsl", format5, 0x1A, 4, stype: 1));
            definition.Commands.Add(new Command3Register("lsr", format5, 0x1A, 4, stype: 3));

            definition.Commands.Add(new Command2Register("mov", format1, 0x1A, 4, Command2Register.RegisterMode.Destitination_Right));

            definition.Commands.Add(new Command1Register("mrs", format1, 0x10, 4, registerLeft: 0xf));
        }

        private void T3ImmediateDefinitionen(AssemblerDefinition definition, ArmAFormat2 format2)
        {
            ArmAFormat3 format3 = new ArmAFormat3(this);
            definition.Formats.Add(format3);

            ArmAFormat7 format7 = new ArmAFormat7(this);
            definition.Formats.Add(format7);

            definition.Commands.Add(new Command1Imediate("mov", format3, 0x3A, 4, 0xFFF));
            definition.Commands.Add(new ArmLdrConst("mov", format3, 0x59, format2, 0xa, 12));
            definition.Commands.Add(new Command1Imediate("cmp", format3, 0x35, 4, 0xFFF, true));

            definition.Commands.Add(new Command2Register1Imediate("adc", format3, 0x2A, 4));
            definition.Commands.Add(new Command2Register1Imediate("add", format3, 0x28, 4));
            definition.Commands.Add(new Command2Register1Imediate("and", format3, 0x20, 4));
            definition.Commands.Add(new Command2Register1Imediate("eor", format3, 0x22, 4));
            definition.Commands.Add(new Command2Register1Imediate("orr", format3, 0x38, 4));
            definition.Commands.Add(new Command2Register1Imediate("sub", format3, 0x24, 4));

            definition.Commands.Add(new Command1Register1Container("ldr", format3, 0x59, 4, 15, 1));
            definition.Commands.Add(new Command1Register1Container("str", format3, 0x58, 4, 15, 1));

            definition.Commands.Add(new ArmLdrJumpPoint("ldr", format3, 0x59, format2, 0xa, 12));
            definition.Commands.Add(new ArmLdrConst("ldr", format3, 0x59, format2, 0xa, 12));

            definition.Commands.Add(new CommandF3List("push", format7, 0x92, 4, 15));
            definition.Commands.Add(new CommandF3List("pop", format7, 0x8B, 4, 15));
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