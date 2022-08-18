using Yama.Assembler.ARMT32;
using Yama.Assembler.Runtime;

namespace Yama.Assembler.Definitions
{

    public class ArmT32Definition : IAssemblerDefinition
    {

        // -----------------------------------------------

        public string Name
        {
            get
            {
                return "arm-t32";
            }
        }

        // -----------------------------------------------

        private bool T3ImmediateDefinitionen(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T3ImmediateFormat());
            definition.Formats.Add(new T2LDRSPFormat());
            definition.Formats.Add(new T3LdrRegisterFormat());
            definition.Formats.Add(new ConstFormat());
            definition.Formats.Add(new T2BranchImmediateFormat());
            definition.Formats.Add(new T1InputOutputSmallFormat());

            definition.Commands.Add(new T3ImmediateCommand ("adc", "T3Immediate", 0xF14, 4));
            definition.Commands.Add(new T3ImmediateCommand ("add", "T3Immediate", 0xF10, 4));
            definition.Commands.Add(new T3ImmediateCommand ("sub", "T3Immediate", 0xF1A, 4));
            definition.Commands.Add(new T3ImmediateCommand ("sbc", "T3Immediate", 0xF16, 4));
            //definition.Commands.Add(new T3ImmediateCommand ("mul", "T3Immediate", 0xFB0, 4));
            definition.Commands.Add(new T3ImmediateCommand ("and", "T3Immediate", 0xF00, 4));
            definition.Commands.Add(new T2RegisterImmediateCommand("cmp", "T3Immediate", 0xF1B, 4, 0x7ff));
            definition.Commands.Add(new T3ImmediateCommand("eor", "T3Immediate", 0xF08, 4));
            definition.Commands.Add(new T3ImmediateCommand("orr", "T3Immediate", 0xF04, 4));

            definition.Commands.Add(new T2LdrPointerCommand("ldr", "T3LdrRegister", 0xF8D, 4, 0xf, 1));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("ldr", "T2LdrSp", 0x13, 2, 0x7, 4, true));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("ldr", "T1InputOutputSmall", 0xD, 2, 0x7, 4));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("ldr", "T3LdrRegister", 0xF8D, 4, 0xf, 1));
            definition.Commands.Add(new T2LdrConstCommand("ldr", "T3LdrRegister", 0xF8D, 10));
            definition.Commands.Add(new T2LdrJumpCommand("ldr", "T3LdrRegister", 0xF8D, 10));

            definition.Commands.Add(new T2LdrArrayRegisterCommand("str", "T2LdrSp", 0x12, 2, 0x7, 4, true));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("str", "T1InputOutputSmall", 0xC, 2, 0x7, 4));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("str", "T3LdrRegister", 0xF8C, 4, 0xe, 1));

            definition.Commands.Add(new T2RegisterImmediateCommand("mov", "T3Immediate", 0xF04, 4, 0xff));
            definition.Commands.Add(new T2LdrConstCommand("mov", "T3LdrRegister", 0xF8D, 10));

            definition.Commands.Add(new CommandData());
            definition.Commands.Add(new CommandWord());
            definition.Commands.Add(new CommandSpace());
            definition.Commands.Add(new CommandDataList(0));

            return true;
        }

        // -----------------------------------------------

        private bool T1RegisterDefinition(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T1SmallRegisterFormat());
            definition.Formats.Add(new T1MrsFormat());

            definition.Commands.Add(new T1RegistersCommand("cmp", "T1Register", 0x45, 2));
            definition.Commands.Add(new T1RegistersCommand("mov", "T1Register", 0x46, 2));
            definition.Commands.Add(new T1RegisterCommand("mrs", "T1MrsFormat", 0xF3E, 4));

            return true;
        }

        // -----------------------------------------------


        private bool T3RegisterDefinitionen(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T3RegisterFormat());
            definition.Formats.Add(new T2RegisterListFormat());

            definition.Commands.Add(new T3RegisterCommand ("adc", "T3Register", 0xEB4, 4));
            definition.Commands.Add(new T3RegisterCommand ("add", "T3Register", 0xEB0, 4));
            definition.Commands.Add(new T3RegisterCommand ("sbc", "T3Register", 0xEB6, 4));
            definition.Commands.Add(new T3RegisterCommand ("sub", "T3Register", 0xEBA, 4));
            definition.Commands.Add(new T3RegisterCommand ("mul", "T3Register", 0xFB0, 4, true));
            definition.Commands.Add(new T3RegisterCommand ("udiv", "T3Register", 0xFBB, 4, true, true));
            definition.Commands.Add(new T3RegisterCommand ("and", "T3Register", 0xEA0, 4));
            definition.Commands.Add(new T3RegisterCommand ("eor", "T3Register", 0xEA8, 4));
            definition.Commands.Add(new T3RegisterCommand ("orr", "T3Register", 0xEA4, 4));
            definition.Commands.Add(new T3StackOneRegisterCommand ("pop", "T3LdrRegister", 0xF85, 4, 0xB04));
            definition.Commands.Add(new T1RegisterListeCommand ("pop", "T2RegisterList", 0x8BD, 4, 14));
            definition.Commands.Add(new T3StackOneRegisterCommand ("push", "T3LdrRegister", 0xF84, 4, 0xD04));
            definition.Commands.Add(new T1RegisterListeCommand ("push", "T2RegisterList", 0x92D, 4, 14));


            return true;
        }

        // -----------------------------------------------

        private bool BranchesDefinitionen(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T4BigBranchFormat());
            definition.Formats.Add(new T1BranchRegisterFormat());
            definition.Formats.Add(new T1BranchConditionFormat());

            definition.Commands.Add(new T4BranchCommand("b", "T2BranchImmediate", 0x1C, 2, 0x3FF));
            //definition.Commands.Add(new T4BranchCommand("b", "T4BigBranch", 0, 4));
            definition.Commands.Add(new T1RegisterCommand("blx", "T1BLX", 0x08F, 2));
            definition.Commands.Add(new T1RegisterCommand("bx", "T1BLX", 0x08E, 2));
            definition.Commands.Add(new T4BranchCommand("beq", "T1BranchCondition", 0xD0, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bne", "T1BranchCondition", 0xD1, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bcs", "T1BranchCondition", 0xD2, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bcc", "T1BranchCondition", 0xD3, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bvs", "T1BranchCondition", 0xD6, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bvc", "T1BranchCondition", 0xD7, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bge", "T1BranchCondition", 0xDA, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("blt", "T1BranchCondition", 0xDB, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bgt", "T1BranchCondition", 0xDC, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("ble", "T1BranchCondition", 0xDD, 2, 0xFF));

            return true;
        }

        // -----------------------------------------------

        private bool T3asrDefinition(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T3AsrImmediateFormat());
            definition.Formats.Add(new T3AsrRegisterFormat());

            definition.Commands.Add(new T3ImmediateCommand ("lsr", "T3AsrIm", 0xEA4, 4, 1));
            definition.Commands.Add(new T3RegisterCommand ("lsr", "T3AsrRe", 0xFA2, 4));

            definition.Commands.Add(new T3ImmediateCommand ("lsl", "T3AsrIm", 0xEA4, 4, 0));
            definition.Commands.Add(new T3RegisterCommand ("lsl", "T3AsrRe", 0xFA0, 4));

            return true;
        }

        // -----------------------------------------------

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

        public Assembler SetupDefinition(Assembler assembler)
        {
            assembler.Definition = new AssemblerDefinition();
            assembler.Definition.ProgramCounterIncress = 4;
            assembler.Definition.CommandEntitySize = 2;

            this.T3ImmediateDefinitionen ( assembler.Definition );
            this.T3RegisterDefinitionen ( assembler.Definition );
            this.T3asrDefinition ( assembler.Definition );
            this.T1RegisterDefinition ( assembler.Definition );
            this.BranchesDefinitionen ( assembler.Definition );
            this.GenerateRegister ( assembler.Definition );

            return assembler;
        }

    }

}