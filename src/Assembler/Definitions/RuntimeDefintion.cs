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
            Format1 format1 = new Format1();
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

            definition.Commands.Add(new Command1Register("bx", "Format1", 0x30, 4));
            definition.Commands.Add(new Command1Register("blx", "Format1", 0x31, 4));

            definition.Commands.Add(new Command1Register("exec", "Format1", 0xFF, 4));
            //definition.Commands.Add(new Command1Register("end", "Format1", 0xFE, 4));

            definition.Commands.Add(new Command2Register("mov", "Format1", 0x5E, 4));
            definition.Commands.Add(new Command2Register("cmp", "Format1", 0x5F, 4));

            return true;
        }

        // -----------------------------------------------
        private bool RuntimeFormat2Def(AssemblerDefinition definition)
        {
            definition.Formats.Add(new Format2());

            definition.Commands.Add(new Command2Register1Imediate("add", "Format2", 0x10, 4));
            definition.Commands.Add(new Command2Register1Imediate("adc", "Format2", 0x11, 4));
            definition.Commands.Add(new Command2Register1Imediate("sub", "Format2", 0x12, 4));
            definition.Commands.Add(new Command2Register1Imediate("sbc", "Format2", 0x13, 4));
            definition.Commands.Add(new Command2Register1Imediate("mul", "Format2", 0x14, 4));
            definition.Commands.Add(new Command2Register1Imediate("div", "Format2", 0x15, 4));
            definition.Commands.Add(new Command2Register1Imediate("and", "Format2", 0x16, 4));
            definition.Commands.Add(new Command2Register1Imediate("eor", "Format2", 0x17, 4));
            definition.Commands.Add(new Command2Register1Imediate("orr", "Format2", 0x18, 4));
            definition.Commands.Add(new Command2Register1Imediate("lsr", "Format2", 0x19, 4));
            definition.Commands.Add(new Command2Register1Imediate("lsl", "Format2", 0x1A, 4));

            definition.Commands.Add(new Command1Register1Container("ldr", "Format2", 0x1B, 4, 15, 4));
            definition.Commands.Add(new Command1Register1Container("str", "Format2", 0x1C, 4, 15, 4));

            definition.Commands.Add(new Command1RegisterJumpPoint("ldr", "Format2", 0x1B, 8, 0xF));
            definition.Commands.Add(new Command1RegisterConst("ldr", "Format2", 0x1B, 8, 0xF));

            return true;
        }
        // -----------------------------------------------
        private bool RuntimeFormat3Def(AssemblerDefinition definition)
        {
            Format3 format3 = new Format3();
            definition.Formats.Add(format3);

            definition.Commands.Add(new Command1Imediate("mov", format3, 0x2E, 4));
            definition.Commands.Add(new Command1Imediate("cmp", format3, 0x2F, 4));

            definition.Commands.Add(new CommandF3List("push", "Format3", 0x40, 4, 15));
            definition.Commands.Add(new CommandF3List("pop", "Format3", 0x41, 4, 15));

            definition.Commands.Add(new Command1Register("mrs", "Format3", 0x42, 4));

            definition.Commands.Add(new CommandJumpPoint("b", "Format3", 0x32, 4, 0x1FFFF, 0));
            definition.Commands.Add(new CommandJumpPoint("beq", "Format3", 0x32, 4, 0x1FFFF, 1));
            definition.Commands.Add(new CommandJumpPoint("bne", "Format3", 0x32, 4, 0x1FFFF, 2));
            definition.Commands.Add(new CommandJumpPoint("bgt", "Format3", 0x32, 4, 0x1FFFF, 3));
            definition.Commands.Add(new CommandJumpPoint("bge", "Format3", 0x32, 4, 0x1FFFF, 4));
            definition.Commands.Add(new CommandJumpPoint("blt", "Format3", 0x32, 4, 0x1FFFF, 5));
            definition.Commands.Add(new CommandJumpPoint("ble", "Format3", 0x32, 4, 0x1FFFF, 6));

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