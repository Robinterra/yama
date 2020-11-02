using System.Collections.Generic;

namespace Yama.Assembler
{
    public class AssemblerDefinition
    {

        public string Name
        {
            get;
            set;
        }

        public List<Register> Registers
        {
            get;
            set;
        }

        public List<ICommand> Commands
        {
            get;
            set;
        }

        public List<IFormat> Formats
        {
            get;
            set;
        }
    }
}