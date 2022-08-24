using Yama.Assembler.Runtime;

namespace Yama.Assembler.Definitions
{

    public class RuntimeDefinition : IAssemblerDefinition
    {

        // -----------------------------------------------
        public string Name
        {
            get
            {
                return "runtime";
            }
        }

        public uint GetCondition(ConditionMode condition)
        {
            if (condition == ConditionMode.Always) return 0;
            if (condition == ConditionMode.Equal) return 1;
            if (condition == ConditionMode.NotEqual) return 2;
            if (condition == ConditionMode.UnsignedGreaterThan) return 3;
            if (condition == ConditionMode.UnsignedGreaterThanOrEqual) return 4;
            if (condition == ConditionMode.UnsignedLessThan) return 5;
            if (condition == ConditionMode.UnsignedLessThanOrEqual) return 6;
            if (condition == ConditionMode.SkipNext) return 0xf;

            return 0;
        }

        public Assembler SetupDefinition(Assembler assembler)
        {
            assembler.DataAddition = 4;
            assembler.Definition = new AssemblerDefinition();
            assembler.Definition.ProgramCounterIncress = 4;
            assembler.Definition.CommandEntitySize = 4;

            this.RuntimeFormat1Def ( assembler.Definition );
            this.RuntimeFormat2Def ( assembler.Definition );
            this.RuntimeFormat3Def ( assembler.Definition );
            this.RuntimeGenerateRegister ( assembler.Definition );

            return assembler;
        }

        private bool RuntimeFormat1Def(AssemblerDefinition definition)
        {
            Format1 format1 = new Format1(this);
            definition.Formats.Add(format1);

            definition.Commands.Add(new Command3Register("add", format1, 0x50, 4));
            definition.Commands.Add(new Command3Register("adc", format1, 0x51, 4));
            definition.Commands.Add(new Command3Register("sub", format1, 0x52, 4));
            definition.Commands.Add(new Command3Register("sbc", format1, 0x53, 4));
            definition.Commands.Add(new Command3Register("mul", format1, 0x54, 4));
            definition.Commands.Add(new Command3Register("div", format1, 0x55, 4));
            definition.Commands.Add(new Command3Register("and", format1, 0x56, 4));
            definition.Commands.Add(new Command3Register("eor", format1, 0x57, 4));
            definition.Commands.Add(new Command3Register("orr", format1, 0x58, 4));
            definition.Commands.Add(new Command3Register("lsr", format1, 0x59, 4));
            definition.Commands.Add(new Command3Register("lsl", format1, 0x5A, 4));

            definition.Commands.Add(new Command1Register("bx", format1, 0x30, 4));
            definition.Commands.Add(new Command1Register("blx", format1, 0x31, 4));

            definition.Commands.Add(new Command1Register("exec", format1, 0xFF, 4));
            //definition.Commands.Add(new Command1Register("end", "Format1", 0xFE, 4));

            definition.Commands.Add(new Command2Register("mov", format1, 0x5E, 4));
            definition.Commands.Add(new Command2Register("cmp", format1, 0x5F, 4));

            return true;
        }

        // -----------------------------------------------
        private bool RuntimeFormat2Def(AssemblerDefinition definition)
        {
            Format2 format2 = new Format2(this);
            definition.Formats.Add(format2);

            definition.Commands.Add(new Command2Register1Imediate("add", format2, 0x10, 4));
            definition.Commands.Add(new Command2Register1Imediate("adc", format2, 0x11, 4));
            definition.Commands.Add(new Command2Register1Imediate("sub", format2, 0x12, 4));
            definition.Commands.Add(new Command2Register1Imediate("sbc", format2, 0x13, 4));
            definition.Commands.Add(new Command2Register1Imediate("mul", format2, 0x14, 4));
            definition.Commands.Add(new Command2Register1Imediate("div", format2, 0x15, 4));
            definition.Commands.Add(new Command2Register1Imediate("and", format2, 0x16, 4));
            definition.Commands.Add(new Command2Register1Imediate("eor", format2, 0x17, 4));
            definition.Commands.Add(new Command2Register1Imediate("orr", format2, 0x18, 4));
            definition.Commands.Add(new Command2Register1Imediate("lsr", format2, 0x19, 4));
            definition.Commands.Add(new Command2Register1Imediate("lsl", format2, 0x1A, 4));

            definition.Commands.Add(new Command1Register1Container("ldr", format2, 0x1B, 4, 15, 4));
            definition.Commands.Add(new Command1Register1Container("str", format2, 0x1C, 4, 15, 4));

            definition.Commands.Add(new Command1RegisterJumpPoint("ldr", format2, 0x1B, 8, ConditionMode.SkipNext));
            definition.Commands.Add(new Command1RegisterConst("ldr", format2, 0x1B, 8, ConditionMode.SkipNext));

            return true;
        }
        // -----------------------------------------------
        private bool RuntimeFormat3Def(AssemblerDefinition definition)
        {
            Format3 format3 = new Format3(this);
            definition.Formats.Add(format3);

            definition.Commands.Add(new Command1Imediate("mov", format3, 0x2E, 4, 0xFFFF));
            definition.Commands.Add(new Command1Imediate("cmp", format3, 0x2F, 4, 0xFFFF));

            definition.Commands.Add(new CommandF3List("push", format3, 0x40, 4, 15));
            definition.Commands.Add(new CommandF3List("pop", format3, 0x41, 4, 15));

            definition.Commands.Add(new Command1Register("mrs", format3, 0x42, 4));

            definition.Commands.Add(new CommandJumpPoint("b", format3, 0x32, 4, 0x1FFFF, ConditionMode.Always));
            definition.Commands.Add(new CommandJumpPoint("beq", format3, 0x32, 4, 0x1FFFF, ConditionMode.Equal));
            definition.Commands.Add(new CommandJumpPoint("bne", format3, 0x32, 4, 0x1FFFF, ConditionMode.NotEqual));
            definition.Commands.Add(new CommandJumpPoint("bgt", format3, 0x32, 4, 0x1FFFF, ConditionMode.UnsignedGreaterThan));
            definition.Commands.Add(new CommandJumpPoint("bge", format3, 0x32, 4, 0x1FFFF, ConditionMode.UnsignedGreaterThanOrEqual));
            definition.Commands.Add(new CommandJumpPoint("blt", format3, 0x32, 4, 0x1FFFF, ConditionMode.UnsignedLessThan));
            definition.Commands.Add(new CommandJumpPoint("ble", format3, 0x32, 4, 0x1FFFF, ConditionMode.UnsignedLessThanOrEqual));

            definition.Commands.Add(new CommandData());
            definition.Commands.Add(new CommandDataList(4));

            return true;
        }

        // -----------------------------------------------

        private bool RuntimeGenerateRegister(AssemblerDefinition definition)
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