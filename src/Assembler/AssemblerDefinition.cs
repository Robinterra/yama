using System.Collections.Generic;
using Yama.Assembler.Definitions;

namespace Yama.Assembler
{
    public class AssemblerDefinition
    {

        public string? Name
        {
            get;
            set;
        }

        public List<Register> Registers
        {
            get;
            set;
        } = new List<Register>();

        public List<ICommand> Commands
        {
            get;
            set;
        } = new List<ICommand>();

        public List<IFormat> Formats
        {
            get;
            set;
        } = new List<IFormat>();
        public uint ProgramCounterIncress
        {
            get;
            set;
        }

        public uint CommandEntitySize
        {
            get;
            set;
        }

        public Arm2x86Def? Arm2x86 { get; internal set; }
    }
}