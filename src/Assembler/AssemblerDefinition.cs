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
    }
}